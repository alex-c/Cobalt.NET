using Cobalt.AbstractSyntaxTree.Expressions;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class StandardOutputStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; set; }

        public StandardOutputStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Expression})";
        }
    }
}
