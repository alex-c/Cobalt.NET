namespace Cobalt.AbstractSyntaxTree.Expressions.LiteralValues
{
    public class IntegerValueNode : LiteralValueNode
    {
        public new int Value { get; }

        public IntegerValueNode(int sourceLine, int value) : base(sourceLine)
        {
            Value = value;
        }
    }
}
