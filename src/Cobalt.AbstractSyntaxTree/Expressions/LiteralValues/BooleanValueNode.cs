namespace Cobalt.AbstractSyntaxTree.Expressions.LiteralValues
{
    public class BooleanValueNode : LiteralValueNode
    {
        public new bool Value { get; }

        public BooleanValueNode(int sourceLine, bool value) : base(sourceLine)
        {
            Value = value;
        }
    }
}
