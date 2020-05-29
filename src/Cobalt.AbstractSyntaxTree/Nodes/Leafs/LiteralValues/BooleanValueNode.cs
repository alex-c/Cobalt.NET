using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class BooleanValueNode : LiteralValueNode
    {
        public bool Value { get; }

        public BooleanValueNode(int sourceLine, bool value) : base(sourceLine, CobaltType.Boolean)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
