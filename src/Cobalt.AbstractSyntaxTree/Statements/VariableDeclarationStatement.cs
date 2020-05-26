using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Leafs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cobalt.AbstractSyntaxTree.Statements
{
    public class VariableDeclarationStatement : StatementNode
    {
        public IdentifierNode Identifier { get; set; }

        public TypeNode Type { get; set; }

        public ExpressionNode Expression { get; set; }

        public VariableDeclarationStatement(int sourceLine) : base(sourceLine) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Identifier} : {Type} = {Expression})";
        }
    }
}
