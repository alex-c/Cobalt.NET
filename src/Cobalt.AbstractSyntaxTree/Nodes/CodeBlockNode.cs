﻿using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using System;
using System.Collections.Generic;

namespace Cobalt.AbstractSyntaxTree.Nodes
{
    public class CodeBlockNode : ScopeDefiningAstNode
    {
        public ICollection<StatementNode> Statements { get; }

        public CodeBlockNode(int sourceLine) : base(sourceLine)
        {
            Statements = new List<StatementNode>();
        }

        public void AddStatement(StatementNode statement)
        {
            statement.Parent = this;
            Statements.Add(statement);
        }

        public override string ToString()
        {
            string code = "";
            foreach (StatementNode statement in Statements)
            {
                code += $"  {statement}{Environment.NewLine}";
            }
            return $"{GetType().Name}({Environment.NewLine}{code})";
        }
    }
}
