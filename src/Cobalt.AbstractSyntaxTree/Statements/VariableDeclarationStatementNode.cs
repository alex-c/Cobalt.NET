using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Leafs;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class VariableDeclarationStatementNode : StatementNode
    {
        public IdentifierNode Identifier { get; set; }

        public TypeNode Type { get; set; }

        public ExpressionNode Expression { get; set; }

        public VariableDeclarationStatementNode(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            string type = Type == null ? " :" : $":{Type} ";
            string expr = Expression == null ? "" : $"= {Expression}";
            return $"{GetType().Name}({Identifier}{type}{expr})";
        }
    }
}
