#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathematics
{
    /// <summary>
    /// Equation: LHS (operator) RHS, supporting equality and inequality relations,
    /// numeric evaluation, and simple single-variable solving via bisection.
    /// </summary>
    public class Equation
    {
        // ------------------------------------------------------------
        // Constructors
        // ------------------------------------------------------------

        public Equation() { }

        public Equation(Expression lhs, Expression rhs, Operator? equalityOperator = null)
        {
            LHS = lhs ?? throw new ArgumentNullException(nameof(lhs));
            RHS = rhs ?? throw new ArgumentNullException(nameof(rhs));
            EqualityOperator = equalityOperator ?? new Operator(Operator.OperatorTypeEnum.Equal);
        }

        // ------------------------------------------------------------
        // Properties
        // ------------------------------------------------------------

        /// <summary>Display name.</summary>
        public string? Name { get; set; }

        /// <summary>Left-hand side expression.</summary>
        public Expression? LHS { get; set; }

        /// <summary>Right-hand side expression.</summary>
        public Expression? RHS { get; set; }

        /// <summary>Relational operator (=, ≠, &lt;, ≤, >, ≥).</summary>
        public Operator EqualityOperator { get; set; } = new Operator(Operator.OperatorTypeEnum.Equal);

        /// <summary>True when the relational operator is not strict equality.</summary>
        public bool IsInequality => EqualityOperator.OperatorType != Operator.OperatorTypeEnum.Equal;

        /// <summary>All operators used across both sides.</summary>
        public List<Operator> Operators { get; set; } = [];

        // ------------------------------------------------------------
        // Evaluation
        // ------------------------------------------------------------

        /// <summary>Evaluate LHS − RHS (the residual). Zero means the equation is satisfied.</summary>
        public double Residual(Dictionary<string, double>? variableValues = null)
        {
            EnsureSides();
            return LHS!.Evaluate(variableValues) - RHS!.Evaluate(variableValues);
        }

        /// <summary>Check whether the relation holds for the given variable values.</summary>
        public bool IsSatisfied(Dictionary<string, double>? variableValues = null)
        {
            EnsureSides();
            var left = LHS!.Evaluate(variableValues);
            var right = RHS!.Evaluate(variableValues);
            return EqualityOperator.Compare(left, right);
        }

        /// <summary>Evaluate LHS for given variable values.</summary>
        public double EvaluateLHS(Dictionary<string, double>? variableValues = null)
        {
            if (LHS is null) throw new InvalidOperationException("LHS is not defined.");
            return LHS.Evaluate(variableValues);
        }

        /// <summary>Evaluate RHS for given variable values.</summary>
        public double EvaluateRHS(Dictionary<string, double>? variableValues = null)
        {
            if (RHS is null) throw new InvalidOperationException("RHS is not defined.");
            return RHS.Evaluate(variableValues);
        }

        // ------------------------------------------------------------
        // Solving
        // ------------------------------------------------------------

        /// <summary>
        /// Solve for a single variable using bisection on LHS − RHS = 0.
        /// All other variables must be supplied in <paramref name="knownValues"/>.
        /// </summary>
        public double SolveBisection(
            string variableName,
            double lowerBound,
            double upperBound,
            Dictionary<string, double>? knownValues = null,
            double tolerance = 1e-10,
            int maxIterations = 1000)
        {
            EnsureSides();
            knownValues ??= [];

            double F(double x)
            {
                var vals = new Dictionary<string, double>(knownValues) { [variableName] = x };
                return Residual(vals);
            }

            var fLow = F(lowerBound);
            var fHigh = F(upperBound);

            if (Math.Sign(fLow) == Math.Sign(fHigh))
                throw new ArgumentException("Function must have opposite signs at the bounds.");

            double mid = lowerBound;
            for (int i = 0; i < maxIterations; i++)
            {
                mid = (lowerBound + upperBound) / 2.0;
                var fMid = F(mid);

                if (Math.Abs(fMid) < tolerance || (upperBound - lowerBound) / 2.0 < tolerance)
                    return mid;

                if (Math.Sign(fMid) == Math.Sign(fLow))
                {
                    lowerBound = mid;
                    fLow = fMid;
                }
                else
                {
                    upperBound = mid;
                }
            }

            return mid;
        }

        // ------------------------------------------------------------
        // Utilities
        // ------------------------------------------------------------

        /// <summary>Collect all unique variable names from both sides.</summary>
        public HashSet<string> GetAllVariableNames()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            if (LHS is not null) names.UnionWith(LHS.GetVariableNames());
            if (RHS is not null) names.UnionWith(RHS.GetVariableNames());
            return names;
        }

        /// <summary>Swap LHS and RHS.</summary>
        public void Transpose() => (LHS, RHS) = (RHS, LHS);

        public override string ToString()
            => $"{LHS} {EqualityOperator} {RHS}";

        private void EnsureSides()
        {
            if (LHS is null) throw new InvalidOperationException("LHS is not defined.");
            if (RHS is null) throw new InvalidOperationException("RHS is not defined.");
        }
    }
}
