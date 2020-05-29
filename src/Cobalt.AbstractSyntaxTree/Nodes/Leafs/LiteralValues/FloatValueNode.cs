using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class FloatValueNode : LiteralValueNode<float>
    {
        public FloatValueNode(int sourceLine, float value) : base(sourceLine, CobaltType.Float, value) { }
    }
}
