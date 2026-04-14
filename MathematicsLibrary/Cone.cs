#nullable enable
using System;
using System.Collections.Generic;



namespace Mathematics
{
    /// <summary>
    /// 3D cone primitive. Supports both a full cone (apex-to-base) and a frustum (truncated cone).
    /// </summary>
    public sealed class Cone : Primitive
    {
        // ------------------------------------------------------------
        // Constructors
        // ------------------------------------------------------------

        /// <summary>
        /// Creates a cone with default center, unit base circle, and unit height.
        /// </summary>
        public Cone()
        {
            Is2D = false;
            ThreeDType = ThreeDPrimitiveTypeEnum.Cone;

            CenterPoint = new Point(0, 0, 0);
            BaseCircle = new Circle();
            Height = 1.0;
        }

        /// <summary>
        /// Creates a cone or frustum.
        /// </summary>
        /// <param name="baseCircle">Base circle of the cone.</param>
        /// <param name="height">Axial height (≥ 0). For a frustum, this is the distance between base and truncation planes.</param>
        /// <param name="truncationCircle">Optional top circle; if specified, represents a frustum (truncated cone).</param>
        public Cone(Circle baseCircle, double height, Circle? truncationCircle = null)
            : this()
        {
            BaseCircle = baseCircle;
            Height = height;
            TruncationCircle = truncationCircle;
        }

        // ------------------------------------------------------------
        // Identification
        // ------------------------------------------------------------
        public string? ConeID { get; set; }

        // ------------------------------------------------------------
        // Dimensions / Geometry
        // ------------------------------------------------------------

        /// <summary>
        /// Axial height (≥ 0). For a frustum, this is the distance between base and truncation planes.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// True when a truncation circle exists (i.e., this is a frustum).
        /// </summary>
        public bool IsTruncated => TruncationCircle is not null;

        /// <summary>
        /// Base circle of the cone or frustum. Its radius defines the base radius R.
        /// </summary>
        public Circle BaseCircle { get; set; } = new Circle();

        /// <summary>
        /// Optional truncation (top) circle. If present, its radius defines the top radius r for a frustum.
        /// When null, the cone is a full cone (apex radius 0).
        /// </summary>
        public Circle? TruncationCircle { get; set; }

        /// <summary>
        /// Convenience accessor for the base radius R.
        /// </summary>
        public double BaseRadius => BaseCircle?.Radius ?? 0.0;

        /// <summary>
        /// Convenience accessor for the top radius r (0 for a full cone).
        /// </summary>
        public double TopRadius => TruncationCircle?.Radius ?? 0.0;

        // ------------------------------------------------------------
        // Validation
        // ------------------------------------------------------------

        /// <summary>
        /// Returns true if geometry is valid (non-negative height and radii, base/top circles present where required).
        /// </summary>
        public bool IsValid(out string? reason)
        {
            if (Height < 0)
            {
                reason = "Height cannot be negative.";
                return false;
            }

            if (BaseCircle is null)
            {
                reason = "BaseCircle must be defined.";
                return false;
            }

            if (BaseRadius < 0)
            {
                reason = "Base radius cannot be negative.";
                return false;
            }

            if (TruncationCircle is not null && TopRadius < 0)
            {
                reason = "Top (truncation) radius cannot be negative.";
                return false;
            }

            reason = null;
            return true;
        }

        // ------------------------------------------------------------
        // Geometry helpers
        // ------------------------------------------------------------

        /// <summary>
        /// Slant height. For full cone: sqrt(R^2 + H^2). For frustum: sqrt((R - r)^2 + H^2).
        /// </summary>
        public double GetSlantHeight()
        {
            var R = BaseRadius;
            var r = TopRadius;
            var h = Height;

            if (h < 0 || R < 0 || r < 0) return double.NaN;

            var delta = IsTruncated ? (R - r) : R;
            return System.Math.Sqrt(delta * delta + h * h);
        }

        /// <summary>
        /// Lateral surface area. Full cone: π R s. Frustum: π (R + r) s.
        /// </summary>
        public double GetLateralSurfaceArea()
        {
            var R = BaseRadius;
            var r = TopRadius;
            var s = GetSlantHeight();

            if (double.IsNaN(s)) return double.NaN;

            return IsTruncated
                ? System.Math.PI * (R + r) * s
                : System.Math.PI * R * s;
        }

        /// <summary>
        /// Total surface area including base (and top if truncated).
        /// Full cone: lateral + πR². Frustum: lateral + π(R² + r²).
        /// </summary>
        public double GetTotalSurfaceArea()
        {
            var R = BaseRadius;
            var r = TopRadius;
            var lateral = GetLateralSurfaceArea();

            if (double.IsNaN(lateral)) return double.NaN;

            var baseArea = System.Math.PI * R * R;
            return IsTruncated
                ? lateral + baseArea + System.Math.PI * r * r
                : lateral + baseArea;
        }

        /// <summary>
        /// Volume. Full cone: (1/3)πR²H. Frustum: (1/3)πH(R² + Rr + r²).
        /// </summary>
        public double GetVolume()
        {
            var R = BaseRadius;
            var r = TopRadius;
            var h = Height;

            if (h < 0 || R < 0 || r < 0) return double.NaN;

            return IsTruncated
                ? (System.Math.PI * h / 3.0) * (R * R + R * r + r * r)
                : (System.Math.PI * R * R * h) / 3.0;
        }

        public override string ToString()
        {
            var id = ConeID ?? "—";
            var form = IsTruncated ? "Frustum" : "Cone";
            return $"{form}(H={Height:0.###}, Rb={BaseRadius:0.###}, Rt={TopRadius:0.###}, ID={id})";
        }
    }
}
