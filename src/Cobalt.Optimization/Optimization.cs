namespace Cobalt.Optimization
{
    /// <summary>
    /// Lists the available optimizations.
    /// </summary>
    public enum Optimization
    {
        /// <summary>
        /// Simplifies negative numbers by coalescing <see cref="Cobalt.AbstractSyntaxTree.Expressions.UnaryExpressions.ArithmeticNegationNode"/> followed by literal values to a literal value with inversed value.
        /// </summary>
        SimplifyNegativeNumbers
    }
}
