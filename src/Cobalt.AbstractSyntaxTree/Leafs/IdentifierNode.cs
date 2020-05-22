namespace Cobalt.AbstractSyntaxTree.Leafs
{
    public class IdentifierNode : AstNode
    {
        public string IdentifierName { get; }

        public IdentifierNode(int sourceLine, string identifierName) : base(sourceLine)
        {
            IdentifierName = identifierName;
        }

        public override string ToString()
        {
            return $"{GetType().Name}(`{IdentifierName}`)";
        }
    }
}
