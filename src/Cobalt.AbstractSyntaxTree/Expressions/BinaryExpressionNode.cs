namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class BinaryExpressionNode : ExpressionNode
    {
        public AstNode LeftOperand { get; set; }

        public AstNode RightOperand { get; set; }

        public BinaryExpressionNode(int sourceLine) : base(sourceLine) { }
    }
}
