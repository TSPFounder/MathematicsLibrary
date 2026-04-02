#nullable enable
using System;

namespace MathematicsLibrary
{
    /// <summary>
    /// Complex number z = Real + Imaginary·i, with polar form support.
    /// </summary>
    public class ComplexNumber
    {
        // ------------------------------------------------------------
        // Constructors
        // ------------------------------------------------------------

        public ComplexNumber() { }

        public ComplexNumber(double real, double imaginary)
        {
            Real = real;
            Imaginary = imaginary;
        }

        /// <summary>Create from polar form: r·e^(iθ).</summary>
        public static ComplexNumber FromPolar(double magnitude, double phaseRadians) =>
            new ComplexNumber(
                magnitude * Math.Cos(phaseRadians),
                magnitude * Math.Sin(phaseRadians));

        // ------------------------------------------------------------
        // Properties
        // ------------------------------------------------------------

        /// <summary>Real part (Re).</summary>
        public double Real { get; set; }

        /// <summary>Imaginary part (Im).</summary>
        public double Imaginary { get; set; }

        /// <summary>Magnitude |z| = √(Re² + Im²).</summary>
        public double Magnitude => Math.Sqrt(Real * Real + Imaginary * Imaginary);

        /// <summary>Phase angle θ = atan2(Im, Re) in radians.</summary>
        public double Phase => Math.Atan2(Imaginary, Real);

        /// <summary>True if the imaginary part is effectively zero.</summary>
        public bool IsReal(double tolerance = 1e-15) => Math.Abs(Imaginary) <= tolerance;

        /// <summary>True if the real part is effectively zero.</summary>
        public bool IsImaginary(double tolerance = 1e-15) => Math.Abs(Real) <= tolerance;

        // ------------------------------------------------------------
        // Arithmetic
        // ------------------------------------------------------------

        /// <summary>z₁ + z₂.</summary>
        public static ComplexNumber Add(ComplexNumber a, ComplexNumber b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new ComplexNumber(a.Real + b.Real, a.Imaginary + b.Imaginary);
        }

        /// <summary>z₁ − z₂.</summary>
        public static ComplexNumber Subtract(ComplexNumber a, ComplexNumber b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new ComplexNumber(a.Real - b.Real, a.Imaginary - b.Imaginary);
        }

        /// <summary>z₁ · z₂ = (ac − bd) + (ad + bc)i.</summary>
        public static ComplexNumber Multiply(ComplexNumber a, ComplexNumber b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));
            return new ComplexNumber(
                a.Real * b.Real - a.Imaginary * b.Imaginary,
                a.Real * b.Imaginary + a.Imaginary * b.Real);
        }

        /// <summary>z₁ / z₂.</summary>
        public static ComplexNumber Divide(ComplexNumber a, ComplexNumber b)
        {
            if (a is null) throw new ArgumentNullException(nameof(a));
            if (b is null) throw new ArgumentNullException(nameof(b));

            var denom = b.Real * b.Real + b.Imaginary * b.Imaginary;
            if (denom == 0) throw new DivideByZeroException("Cannot divide by zero complex number.");

            return new ComplexNumber(
                (a.Real * b.Real + a.Imaginary * b.Imaginary) / denom,
                (a.Imaginary * b.Real - a.Real * b.Imaginary) / denom);
        }

        /// <summary>Scalar multiplication: s · z.</summary>
        public ComplexNumber Scale(double scalar) =>
            new ComplexNumber(Real * scalar, Imaginary * scalar);

        // ------------------------------------------------------------
        // Unary operations
        // ------------------------------------------------------------

        /// <summary>Complex conjugate z* = Re − Im·i.</summary>
        public ComplexNumber Conjugate() => new ComplexNumber(Real, -Imaginary);

        /// <summary>Negation −z.</summary>
        public ComplexNumber Negate() => new ComplexNumber(-Real, -Imaginary);

        /// <summary>Reciprocal 1/z.</summary>
        public ComplexNumber Reciprocal()
        {
            var denom = Real * Real + Imaginary * Imaginary;
            if (denom == 0) throw new DivideByZeroException("Cannot invert zero.");
            return new ComplexNumber(Real / denom, -Imaginary / denom);
        }

        // ------------------------------------------------------------
        // Functions
        // ------------------------------------------------------------

        /// <summary>e^z = e^Re · (cos Im + i sin Im).</summary>
        public static ComplexNumber Exp(ComplexNumber z)
        {
            if (z is null) throw new ArgumentNullException(nameof(z));
            var eRe = Math.Exp(z.Real);
            return new ComplexNumber(eRe * Math.Cos(z.Imaginary), eRe * Math.Sin(z.Imaginary));
        }

        /// <summary>√z via polar form.</summary>
        public ComplexNumber Sqrt() => FromPolar(Math.Sqrt(Magnitude), Phase / 2.0);

        /// <summary>zⁿ (integer power) via De Moivre's theorem.</summary>
        public ComplexNumber Power(int n) =>
            FromPolar(Math.Pow(Magnitude, n), Phase * n);

        // ------------------------------------------------------------
        // Operators
        // ------------------------------------------------------------

        public static ComplexNumber operator +(ComplexNumber a, ComplexNumber b) => Add(a, b);
        public static ComplexNumber operator -(ComplexNumber a, ComplexNumber b) => Subtract(a, b);
        public static ComplexNumber operator *(ComplexNumber a, ComplexNumber b) => Multiply(a, b);
        public static ComplexNumber operator /(ComplexNumber a, ComplexNumber b) => Divide(a, b);
        public static ComplexNumber operator -(ComplexNumber a) => a.Negate();

        // ------------------------------------------------------------
        // Equality
        // ------------------------------------------------------------

        public bool Equals(ComplexNumber? other, double tolerance = 1e-15) =>
            other is not null
            && Math.Abs(Real - other.Real) <= tolerance
            && Math.Abs(Imaginary - other.Imaginary) <= tolerance;

        public override bool Equals(object? obj) => obj is ComplexNumber c && Equals(c);

        public override int GetHashCode() => HashCode.Combine(Real, Imaginary);

        public override string ToString()
        {
            if (Imaginary >= 0) return $"{Real} + {Imaginary}i";
            return $"{Real} - {Math.Abs(Imaginary)}i";
        }
    }
}
