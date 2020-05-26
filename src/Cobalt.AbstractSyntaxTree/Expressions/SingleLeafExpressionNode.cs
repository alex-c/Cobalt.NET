namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class SingleLeafExpressionNode : ExpressionNode
    {
        public AstNode Leaf { get; set; }

        public SingleLeafExpressionNode(int sourceLine) : base(sourceLine) { }
    }
}
