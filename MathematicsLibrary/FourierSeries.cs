#nullable enable
using System;
using System.Collections.Generic;

namespace MathematicsLibrary
{
    /// <summary>
    /// Fourier series representation of a periodic function f(x) with period P.
    /// f(x) ≈ a₀/2 + Σ [aₙ cos(2πnx/P) + bₙ sin(2πnx/P)]
    /// </summary>
    public class FourierSeries
    {
        /// <summary>The periodic function f(x).</summary>
        public Func<double, double>? Function { get; set; }

        /// <summary>Period of the function.</summary>
        public double Period { get; set; } = 2.0 * Math.PI;

        /// <summary>Number of harmonics to compute (n = 1 … N).</summary>
        public int Harmonics { get; set; } = 10;

        /// <summary>Number of integration steps per coefficient.</summary>
        public int IntegrationSteps { get; set; } = 1_000;

        /// <summary>Computed cosine coefficients aₙ (index 0 = a₀).</summary>
        public List<double> A_Coefficients { get; private set; } = [];

        /// <summary>Computed sine coefficients bₙ (index 0 = b₀ = 0).</summary>
        public List<double> B_Coefficients { get; private set; } = [];

        /// <summary>
        /// Compute all Fourier coefficients a₀…aN and b₁…bN via trapezoidal integration.
        /// </summary>
        public void ComputeCoefficients()
        {
            if (Function is null)
                throw new InvalidOperationException("Function must be set before computing coefficients.");

            var P = Period;
            var N = Harmonics;
            var steps = IntegrationSteps;
            var dx = P / steps;

            A_Coefficients = new List<double>(N + 1);
            B_Coefficients = new List<double>(N + 1);

            for (int n = 0; n <= N; n++)
            {
                double aSum = 0, bSum = 0;
                var omega = 2.0 * Math.PI * n / P;

                for (int i = 0; i <= steps; i++)
                {
                    var x = i * dx;
                    var fx = Function(x);
                    var weight = (i == 0 || i == steps) ? 0.5 : 1.0;

                    aSum += weight * fx * Math.Cos(omega * x);
                    bSum += weight * fx * Math.Sin(omega * x);
                }

                A_Coefficients.Add(2.0 / P * aSum * dx);
                B_Coefficients.Add(2.0 / P * bSum * dx);
            }
        }

        /// <summary>
        /// Evaluate the Fourier series approximation at a given x using stored coefficients.
        /// </summary>
        public double Evaluate(double x)
        {
            if (A_Coefficients.Count == 0)
                throw new InvalidOperationException("Call ComputeCoefficients first.");

            var P = Period;
            var result = A_Coefficients[0] / 2.0;

            for (int n = 1; n < A_Coefficients.Count; n++)
            {
                var omega = 2.0 * Math.PI * n / P;
                result += A_Coefficients[n] * Math.Cos(omega * x)
                        + B_Coefficients[n] * Math.Sin(omega * x);
            }

            return result;
        }

        /// <summary>
        /// Evaluate a partial sum using only the first <paramref name="terms"/> harmonics.
        /// </summary>
        public double EvaluatePartialSum(double x, int terms)
        {
            if (A_Coefficients.Count == 0)
                throw new InvalidOperationException("Call ComputeCoefficients first.");

            var limit = Math.Min(terms, A_Coefficients.Count - 1);
            var P = Period;
            var result = A_Coefficients[0] / 2.0;

            for (int n = 1; n <= limit; n++)
            {
                var omega = 2.0 * Math.PI * n / P;
                result += A_Coefficients[n] * Math.Cos(omega * x)
                        + B_Coefficients[n] * Math.Sin(omega * x);
            }

            return result;
        }
    }
}
