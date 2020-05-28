using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Statements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Cobalt.SemanticAnalysis
{
    public class CobaltSemanticAnalyzer
    {
        private ILogger Logger { get; }

        public CobaltSemanticAnalyzer(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CobaltSemanticAnalyzer>();
        }

        public void Analyze(CobaltProgram program)
        {
            ICollection<StatementNode> programStatements = program.Code.Statements;
            foreach (StatementNode statement in programStatements)
            {
                // TODO
            }
        }

        private void RegisterSymbol(AstNode node, Symbol symbol)
        {
            if (node is ScopeDefiningAstNode scopeDefiningAstNode)
            {
                scopeDefiningAstNode.SymbolTable.RegisterSymbol(symbol);
            }
            else
            {
                RegisterSymbol(node.Parent, symbol);
            }
        }

        private Symbol LookupSymbol(AstNode node, string identifier)
        {
            if (node is ScopeDefiningAstNode scopeDefiningAstNode)
            {
                if (scopeDefiningAstNode.SymbolTable.TryGetSymbol(identifier, out Symbol symbol))
                {
                    return symbol;
                }
            }
            return LookupSymbol(node.Parent, identifier);
        }
    }
}
