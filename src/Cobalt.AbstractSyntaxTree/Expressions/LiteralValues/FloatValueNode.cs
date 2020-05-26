namespace Cobalt.AbstractSyntaxTree.Expressions.LiteralValues
{
    public class FloatValueNode : LiteralValueNode
    {
        public new float Value { get; }

        public FloatValueNode(int sourceLine, float value) : base(sourceLine)
        {
            Value = value;
        }
    }
}
