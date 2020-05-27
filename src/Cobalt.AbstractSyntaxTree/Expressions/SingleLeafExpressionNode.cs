namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class SingleLeafExpressionNode : ExpressionNode
    {
        public AstNode Leaf { get; set; }

        public SingleLeafExpressionNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Leaf})";
        }
    }
}
