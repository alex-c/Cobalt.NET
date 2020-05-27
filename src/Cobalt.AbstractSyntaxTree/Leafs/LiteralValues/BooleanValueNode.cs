namespace Cobalt.AbstractSyntaxTree.Leafs.LiteralValues
{
    public class BooleanValueNode : LiteralValueNode
    {
        public bool Value { get; }

        public BooleanValueNode(int sourceLine, bool value) : base(sourceLine)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
