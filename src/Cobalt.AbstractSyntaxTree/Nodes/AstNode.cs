namespace Cobalt.AbstractSyntaxTree.Nodes
{
    public class AstNode
    {
        public int SourceLine { get; }

        public bool DefinesScope { get; }

        public AstNode Parent { get; set; }

        public AstNode(int sourceLine, bool definesScope = false)
        {
            SourceLine = sourceLine;
            DefinesScope = definesScope;
            Parent = null;
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
