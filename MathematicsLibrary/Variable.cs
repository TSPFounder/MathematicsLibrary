#nullable enable

namespace Mathematics
{
    /// <summary>
    /// Named variable with an optional coefficient (e.g. 3x ? Coefficient=3, Name="x").
    /// </summary>
    public class Variable
    {
        public Variable() { }

        public Variable(string name, double coefficient = 1.0)
        {
            Name = name;
            Coefficient = coefficient;
        }

        /// <summary>Variable name (e.g. "x", "t").</summary>
        public string? Name { get; set; }

        /// <summary>Multiplier applied to this variable's value.</summary>
        public double Coefficient { get; set; } = 1.0;

        /// <summary>Optional exponent (e.g. x² ? Exponent=2).</summary>
        public double Exponent { get; set; } = 1.0;

        public override string ToString()
        {
            var coeff = Coefficient == 1.0 ? "" : $"{Coefficient:G}·";
            var exp = Exponent == 1.0 ? "" : $"^{Exponent:G}";
            return $"{coeff}{Name ?? "?"}{exp}";
        }
    }
}