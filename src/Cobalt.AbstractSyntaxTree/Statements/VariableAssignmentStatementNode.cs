using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Leafs;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class VariableAssignmentStatementNode : StatementNode
    {
        private IdentifierNode _identifier;
        private ExpressionNode _expression;

        public IdentifierNode Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                value.Parent = this;
                _identifier = value;
            }
        }

        public ExpressionNode Expression
        {
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

        public VariableAssignmentStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Identifier} := {Expression})";
        }
    }
}
