namespace Cobalt.AbstractSyntaxTree.Leafs
{
    public class IdentifierNode : AstNode
    {
        public string Identifier { get; }

        public IdentifierNode(int sourceLine, string identifier) : base(sourceLine)
        {
            Identifier = identifier;
        }

        public override string ToString()
        {
            return $"{GetType().Name}(`{Identifier}`)";
        }
    }
}
