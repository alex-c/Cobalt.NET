using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class IntegerValueNode : LiteralValueNode<int>
    {
        public IntegerValueNode(int sourceLine, int value) : base(sourceLine, CobaltType.Integer, value) { }
    }
}
