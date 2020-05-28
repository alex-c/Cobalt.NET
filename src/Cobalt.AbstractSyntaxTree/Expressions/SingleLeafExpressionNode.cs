namespace Cobalt.AbstractSyntaxTree.Expressions
{
    public class SingleLeafExpressionNode : ExpressionNode
    {
        private AstNode _leaf;

        public AstNode Leaf
        {
            get
            {
                return _leaf;
            }
            set
            {
                value.Parent = this;
                _leaf = value;
            }
        }

        public SingleLeafExpressionNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Leaf})";
        }
    }
}
