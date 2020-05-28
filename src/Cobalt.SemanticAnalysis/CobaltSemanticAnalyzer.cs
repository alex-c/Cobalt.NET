using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
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
            AnalyzeCodeBlock(program.Code);
        }

        private void AnalyzeCodeBlock(CodeBlockNode codeBlock)
        {
            ICollection<StatementNode> statements = codeBlock.Statements;
            foreach (StatementNode statement in statements)
            {
                switch (statement)
                {
                    case VariableDeclarationStatementNode variableDeclaration:
                        AnalyzeVariableDeclaration(variableDeclaration);
                        break;
                    default:
                        throw new NotImplementedException($"No semantic analysis implemented for AST node of type `{statement.GetType()}`.");
                }
            }
        }

        private void AnalyzeVariableDeclaration(VariableDeclarationStatementNode variableDeclaration)
        {
            throw new NotImplementedException();
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
