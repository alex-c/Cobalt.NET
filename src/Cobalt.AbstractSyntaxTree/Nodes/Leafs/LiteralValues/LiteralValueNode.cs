using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class LiteralValueNode : AstNode
    {
        public CobaltType Type { get; }

        public LiteralValueNode(int sourceLine, CobaltType type) : base(sourceLine)
        {
            Type = type;
        }
    }
}
