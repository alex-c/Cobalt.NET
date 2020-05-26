namespace Cobalt.AbstractSyntaxTree.Leafs
{
    public class TypeNode : AstNode
    {
        public TypeNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
