using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.BinaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.UnaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.AbstractSyntaxTree.Types;
using Cobalt.SemanticAnalysis.Exceptions;
using Cobalt.Shared;
using Cobalt.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

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
                    case VariableAssignmentStatementNode variableAssignment:
                        AnalyzeVariableAssignment(variableAssignment);
                        break;
                    case StandardInputStatementNode standardInputStatement:
                        AnalzyeStandardInputStatement(standardInputStatement);
                        break;
                    case StandardOutputStatementNode standardOutputStatement:
                        AnalyzeStandardOutputStatement(standardOutputStatement);
                        break;
                    default:
                        throw new NotImplementedException($"No semantic analysis implemented for AST node of type `{statement.GetType()}`.");
                }
            }
        }

        #region Statements

        private void AnalyzeVariableDeclaration(VariableDeclarationStatementNode variableDeclaration)
        {
            CobaltType variableType;
            if (variableDeclaration.TypeKeyword != null && variableDeclaration.Expression != null)
            {
                CobaltType expressionType = AnalyzeExpression(variableDeclaration.Expression);
                if (expressionType != variableDeclaration.TypeKeyword.Type)
                {
                    throw new CobaltTypeError($"Type mismatch between explicitely declared type of variable `{variableDeclaration.Identifier.IdentifierName}` ({variableDeclaration.TypeKeyword.Type}) and type of expression ({expressionType}).", variableDeclaration.Expression.SourceLine);
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
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture in variable declaration from line {variableDeclaration.SourceLine}.");
            }
            Symbol variable = new Symbol(variableDeclaration.Identifier.IdentifierName,
                new VariableTypeSignature(variableType),
                variableDeclaration.Expression != null,
                variableDeclaration.SourceLine);
            RegisterSymbol(variableDeclaration.Parent, variable);
        }

        private void AnalyzeVariableAssignment(VariableAssignmentStatementNode variableAssignment)
        {
            Symbol variable = LookupSymbol(variableAssignment.Parent, variableAssignment.Identifier.IdentifierName);
            CobaltType expressionType = AnalyzeExpression(variableAssignment.Expression);
            if (variable.Type is VariableTypeSignature variableType)
            {
                if (variableType.CobaltType == expressionType)
                {
                    variable.Initialized = true;
                }
                else
                {
                    throw new CobaltTypeError($"Type mismatch between variable type of variable `{variableAssignment.Identifier.IdentifierName}` ({variableType.CobaltType}) and type of expression ({expressionType}).", variableAssignment.Expression.SourceLine);
                }
            }
            else
            {
                throw new CompilerException($"The type signature of identifier `{variable.Identifier}` is not a variable type signature.");
            }
        }

        private void AnalzyeStandardInputStatement(StandardInputStatementNode standardInput)
        {
            Symbol identifier = LookupSymbol(standardInput.Parent, standardInput.Identifier.IdentifierName);
            if (identifier.Type is VariableTypeSignature)
            {
                identifier.Initialized = true;
            }
            else
            {
                throw new CobaltTypeError($"Standard input called with an identifier that is not a variable (`{identifier.Identifier}`).", standardInput.Identifier.SourceLine);
            }
        }

        private void AnalyzeStandardOutputStatement(StandardOutputStatementNode standardOutput)
        {
            AnalyzeExpression(standardOutput.Expression);
        }

        #endregion

        #region Expressions

        private CobaltType AnalyzeExpression(ExpressionNode expression)
        {
            CobaltType expressionType;
            switch (expression)
            {
                case BinaryExpressionNode binaryExpression:
                    expressionType = AnalyzeBinaryExpression(binaryExpression);
                    break;
                case UnaryExpressionNode unaryExpression:
                    expressionType = AnalyzeUnaryExpression(unaryExpression);
                    break;
                case SingleLeafExpressionNode singleLeafExpression:
                    expressionType = AnalyzeExpressionLeaf(singleLeafExpression.Leaf);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected an expression node.");
            }
            return expressionType;
        }

        private CobaltType AnalyzeBinaryExpression(BinaryExpressionNode binaryExpression)
        {
            CobaltType leftOperandType;
            if (binaryExpression.LeftOperand is ExpressionNode leftOperandExpression)
            {
                leftOperandType = AnalyzeExpression(leftOperandExpression);
            }
            else
            {
                leftOperandType = AnalyzeExpressionLeaf(binaryExpression.LeftOperand);
            }
            CobaltType rightOperandType;
            if (binaryExpression.RightOperand is ExpressionNode rightOperandExpression)
            {
                rightOperandType = AnalyzeExpression(rightOperandExpression);
            }
            else
            {
                rightOperandType = AnalyzeExpressionLeaf(binaryExpression.RightOperand);
            }
            switch (binaryExpression)
            {
                case ArithmeticBinaryExpressionNode arithmeticBinaryExpression:
                    if (!IsNumberType(leftOperandType))
                    {
                        throw new CobaltTypeError($"Left operand of binary arithmetic expression is not a number type, but of type `{leftOperandType}` instead.", binaryExpression.LeftOperand.SourceLine);
                    }
                    if (!IsNumberType(rightOperandType))
                    {
                        throw new CobaltTypeError($"Right operand of binary arithmetic expression is not a number type, but of type `{rightOperandType}` instead.", binaryExpression.RightOperand.SourceLine);
                    }
                    switch (arithmeticBinaryExpression)
                    {
                        case AdditionNode _:
                        case SubstractionNode _:
                        case MultiplicationNode _:
                            if (leftOperandType == CobaltType.Integer && rightOperandType == CobaltType.Integer)
                            {
                                return CobaltType.Integer;
                            }
                            else
                            {
                                return CobaltType.Float;
                            }
                        case DivisionNode _:
                            return CobaltType.Float;
                        default:
                            throw new CompilerException($"Failed analyzing arithmetic binary expression.");
                    }
                case ComparisonNode comparison:
                    if ((IsNumberType(leftOperandType) && IsNumberType(rightOperandType)) ||
                        (leftOperandType == CobaltType.Boolean && rightOperandType == CobaltType.Boolean))
                    {
                        return CobaltType.Boolean;
                    }
                    throw new CobaltTypeError($"Illegal operand types in comparison expression. Left operand is of type `{leftOperandType}`, right operand is of type `{rightOperandType}`.", comparison.SourceLine);
                case LogicalAndNode _:
                case LogicalOrNode _:

                    if (leftOperandType == CobaltType.Boolean && rightOperandType == CobaltType.Boolean)
                    {
                        return CobaltType.Boolean;
                    }
                    else
                    {
                        if (leftOperandType != CobaltType.Boolean)
                        {
                            throw new CobaltTypeError($"Left operand of binary logical expression is not of type `{CobaltType.Boolean}`, but of type `{leftOperandType}` instead.", binaryExpression.LeftOperand.SourceLine);
                        }
                        else
                        {
                            throw new CobaltTypeError($"Right operand of binary logical expression is not of type `{CobaltType.Boolean}`, but of type `{rightOperandType}` instead.", binaryExpression.RightOperand.SourceLine);
                        }
                    }
                default:
                    throw new CompilerException("Failed analyzing binary expression.");
            }
        }

        private CobaltType AnalyzeUnaryExpression(UnaryExpressionNode unaryExpression)
        {
            CobaltType operandType;
            if (unaryExpression.Operand is ExpressionNode operandExpression)
            {
                operandType = AnalyzeExpression(operandExpression);
            }
            else
            {
                operandType = AnalyzeExpressionLeaf(unaryExpression.Operand);
            }
            switch (unaryExpression)
            {
                case ArithmeticNegationNode _:
                    if (IsNumberType(operandType))
                    {
                        return operandType;
                    }
                    throw new CobaltTypeError($"An arithmetic negation (`~`) cannot be applied on an operand of type `{operandType}`.", unaryExpression.SourceLine);
                case LogicalNegationNode _:
                    if (operandType == CobaltType.Boolean)
                    {
                        return operandType;
                    }
                    throw new CobaltTypeError($"A logical negation (`!`) cannot be applied on an operand of type `{operandType}`.", unaryExpression.SourceLine);
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` does not contain an implementation for the unary expression type `{unaryExpression.GetType()}`.");
            }
        }

        public CobaltType AnalyzeExpressionLeaf(AstNode node)
        {
            CobaltType type;
            switch (node)
            {
                case LiteralValueNode literalValue:
                    type = literalValue.Type;
                    break;
                case IdentifierNode identifier:
                    Symbol symbol = LookupSymbol(node.Parent, identifier.IdentifierName);
                    if (!symbol.Initialized)
                    {
                        throw new UninitializedVariableError(symbol.Identifier);
                    }
                    if (symbol.Type is VariableTypeSignature variableType)
                    {
                        type = variableType.CobaltType;
                    }
                    else
                    {
                        throw new CompilerException($"The type signature of identifier `{identifier.IdentifierName}` is not a variable type signature.");
                    }
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad AST stucture. Expected an expression leaf node.");
            }
            return type;
        }

        #endregion

        #region Symbol table helpers

        private void RegisterSymbol(AstNode node, Symbol symbol)
        {
            if (node is ScopeDefiningAstNode scopeDefiningAstNode)
            {
                scopeDefiningAstNode.SymbolTable.RegisterSymbol(symbol);
            }
            else if (node.Parent != null)
            {
                RegisterSymbol(node.Parent, symbol);
            }
            else
            {
                throw new CompilerException($"Could not find a scope-defining AST node. Last parentless visited node is of type {node.GetType()}.");
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
            if (node.Parent != null)
            {
                return LookupSymbol(node.Parent, identifier);
            }
            else
            {
                throw new UndeclaredIdentifierError(identifier);
            }
        }

        #endregion

        private bool IsNumberType(CobaltType type)
        {
            return type == CobaltType.Integer || type == CobaltType.Float;
        }
    }
}
