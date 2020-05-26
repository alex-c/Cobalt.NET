using Cobalt.AbstractSyntaxTree.Leafs;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class StandardInputStatementNode : StatementNode
    {
        public IdentifierNode Identifier { get; set; }

        public StandardInputStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}(`{Identifier.IdentifierName}`)";
        }
    }
}
