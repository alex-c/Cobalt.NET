using Cobalt.AbstractSyntaxTree.Nodes.Leafs;

namespace Cobalt.AbstractSyntaxTree.Nodes.Statements
{
    public class StandardInputStatementNode : StatementNode
    {
        private IdentifierNode _identifier;

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

        public StandardInputStatementNode(int sourceLine) : base(sourceLine) { }


        public override string ToString()
        {
            return $"{GetType().Name}(`{Identifier.IdentifierName}`)";
        }
    }
}
