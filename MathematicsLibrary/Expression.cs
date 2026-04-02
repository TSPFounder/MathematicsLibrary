#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathematics
{
    /// <summary>
    /// Mathematical expression composed of constants, coefficients, variables, operators,
    /// and nested sub-expressions. Supports numeric evaluation via variable binding.
    /// </summary>
    public class Expression
    {
        // ------------------------------------------------------------
        // Constructors
        // ------------------------------------------------------------

        public Expression() { }

        /// <summary>Create an expression that wraps a single constant value.</summary>
        public Expression(double constantValue) => Constants.Add(constantValue);

        // ------------------------------------------------------------
        // Properties
        // ------------------------------------------------------------

        /// <summary>Display name.</summary>
        public string? Name { get; set; }

        /// <summary>Literal constant values in this expression.</summary>
        public List<double> Constants { get; set; } = [];

        /// <summary>Numeric coefficients (multipliers of variables/sub-expressions).</summary>
        public List<double> Coefficients { get; set; } = [];

        /// <summary>Operators that combine terms.</summary>
        public List<Operator> Operators { get; set; } = [];

        /// <summary>Named parameters (fixed once set, e.g. π, g).</summary>
        public List<Parameter> Parameters { get; set; } = [];

        /// <summary>Named variables (values supplied at evaluation time).</summary>
        public List<Variable> Variables { get; set; } = [];

        /// <summary>Nested sub-expressions.</summary>
        public List<Expression> SubExpressions { get; set; } = [];

        /// <summary>Number of terms (constants + variables + sub-expressions).</summary>
        public int TermCount => Constants.Count + Variables.Count + SubExpressions.Count;

        // ------------------------------------------------------------
        // Factory helpers
        // ------------------------------------------------------------

        /// <summary>Build a simple binary expression: left op right.</summary>
        public static Expression Binary(Expression left, Operator op, Expression right)
        {
            if (left is null) throw new ArgumentNullException(nameof(left));
            if (op is null) throw new ArgumentNullException(nameof(op));
            if (right is null) throw new ArgumentNullException(nameof(right));

            var expr = new Expression();
            expr.SubExpressions.Add(left);
            expr.SubExpressions.Add(right);
            expr.Operators.Add(op);
            return expr;
        }

        /// <summary>Build a unary expression: op(operand).</summary>
        public static Expression Unary(Operator op, Expression operand)
        {
            if (op is null) throw new ArgumentNullException(nameof(op));
            if (operand is null) throw new ArgumentNullException(nameof(operand));

            var expr = new Expression();
            expr.SubExpressions.Add(operand);
            expr.Operators.Add(op);
            return expr;
        }

        // ------------------------------------------------------------
        // Evaluation
        // ------------------------------------------------------------

        /// <summary>
        /// Evaluate the expression using supplied variable values.
        /// Keys are variable names; values are their numeric values.
        /// </summary>
        public double Evaluate(Dictionary<string, double>? variableValues = null)
        {
            variableValues ??= [];

            // Single constant
            if (Constants.Count == 1 && Variables.Count == 0 && SubExpressions.Count == 0)
                return Constants[0];

            // Collect term values in order
            var terms = new List<double>();

            foreach (var c in Constants)
                terms.Add(c);

            foreach (var v in Variables)
            {
                if (variableValues.TryGetValue(v.Name ?? "", out var val))
                    terms.Add(val * v.Coefficient);
                else
                    throw new InvalidOperationException($"No value supplied for variable '{v.Name}'.");
            }

            foreach (var sub in SubExpressions)
                terms.Add(sub.Evaluate(variableValues));

            if (terms.Count == 0) return 0.0;

            // Apply unary operator if single term
            if (terms.Count == 1 && Operators.Count == 1 && Operators[0].IsUnary)
                return Operators[0].Apply(terms[0]);

            // Fold left-to-right with binary operators
            var result = terms[0];
            for (int i = 0; i < Operators.Count && i + 1 < terms.Count; i++)
                result = Operators[i].Apply(result, terms[i + 1]);

            return result;
        }

        /// <summary>
        /// Substitute a variable with a constant value and return a simplified copy.
        /// </summary>
        public Expression Substitute(string variableName, double value)
        {
            var copy = ShallowCopy();

            var matched = copy.Variables.Where(v => v.Name == variableName).ToList();
            foreach (var v in matched)
            {
                copy.Constants.Add(value * v.Coefficient);
                copy.Variables.Remove(v);
            }

            for (int i = 0; i < copy.SubExpressions.Count; i++)
                copy.SubExpressions[i] = copy.SubExpressions[i].Substitute(variableName, value);

            return copy;
        }

        /// <summary>Collect all unique variable names in this expression tree.</summary>
        public HashSet<string> GetVariableNames()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var v in Variables)
            {
                if (v.Name is not null) names.Add(v.Name);
            }
            foreach (var sub in SubExpressions)
                names.UnionWith(sub.GetVariableNames());
            return names;
        }

        public override string ToString()
        {
            if (Constants.Count == 1 && Variables.Count == 0 && SubExpressions.Count == 0)
                return Constants[0].ToString("G");

            var parts = new List<string>();
            foreach (var c in Constants) parts.Add(c.ToString("G"));
            foreach (var v in Variables) parts.Add(v.ToString());
            foreach (var sub in SubExpressions) parts.Add($"({sub})");

            if (Operators.Count == 0) return string.Join(", ", parts);

            // Interleave terms and operators
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < parts.Count; i++)
            {
                if (i > 0 && i - 1 < Operators.Count)
                    sb.Append($" {Operators[i - 1]} ");
                sb.Append(parts[i]);
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------

        private Expression ShallowCopy() => new Expression
        {
            Name = Name,
            Constants = new List<double>(Constants),
            Coefficients = new List<double>(Coefficients),
            Operators = new List<Operator>(Operators),
            Parameters = new List<Parameter>(Parameters),
            Variables = new List<Variable>(Variables),
            SubExpressions = new List<Expression>(SubExpressions)
        };
    }
}
