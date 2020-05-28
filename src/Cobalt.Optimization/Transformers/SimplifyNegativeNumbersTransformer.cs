using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Expressions.UnaryExpressions;

namespace Cobalt.Optimization.Transformers
{
    public class SimplifyNegativeNumbersTransformer : IAstTransformer<ArithmeticNegationNode>
    {
        public AstNode Transform(ArithmeticNegationNode ast)
        {
            // TODO: implement transformation
            return ast;
        }
    }
}
