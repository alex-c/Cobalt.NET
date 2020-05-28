using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeNodes
{
    public class IntegerTypeNode : TypeNode
    {
        public IntegerTypeNode(int sourceLine) : base (sourceLine, CobaltType.Integer) { }
    }
}
