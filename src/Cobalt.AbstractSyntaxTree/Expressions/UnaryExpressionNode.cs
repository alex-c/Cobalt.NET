namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class UnaryExpressionNode : ExpressionNode
    {
        private AstNode _operand;

        public AstNode Operand
        {
            get
            {
                return _operand;
            }
            set
            {
                value.Parent = this;
                _operand = value;
            }
        }

        public UnaryExpressionNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Operand})";
        }
    }
}
