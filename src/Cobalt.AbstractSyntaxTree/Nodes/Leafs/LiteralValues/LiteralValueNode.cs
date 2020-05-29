using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class LiteralValueNode<T> : AstNode
    {
        public CobaltType Type { get; }

        public T Value { get; }

        public LiteralValueNode(int sourceLine, CobaltType type, T value) : base(sourceLine)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
