namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class BinaryExpressionNode : ExpressionNode
    {
        public BinaryExpressionNode(int sourceLine) : base(sourceLine) { }

        public AstNode LeftOperand { get; set; }

        public AstNode RightOperand { get; set; }
    }
}
