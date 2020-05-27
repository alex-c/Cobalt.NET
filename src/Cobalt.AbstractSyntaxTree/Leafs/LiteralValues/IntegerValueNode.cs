namespace Cobalt.AbstractSyntaxTree.Leafs.LiteralValues
{
    public class IntegerValueNode : LiteralValueNode
    {
        public int Value { get; }

        public IntegerValueNode(int sourceLine, int value) : base(sourceLine)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
