namespace Cobalt.AbstractSyntaxTree.Leafs.LiteralValues
{
    public class FloatValueNode : LiteralValueNode
    {
        public float Value { get; }

        public FloatValueNode(int sourceLine, float value) : base(sourceLine)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
