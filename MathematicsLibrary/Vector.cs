#nullable enable
using System;

namespace Mathematics
{
    /// <summary>
    /// Geometric vector with multiple coordinate representations and light ownership links.
    /// </summary>
    public class Vector
    {
        // -----------------------------
        // Types
        // -----------------------------
        public enum VectorTypeEnum
        {
            Cartesian = 0,
            Cylindrical,
            Spherical,
            Polar
        }

        // -----------------------------
        // Constructors / factories
        // -----------------------------
        public Vector()
        {
            WorldCoordinateSystem = CoordinateSystem.CreateWcs();
            CurrentCoordinateSystem = WorldCoordinateSystem;
        }

        /// <summary>
        /// Create a Cartesian vector from two points of any coordinate type.
        /// The vector components are computed as (end − start) in Cartesian space,
        /// so the points do not need to be at the origin.
        /// </summary>
        public Vector(Point startPoint, Point endPoint) : this()
        {
            if (startPoint is null) throw new ArgumentNullException(nameof(startPoint));
            if (endPoint is null) throw new ArgumentNullException(nameof(endPoint));

            StartPoint = startPoint;
            EndPoint = endPoint;

            // Convert both points to Cartesian regardless of their native type
            var (sx, sy, sz) = PointToCartesian(startPoint);
            var (ex, ey, ez) = PointToCartesian(endPoint);

            X_Value = ex - sx;
            Y_Value = ey - sy;
            Z_Value = ez - sz;

            VectorType = VectorTypeEnum.Cartesian;
            WorldCoordinateSystem.BaseVector ??= this;
        }

        /// <summary>Create a Cartesian vector from components.</summary>
        public static Vector FromCartesian(double x, double y, double z = 0) =>
            new Vector { VectorType = VectorTypeEnum.Cartesian, X_Value = x, Y_Value = y, Z_Value = z };

        /// <summary>Create a Cylindrical vector (r, θ, z).</summary>
        public static Vector FromCylindrical(double r, double thetaRadians, double z = 0) =>
            new Vector { VectorType = VectorTypeEnum.Cylindrical, Cyl_R = r, Cyl_Theta = thetaRadians, L = z };

        /// <summary>Create a Spherical vector (R, θ, φ). θ = azimuth, φ = polar angle from +Z.</summary>
        public static Vector FromSpherical(double r, double thetaRadians, double phiRadians) =>
            new Vector { VectorType = VectorTypeEnum.Spherical, Sph_R = r, Sph_Theta = thetaRadians, Phi = phiRadians };

        /// <summary>
        /// Create a Cartesian vector from two points of any coordinate type.
        /// Equivalent to <c>new Vector(start, end)</c>.
        /// </summary>
        public static Vector FromPoints(Point start, Point end) => new Vector(start, end);

        // -----------------------------
        // Identification
        // -----------------------------
        public string? Name { get; set; }
        public string? VectorID { get; set; }
        public bool IsKnotVector { get; set; }
        public VectorTypeEnum VectorType { get; set; } = VectorTypeEnum.Cartesian;

        // -----------------------------
        // Magnitude (computed)
        // -----------------------------
        public double Length => GetVectorLength();

        // -----------------------------
        // Cartesian components
        // -----------------------------
        public double? X_Value { get; set; }
        public double? Y_Value { get; set; }
        public double? Z_Value { get; set; }

        // -----------------------------
        // Cylindrical / Polar (r, θ, z)
        // -----------------------------
        public double? Cyl_R { get; set; }
        /// <summary>Azimuth angle θ (radians).</summary>
        public double? Cyl_Theta { get; set; }
        /// <summary>Axial component (z or L depending on usage).</summary>
        public double? L { get; set; }

        // -----------------------------
        // Spherical (R, θ, φ)
        // θ: azimuth about +Z from +X; φ: polar angle from +Z
        // -----------------------------
        public double? Sph_R { get; set; }
        public double? Sph_Theta { get; set; }
        public double? Phi { get; set; }

        // -----------------------------
        // Ownership / context
        // -----------------------------
        public Point? StartPoint { get; set; }
        public Point? EndPoint { get; set; }

        public CoordinateSystem WorldCoordinateSystem { get; set; } = CoordinateSystem.CreateWcs();
        public CoordinateSystem? CurrentCoordinateSystem { get; set; }

        // -----------------------------
        // Coordinate helpers
        // -----------------------------

        /// <summary>
        /// Convert any <see cref="Point"/> to Cartesian (x, y, z) regardless of its native type.
        /// </summary>
        public static (double X, double Y, double Z) PointToCartesian(Point p)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));

            return p.MyType switch
            {
                Point.PointTypeEnum.Cartesian => (
                    p.X_Value,
                    p.Y_Value,
                    p.Z_Value_Cartesian
                ),

                Point.PointTypeEnum.Cylindrical => (
                    p.R_Value_Cylindrical * Math.Cos(p.Theta_Value_Cylindrical),
                    p.R_Value_Cylindrical * Math.Sin(p.Theta_Value_Cylindrical),
                    p.Z_Value_Cylindrical
                ),

                Point.PointTypeEnum.Spherical => (
                    p.R_Value_Spherical * Math.Sin(p.Phi_Value) * Math.Cos(p.Theta_Value_Spherical),
                    p.R_Value_Spherical * Math.Sin(p.Phi_Value) * Math.Sin(p.Theta_Value_Spherical),
                    p.R_Value_Spherical * Math.Cos(p.Phi_Value)
                ),

                _ => (p.X_Value, p.Y_Value, p.Z_Value_Cartesian)
            };
        }

        /// <summary>
        /// Derive Cartesian (x, y, z) components for this vector.
        /// <list type="bullet">
        ///   <item>If <see cref="StartPoint"/> and <see cref="EndPoint"/> are both set,
        ///         components are re-derived as (end − start) from the points' native coordinates.</item>
        ///   <item>Otherwise, the vector's own coordinate properties are converted
        ///         from the native <see cref="VectorType"/> to Cartesian.</item>
        /// </list>
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the required coordinate properties for the current
        /// <see cref="VectorType"/> have not been set (are null).
        /// </exception>
        public (double X, double Y, double Z) ToCartesianComponents()
        {
            // When defined by two points, always re-derive from the points.
            if (StartPoint is not null && EndPoint is not null)
            {
                var (sx, sy, sz) = PointToCartesian(StartPoint);
                var (ex, ey, ez) = PointToCartesian(EndPoint);
                return (ex - sx, ey - sy, ez - sz);
            }

            // Fall back to converting the vector's own stored coordinates.
            return VectorType switch
            {
                VectorTypeEnum.Cartesian => (
                    X_Value ?? throw new InvalidOperationException("X_Value is not set for Cartesian vector."),
                    Y_Value ?? throw new InvalidOperationException("Y_Value is not set for Cartesian vector."),
                    Z_Value ?? 0.0
                ),

                VectorTypeEnum.Cylindrical => CartesianFromCylindrical(
                    Cyl_R ?? throw new InvalidOperationException("Cyl_R is not set for Cylindrical vector."),
                    Cyl_Theta ?? throw new InvalidOperationException("Cyl_Theta is not set for Cylindrical vector."),
                    L ?? 0.0
                ),

                VectorTypeEnum.Spherical => CartesianFromSpherical(
                    Sph_R ?? throw new InvalidOperationException("Sph_R is not set for Spherical vector."),
                    Sph_Theta ?? throw new InvalidOperationException("Sph_Theta is not set for Spherical vector."),
                    Phi ?? throw new InvalidOperationException("Phi is not set for Spherical vector.")
                ),

                VectorTypeEnum.Polar => CartesianFromCylindrical(
                    Cyl_R ?? throw new InvalidOperationException("Cyl_R is not set for Polar vector."),
                    Cyl_Theta ?? throw new InvalidOperationException("Cyl_Theta is not set for Polar vector."),
                    0.0
                ),

                _ => (X_Value ?? 0.0, Y_Value ?? 0.0, Z_Value ?? 0.0)
            };
        }

        // -----------------------------
        // Conversion helpers
        // -----------------------------

        private static (double X, double Y, double Z) CartesianFromCylindrical(double r, double theta, double z)
            => (r * Math.Cos(theta), r * Math.Sin(theta), z);

        private static (double X, double Y, double Z) CartesianFromSpherical(double r, double theta, double phi)
            => (r * Math.Sin(phi) * Math.Cos(theta),
                r * Math.Sin(phi) * Math.Sin(theta),
                r * Math.Cos(phi));

        // -----------------------------
        // Methods
        // -----------------------------

        /// <summary>
        /// Compute vector length from correctly derived Cartesian components.
        /// </summary>
        public double GetVectorLength()
        {
            var (x, y, z) = ToCartesianComponents();
            return Math.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Returns a unit (normalized) Cartesian vector.
        /// Works for any vector type by converting to Cartesian first.
        /// </summary>
        public Vector Normalize()
        {
            var (x, y, z) = ToCartesianComponents();
            var len = Math.Sqrt(x * x + y * y + z * z);

            if (len <= 0)
                throw new InvalidOperationException("Cannot normalize a zero-length vector.");

            return FromCartesian(x / len, y / len, z / len);
        }

        /// <summary>
        /// Dot product. Works for any vector type by converting to Cartesian first.
        /// </summary>
        public static double Dot(Vector a, Vector b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));

            var (ax, ay, az) = a.ToCartesianComponents();
            var (bx, by, bz) = b.ToCartesianComponents();

            return ax * bx + ay * by + az * bz;
        }

        /// <summary>
        /// Cross product. Works for any vector type by converting to Cartesian first.
        /// </summary>
        public static Vector Cross(Vector a, Vector b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));

            var (ax, ay, az) = a.ToCartesianComponents();
            var (bx, by, bz) = b.ToCartesianComponents();

            return FromCartesian(
                ay * bz - az * by,
                az * bx - ax * bz,
                ax * by - ay * bx
            );
        }

        public override string ToString()
        {
            var (x, y, z) = ToCartesianComponents();
            return $"Vector(Name={Name ?? "<unnamed>"}, Type={VectorType}, Len={Length:F6}, " +
                   $"Cart=({x:F3},{y:F3},{z:F3}))";
        }
    }
}