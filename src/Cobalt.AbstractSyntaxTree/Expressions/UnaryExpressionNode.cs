namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class UnaryExpressionNode : ExpressionNode
    {
        public AstNode Operand { get; set; }

        public UnaryExpressionNode(int sourceLine) : base(sourceLine) { }
    }
}
