using Cobalt.AbstractSyntaxTree.Leafs;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class StandardInputStatementNode : StatementNode
    {
        public IdentifierNode Identifier { get; }

        public StandardInputStatementNode(int sourceLine, IdentifierNode identifier) : base(sourceLine)
        {
            Identifier = identifier;
        }
    }
}
