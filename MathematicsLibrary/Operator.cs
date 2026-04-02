#nullable enable
using System;

namespace Mathematics
{
    /// <summary>
    /// Mathematical operator with a type, symbol, and the ability to apply itself to operands.
    /// </summary>
    public class Operator
    {
        // ------------------------------------------------------------
        // Enum
        // ------------------------------------------------------------

        public enum OperatorTypeEnum
        {
            Plus = 0,
            Minus,
            Multiply,
            Divide,
            Modulus,
            Exponentiate,
            EulerExponent,
            LogBaseTen,
            NaturalLog,
            SquareRoot,
            Absolute,
            Negate,
            Sine,
            Cosine,
            Tangent,
            DotProduct,
            CrossProduct,
            TensorProduct,
            InvertMatrix,
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        }

        // ------------------------------------------------------------
        // Constructors
        // ------------------------------------------------------------

        public Operator() { }

        public Operator(OperatorTypeEnum type)
        {
            OperatorType = type;
            Symbol = GetDefaultSymbol(type);
        }

        // ------------------------------------------------------------
        // Properties
        // ------------------------------------------------------------

        /// <summary>Display name.</summary>
        public string? Name { get; set; }

        /// <summary>Kind of operator.</summary>
        public OperatorTypeEnum OperatorType { get; set; }

        /// <summary>Textual symbol (e.g. "+", "×", "sin").</summary>
        public string? Symbol { get; set; }

        /// <summary>Operator precedence (higher binds tighter).</summary>
        public int Precedence => GetPrecedence(OperatorType);

        /// <summary>True if the operator takes one operand (sin, cos, √, etc.).</summary>
        public bool IsUnary => OperatorType is
            OperatorTypeEnum.EulerExponent or OperatorTypeEnum.LogBaseTen or
            OperatorTypeEnum.NaturalLog or OperatorTypeEnum.SquareRoot or
            OperatorTypeEnum.Absolute or OperatorTypeEnum.Negate or
            OperatorTypeEnum.Sine or OperatorTypeEnum.Cosine or
            OperatorTypeEnum.Tangent;

        // ------------------------------------------------------------
        // Methods
        // ------------------------------------------------------------

        /// <summary>Apply a binary operator to two operands.</summary>
        public double Apply(double left, double right) => OperatorType switch
        {
            OperatorTypeEnum.Plus => left + right,
            OperatorTypeEnum.Minus => left - right,
            OperatorTypeEnum.Multiply => left * right,
            OperatorTypeEnum.Divide when right != 0 => left / right,
            OperatorTypeEnum.Divide => throw new DivideByZeroException(),
            OperatorTypeEnum.Modulus when right != 0 => left % right,
            OperatorTypeEnum.Modulus => throw new DivideByZeroException(),
            OperatorTypeEnum.Exponentiate => Math.Pow(left, right),
            _ => throw new InvalidOperationException($"Operator '{OperatorType}' is not a binary arithmetic operator.")
        };

        /// <summary>Apply a unary operator to a single operand.</summary>
        public double Apply(double operand) => OperatorType switch
        {
            OperatorTypeEnum.Negate => -operand,
            OperatorTypeEnum.Absolute => Math.Abs(operand),
            OperatorTypeEnum.SquareRoot => Math.Sqrt(operand),
            OperatorTypeEnum.EulerExponent => Math.Exp(operand),
            OperatorTypeEnum.NaturalLog => Math.Log(operand),
            OperatorTypeEnum.LogBaseTen => Math.Log10(operand),
            OperatorTypeEnum.Sine => Math.Sin(operand),
            OperatorTypeEnum.Cosine => Math.Cos(operand),
            OperatorTypeEnum.Tangent => Math.Tan(operand),
            _ => throw new InvalidOperationException($"Operator '{OperatorType}' is not a unary operator.")
        };

        /// <summary>Evaluate a comparison operator, returning true/false as 1.0/0.0.</summary>
        public bool Compare(double left, double right) => OperatorType switch
        {
            OperatorTypeEnum.Equal => Math.Abs(left - right) < 1e-15,
            OperatorTypeEnum.NotEqual => Math.Abs(left - right) >= 1e-15,
            OperatorTypeEnum.LessThan => left < right,
            OperatorTypeEnum.LessThanOrEqual => left <= right,
            OperatorTypeEnum.GreaterThan => left > right,
            OperatorTypeEnum.GreaterThanOrEqual => left >= right,
            _ => throw new InvalidOperationException($"Operator '{OperatorType}' is not a comparison operator.")
        };

        public override string ToString() => Symbol ?? OperatorType.ToString();

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------

        private static string GetDefaultSymbol(OperatorTypeEnum type) => type switch
        {
            OperatorTypeEnum.Plus => "+",
            OperatorTypeEnum.Minus => "−",
            OperatorTypeEnum.Multiply => "×",
            OperatorTypeEnum.Divide => "÷",
            OperatorTypeEnum.Modulus => "%",
            OperatorTypeEnum.Exponentiate => "^",
            OperatorTypeEnum.EulerExponent => "exp",
            OperatorTypeEnum.LogBaseTen => "log₁₀",
            OperatorTypeEnum.NaturalLog => "ln",
            OperatorTypeEnum.SquareRoot => "√",
            OperatorTypeEnum.Absolute => "|·|",
            OperatorTypeEnum.Negate => "−",
            OperatorTypeEnum.Sine => "sin",
            OperatorTypeEnum.Cosine => "cos",
            OperatorTypeEnum.Tangent => "tan",
            OperatorTypeEnum.Equal => "=",
            OperatorTypeEnum.NotEqual => "≠",
            OperatorTypeEnum.LessThan => "<",
            OperatorTypeEnum.LessThanOrEqual => "≤",
            OperatorTypeEnum.GreaterThan => ">",
            OperatorTypeEnum.GreaterThanOrEqual => "≥",
            OperatorTypeEnum.DotProduct => "·",
            OperatorTypeEnum.CrossProduct => "×",
            OperatorTypeEnum.TensorProduct => "⊗",
            OperatorTypeEnum.InvertMatrix => "⁻¹",
            _ => "?"
        };

        private static int GetPrecedence(OperatorTypeEnum type) => type switch
        {
            OperatorTypeEnum.Plus or OperatorTypeEnum.Minus => 1,
            OperatorTypeEnum.Multiply or OperatorTypeEnum.Divide or OperatorTypeEnum.Modulus => 2,
            OperatorTypeEnum.Exponentiate => 3,
            _ => 0
        };
    }
}
