using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.AbstractSyntaxTree.Types;
using Cobalt.SemanticAnalysis.Exceptions;
using Cobalt.Shared;
using Cobalt.Shared.Exceptions;
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
            CobaltType variableType;
            if (variableDeclaration.TypeKeyword != null && variableDeclaration.Expression != null)
            {
                CobaltType expressionType = AnalyzeExpression(variableDeclaration.Expression);
                if (expressionType != variableDeclaration.TypeKeyword.Type)
                {
                    throw new CobaltTypeError($"Type mismatch between explicitely declared type of variable `{variableDeclaration.Identifier.IdentifierName}` and type of expression.", variableDeclaration.Expression.SourceLine);
                }
                variableType = expressionType;
            }
            else if (variableDeclaration.TypeKeyword == null && variableDeclaration.Expression != null)
            {
                variableType = AnalyzeExpression(variableDeclaration.Expression);
            }
            else if (variableDeclaration.TypeKeyword != null && variableDeclaration.Expression == null)
            {
                variableType = variableDeclaration.TypeKeyword.Type;
            }
            else
            {
                throw new CompilerException("");
            }
            Symbol variable = new Symbol(variableDeclaration.Identifier.IdentifierName,
                new VariableTypeSignature(variableType),
                variableDeclaration.Expression != null,
                variableDeclaration.SourceLine);
            RegisterSymbol(variableDeclaration.Parent, variable);
        }

        private CobaltType AnalyzeExpression(ExpressionNode expression)
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
