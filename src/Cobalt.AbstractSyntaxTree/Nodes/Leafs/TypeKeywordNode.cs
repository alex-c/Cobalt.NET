using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs
{
    public class TypeKeywordNode : AstNode
    {
        public CobaltType Type { get; }

        public TypeKeywordNode(int sourceLine, CobaltType type) : base(sourceLine)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
