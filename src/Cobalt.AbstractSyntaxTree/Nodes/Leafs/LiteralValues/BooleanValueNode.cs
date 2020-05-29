using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class BooleanValueNode : LiteralValueNode<bool>
    {
        public BooleanValueNode(int sourceLine, bool value) : base(sourceLine, CobaltType.Boolean, value) { }
    }
}
