using Cobalt.AbstractSyntaxTree.Nodes.Expressions;

namespace Cobalt.AbstractSyntaxTree.Nodes.Statements
{
    public class StandardOutputStatementNode : StatementNode
    {
        private ExpressionNode _expression;

        public ExpressionNode Expression {
            get
            {
                return _expression;
            }
            set
            {
                value.Parent = this;
                _expression = value;
            }
        }

        public StandardOutputStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Expression})";
        }
    }
}
