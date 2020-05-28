using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeNodes
{
    public class BooleanTypeNode : TypeNode
    {
        public BooleanTypeNode(int sourceLine) : base (sourceLine, CobaltType.Boolean) { }
    }
}
