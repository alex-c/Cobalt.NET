using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;

namespace Cobalt.AbstractSyntaxTree.Nodes.Statements
{
    public class VariableDeclarationStatementNode : StatementNode
    {
        private IdentifierNode _identifier;
        private TypeNode _type;
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

        public TypeNode Type
        {
            get
            {
                return _type;
            }
            set
            {
                value.Parent = this;
                _type = value;
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
            string type = Type == null ? " :" : $":{Type} ";
            string expr = Expression == null ? "" : $"= {Expression}";
            return $"{GetType().Name}({Identifier}{type}{expr})";
        }
    }
}
