using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;

namespace Cobalt.AbstractSyntaxTree.Nodes.Statements
{
    public class VariableDeclarationStatementNode : StatementNode
    {
        private IdentifierNode _identifier;
        private TypeKeywordNode _typeKeyword;
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

        public TypeKeywordNode TypeKeyword
        {
            get
            {
                return _typeKeyword;
            }
            set
            {
                value.Parent = this;
                _typeKeyword = value;
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

        public VariableDeclarationStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            string type = TypeKeyword == null ? " :" : $":{TypeKeyword} ";
            string expr = Expression == null ? "" : $"= {Expression}";
            return $"{GetType().Name}({Identifier}{type}{expr})";
        }
    }
}
