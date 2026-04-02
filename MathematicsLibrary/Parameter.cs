#nullable enable

namespace Mathematics
{
    /// <summary>
    /// Named constant parameter (e.g. ?, g, ?) with a fixed numeric value.
    /// </summary>
    public class Parameter
    {
        public Parameter() { }

        public Parameter(string name, double value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>Parameter name.</summary>
        public string? Name { get; set; }

        /// <summary>Fixed numeric value.</summary>
        public double Value { get; set; }

        /// <summary>Optional description or unit.</summary>
        public string? Description { get; set; }

        public override string ToString() => $"{Name ?? "?"}={Value:G}";
    }
}