using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Leafs;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class VariableAssignmentStatementNode : StatementNode
    {
        public IdentifierNode Identifier { get; set; }

        public ExpressionNode Expression { get; set; }

        public VariableAssignmentStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Identifier} := {Expression})";
        }
    }
}
