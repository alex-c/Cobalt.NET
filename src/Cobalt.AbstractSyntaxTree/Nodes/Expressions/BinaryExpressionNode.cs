namespace Cobalt.AbstractSyntaxTree.Nodes.Expressions
{
    public class BinaryExpressionNode : ExpressionNode
    {
        private AstNode _leftOperand;
        private AstNode _rightOperand;

        public AstNode LeftOperand
        {
            get
            {
                return _leftOperand;
            }
            set
            {
                value.Parent = this;
                _leftOperand = value;
            }
        }

        public AstNode RightOperand
        {
            get
            {
                return _rightOperand;
            }
            set
            {
                value.Parent = this;
                _rightOperand = value;
            }
        }

        public BinaryExpressionNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({LeftOperand},{RightOperand})";
        }
    }
}
