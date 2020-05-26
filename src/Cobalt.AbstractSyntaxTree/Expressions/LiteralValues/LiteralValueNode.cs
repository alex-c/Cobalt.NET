namespace Cobalt.AbstractSyntaxTree.Expressions.LiteralValues
{
    public class LiteralValueNode : AstNode
    {
        public object Value { get; }

        public LiteralValueNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
