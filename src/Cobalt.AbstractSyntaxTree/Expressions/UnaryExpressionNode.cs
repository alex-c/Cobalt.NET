namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class UnaryExpressionNode : ExpressionNode
    {
        public UnaryExpressionNode(int sourceLine) : base(sourceLine) { }

        public AstNode Operand { get; set; }
    }
}
