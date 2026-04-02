#nullable enable
using System;
using System.Numerics;

namespace MathematicsLibrary
{
    /// <summary>
    /// Numerical Laplace transform utilities.
    /// F(s) = ∫₀∞ f(t)·e^(−st) dt, approximated over [0, MaxTime].
    /// </summary>
    public class LaplaceTransform
    {
        /// <summary>Time-domain function f(t).</summary>
        public Func<double, double>? TimeFunction { get; set; }

        /// <summary>Upper integration limit (approximation of ∞).</summary>
        public double MaxTime { get; set; } = 100.0;

        /// <summary>Number of integration steps.</summary>
        public int Steps { get; set; } = 10_000;

        /// <summary>
        /// Evaluate F(s) at a real-valued s using the trapezoidal rule.
        /// </summary>
        public double Evaluate(double s)
        {
            if (TimeFunction is null)
                throw new InvalidOperationException("TimeFunction must be set before evaluation.");

            var dt = MaxTime / Steps;
            var sum = 0.5 * (TimeFunction(0) + TimeFunction(MaxTime) * Math.Exp(-s * MaxTime));

            for (int i = 1; i < Steps; i++)
            {
                var t = i * dt;
                sum += TimeFunction(t) * Math.Exp(-s * t);
            }

            return sum * dt;
        }

        /// <summary>
        /// Evaluate F(s) at a complex-valued s = σ + jω.
        /// </summary>
        public Complex EvaluateComplex(Complex s)
        {
            if (TimeFunction is null)
                throw new InvalidOperationException("TimeFunction must be set before evaluation.");

            var dt = MaxTime / Steps;
            Complex sum = 0.5 * (TimeFunction(0)
                + TimeFunction(MaxTime) * Complex.Exp(-s * MaxTime));

            for (int i = 1; i < Steps; i++)
            {
                var t = i * dt;
                sum += TimeFunction(t) * Complex.Exp(-s * t);
            }

            return sum * dt;
        }

        /// <summary>
        /// Numerical inverse via the Gaver–Stehfest algorithm (N terms).
        /// Returns f(t) ≈ L⁻¹{F}(t).
        /// </summary>
        public static double InverseStehfest(Func<double, double> F, double t, int n = 8)
        {
            if (F is null) throw new ArgumentNullException(nameof(F));
            if (t <= 0) throw new ArgumentOutOfRangeException(nameof(t), "t must be > 0.");
            if (n % 2 != 0) throw new ArgumentException("n must be even.", nameof(n));

            var ln2OverT = Math.Log(2) / t;
            var sum = 0.0;

            for (int i = 1; i <= n; i++)
            {
                sum += StehfestCoefficient(i, n) * F(i * ln2OverT);
            }

            return sum * ln2OverT;
        }

        private static double StehfestCoefficient(int i, int n)
        {
            var half = n / 2;
            var sum = 0.0;

            var kMin = (i + 1) / 2;
            var kMax = Math.Min(i, half);

            for (int k = kMin; k <= kMax; k++)
            {
                var num = Math.Pow(k, half) * Factorial(2 * k);
                var den = Factorial(half - k) * Factorial(k) * Factorial(k - 1)
                        * Factorial(i - k) * Factorial(2 * k - i);
                sum += num / den;
            }

            return Math.Pow(-1, i + half) * sum;
        }

        private static double Factorial(int n)
        {
            if (n < 0) return 0;
            double result = 1;
            for (int i = 2; i <= n; i++) result *= i;
            return result;
        }
    }
}
