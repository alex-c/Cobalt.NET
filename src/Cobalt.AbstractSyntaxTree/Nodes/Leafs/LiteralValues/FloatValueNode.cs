using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class FloatValueNode : LiteralValueNode
    {
        public float Value { get; }

        public FloatValueNode(int sourceLine, float value) : base(sourceLine, CobaltType.Float)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
