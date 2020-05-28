using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs
{
    public class TypeNode : AstNode
    {
        public CobaltType Type { get; }

        public TypeNode(int sourceLine, CobaltType type) : base(sourceLine)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
