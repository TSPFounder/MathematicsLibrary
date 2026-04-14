#nullable enable
using System;
using System.Collections.Generic;



namespace Mathematics
{
    public class Point
    {
        // -----------------------------
        // Types
        // -----------------------------
        public enum PointTypeEnum
        {
            Cartesian = 0,
            Cylindrical,
            Spherical,
            Complex
        }

        // -----------------------------
        // Constructors
        // -----------------------------

        /// <summary>
        /// Creates a Cartesian point at (x, y, z).
        /// </summary>
        public Point(double x, double y, double z = 0.0)
            : this(PointTypeEnum.Cartesian, x, y, z) { }

        /// <summary>
        /// Creates a point of the specified coordinate type with position values.
        /// <list type="bullet">
        ///   <item><b>Cartesian</b>: coord1 = x, coord2 = y, coord3 = z</item>
        ///   <item><b>Cylindrical</b>: coord1 = r, coord2 = θ (radians), coord3 = z</item>
        ///   <item><b>Spherical</b>: coord1 = r, coord2 = θ (radians), coord3 = φ (radians)</item>
        ///   <item><b>Complex</b>: coord1 = real, coord2 = imaginary, coord3 unused</item>
        /// </list>
        /// </summary>
        public Point(PointTypeEnum type, double coord1, double coord2, double coord3 = 0.0)
        {
            MyType = type;
            MyCoordinateSystems = new List<CoordinateSystem>();
            MyConnectedPoints = new List<Point>();

            switch (type)
            {
                case PointTypeEnum.Cartesian:
                    X_Value = coord1;
                    Y_Value = coord2;
                    Z_Value_Cartesian = coord3;
                    Is2D = coord3 == 0.0;
                    break;

                case PointTypeEnum.Cylindrical:
                    R_Value_Cylindrical = coord1;
                    Theta_Value_Cylindrical = coord2;
                    Z_Value_Cylindrical = coord3;
                    Is2D = coord3 == 0.0;
                    break;

                case PointTypeEnum.Spherical:
                    R_Value_Spherical = coord1;
                    Theta_Value_Spherical = coord2;
                    Phi_Value = coord3;
                    Is2D = false;
                    break;

                case PointTypeEnum.Complex:
                    Real_Value = coord1;
                    Complex_Value = coord2;
                    Is2D = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown point type.");
            }
        }

        // -----------------------------
        // Factories (convenience wrappers)
        // -----------------------------

        public static Point FromCartesian(double x, double y, double z = 0.0)
            => new Point(PointTypeEnum.Cartesian, x, y, z);

        public static Point FromCylindrical(double r, double thetaRadians, double z = 0.0)
            => new Point(PointTypeEnum.Cylindrical, r, thetaRadians, z);

        public static Point FromSpherical(double r, double thetaRadians, double phiRadians)
            => new Point(PointTypeEnum.Spherical, r, thetaRadians, phiRadians);

        public static Point FromComplex(double real, double imaginary)
            => new Point(PointTypeEnum.Complex, real, imaginary);

        // -----------------------------
        // Identification
        // -----------------------------
        public string? PointID { get; set; }
        public bool IsWeightPoint { get; set; }

        // -----------------------------
        // Kind / flags
        // -----------------------------
        public PointTypeEnum MyType { get; set; }
        public bool Is2D { get; set; }

        // -----------------------------
        // Geometry – Cartesian
        // -----------------------------
        public double X_Value { get; set; }
        public double Y_Value { get; set; }
        public double Z_Value_Cartesian { get; set; }

        // -----------------------------
        // Geometry – Cylindrical (r, θ, z)
        // -----------------------------
        public double R_Value_Cylindrical { get; set; }
        public double Theta_Value_Cylindrical { get; set; }   // radians
        public double Z_Value_Cylindrical { get; set; }

        // -----------------------------
        // Geometry – Spherical (r, θ, φ)
        // θ: azimuth about +Z from +X; φ: polar angle from +Z (0..π)
        // -----------------------------
        public double R_Value_Spherical { get; set; }
        public double Theta_Value_Spherical { get; set; }   // radians
        public double Phi_Value { get; set; }   // radians

        // -----------------------------
        // GPS (optional)
        // -----------------------------
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }

        // -----------------------------
        // Complex (optional)
        // -----------------------------
        public double Real_Value { get; set; }
        public double Complex_Value { get; set; }

        // -----------------------------
        // Coordinate systems / connectivity
        // -----------------------------
        public CoordinateSystem? CurrentCoordinateSystem { get; set; }
        public List<CoordinateSystem> MyCoordinateSystems { get; set; }
        public Point? CurrentConnectedPoint { get; set; }
        public List<Point> MyConnectedPoints { get; set; }

        // -----------------------------
        // Conversions (in-place)
        // -----------------------------

        /// <summary>Convert cylindrical (r,θ,z) to Cartesian, if MyType == Cylindrical.</summary>
        public bool CylindricalToCartesian(double r, double thetaRadians)
        {
            try
            {
                if (MyType != PointTypeEnum.Cylindrical) return false;

                X_Value = r * Math.Cos(thetaRadians);
                Y_Value = r * Math.Sin(thetaRadians);
                Z_Value_Cartesian = Z_Value_Cylindrical;
                return true;
            }
            catch { return false; }
        }

        /// <summary>Convert spherical (r,θ,φ) to Cartesian, if MyType == Spherical.
        /// θ = azimuth, φ = polar angle from +Z.</summary>
        public bool SphericalToCartesian(double r, double thetaRadians, double phiRadians)
        {
            try
            {
                if (MyType != PointTypeEnum.Spherical) return false;

                X_Value = r * Math.Cos(thetaRadians) * Math.Sin(phiRadians);
                Y_Value = r * Math.Sin(thetaRadians) * Math.Sin(phiRadians);
                Z_Value_Cartesian = r * Math.Cos(phiRadians);
                return true;
            }
            catch { return false; }
        }

        /// <summary>Convert Cartesian (x,y) to cylindrical (r,θ,z), if MyType == Cartesian.</summary>
        public bool CartesianToCylindrical(double x, double y)
        {
            try
            {
                if (MyType != PointTypeEnum.Cartesian) return false;

                R_Value_Cylindrical = Math.Sqrt(x * x + y * y);
                Theta_Value_Cylindrical = Math.Atan2(y, x);
                Z_Value_Cylindrical = Z_Value_Cartesian;
                return true;
            }
            catch { return false; }
        }

        /// <summary>Convert spherical (r,φ) to cylindrical (r,θ,z), if MyType == Spherical.</summary>
        public bool SphericalToCylindrical(double r, double phiRadians)
        {
            try
            {
                if (MyType != PointTypeEnum.Spherical) return false;

                R_Value_Cylindrical = r * Math.Sin(phiRadians);
                Z_Value_Cylindrical = r * Math.Cos(phiRadians);
                Theta_Value_Cylindrical = Theta_Value_Spherical;
                return true;
            }
            catch { return false; }
        }

        /// <summary>Convert Cartesian (x,y,z) to spherical (r,θ,φ), if MyType == Cartesian.</summary>
        public bool CartesianToSpherical(double x, double y, double z)
        {
            try
            {
                if (MyType != PointTypeEnum.Cartesian) return false;

                R_Value_Spherical = Math.Sqrt(x * x + y * y + z * z);
                Theta_Value_Spherical = Math.Atan2(y, x);
                Phi_Value = Math.Atan2(Math.Sqrt(x * x + y * y), z);
                return true;
            }
            catch { return false; }
        }

        /// <summary>Convert cylindrical (r,z) to spherical (r,θ,φ), if MyType == Cylindrical.</summary>
        public bool CylindricalToSpherical(double r, double z)
        {
            try
            {
                if (MyType != PointTypeEnum.Cylindrical) return false;

                R_Value_Spherical = Math.Sqrt(r * r + z * z);
                Phi_Value = Math.Atan2(r, z);
                Theta_Value_Spherical = Theta_Value_Cylindrical;
                return true;
            }
            catch { return false; }
        }

        // -----------------------------
        // Utilities
        // -----------------------------
        public static double DegreesToRadians(double angleInDegrees) => angleInDegrees * Math.PI / 180.0;
        public static double RadiansToDegrees(double angleInRadians) => angleInRadians * 180.0 / Math.PI;

        public void ConnectTo(Point other)
        {
            if (other is null) throw new ArgumentNullException(nameof(other));
            MyConnectedPoints.Add(other);
            CurrentConnectedPoint ??= other;
        }

        public override string ToString()
            => $"Point(ID={PointID ?? "<null>"}, Type={MyType}, " +
               $"Cart=({X_Value:F3},{Y_Value:F3},{Z_Value_Cartesian:F3}))";
    }
}
