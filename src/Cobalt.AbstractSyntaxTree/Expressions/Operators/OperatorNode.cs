namespace Cobalt.AbstractSyntaxTree.Expressions.Operators
{
    public class OperatorNode : AstNode
    {
        public int Arity { get; }

        public int Precedence { get; }

        public OperatorNode(int sourceLine, int arity, int precedence) : base(sourceLine)
        {
            Arity = arity;
            Precedence = precedence;
        }
    }
}
