using Cobalt.AbstractSyntaxTree.Statements;
using System.Collections.Generic;

namespace Cobalt.AbstractSyntaxTree
{
    public class CodeBlockNode : ScopeDefiningAstNode
    {
        public ICollection<StatementNode> Statements { get; }

        public CodeBlockNode(int sourceLine) : base(sourceLine)
        {
            Statements = new List<StatementNode>();
        }
    }
}
