using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.BinaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.UnaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeKeywords;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.Compiler.Exceptions;
using Cobalt.Compiler.Tokens;
using Cobalt.Shared;
using Cobalt.Shared.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cobalt.Compiler.Parser
{
    /// <summary>
    /// A parser for the Cobalt programming language.
    /// </summary>
    public class CobaltParser
    {
        /// <summary>
        /// A logger for logging inside the parser.
        /// </summary>
        private ILogger Logger { get; }
        // TODO: Add debug logging to parser

        /// <summary>
        /// Creates a parser instance with a given logger factory to create a local logger from.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to create a local logger from.</param>
        public CobaltParser(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CobaltParser>();
        }

        /// <summary>
        /// Parses a Cobalt program and returns it as an abstract syntax tree. This is the main parsing entrypoint.
        /// </summary>
        /// <param name="tokens">The tokens representing the program to parse.</param>
        /// <returns>Returns a successfully parsed Cobalt program.</returns>
        /// <exception cref="CobaltSyntaxError">Thrown when the input program contains a syntax error.</exception>
        public CobaltProgram Parse(List<Token> tokens)
        {
            // Validate input
            if (!tokens.Any())
            {
                throw new CobaltSyntaxError("A Cobalt program must contain at least one statement.");
            }

            // Parse program as a code block
            CobaltProgram program = new CobaltProgram();
            program.Code = ParseCodeBlock(tokens, 0, tokens.Count());

            // No error encountered, terminate parsing
            return program;
        }

        public CodeBlockNode ParseCodeBlock(List<Token> tokens, int offset, int limit)
        {
            // Validate arguments
            if (offset < 0 || limit <= offset || limit > tokens.Count())
            {
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with bad offset and/or limit parameters (offset: {offset}, limit: {limit}).");
            }

            // Initialize code block node
            CodeBlockNode codeBlock = new CodeBlockNode(tokens.ElementAt(offset).SourceLine);

            // Iteratively parse statements
            int position = offset;
            while (position < limit)
            {
                Token token = tokens.ElementAt(position);
                int statementLimit = tokens.FindIndex(position, t => t.Type == TokenType.Semicolon);
                if (statementLimit < 0)
                {
                    throw new CobaltSyntaxError("Could not find a valid statement. Are you missing a semicolon?", token.SourceLine, token.PositionOnLine);
                }
                List<Token> statementTokens = tokens.GetRange(position, statementLimit - position);
                StatementNode statementNode = null;
                switch (token.Type)
                {
                    case TokenType.Declaration:
                        statementNode = ParseVariableDeclarationStatement(statementTokens);
                        break;
                    case TokenType.Identifier:
                        statementNode = ParseVariableAssignmentStatement(statementTokens);
                        break;
                    case TokenType.StandardInput:
                        statementNode = ParseStandardInputStatement(statementTokens);
                        break;
                    case TokenType.StandardOutput:
                        statementNode = ParseStandardOutputStatement(statementTokens);
                        break;
                    default:
                        throw new CobaltSyntaxError($"Expected a statement, but got a {token.Type} token instead.", token.SourceLine, token.PositionOnLine);
                }
                codeBlock.AddStatement(statementNode);
                position = statementLimit + 1;
            }

            // Reached end of the code block, return
            return codeBlock;
        }

        #region Statements

        private StatementNode ParseStandardInputStatement(List<Token> tokens)
        {
            if (tokens.Count != 2 ||
                tokens.ElementAt(0).Type != TokenType.StandardInput ||
                tokens.ElementAt(1).Type != TokenType.Identifier)
            {
                throw new CobaltSyntaxError($"Ivalid standard input statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse identifier
            IdentifierNode identifier = ParseIdentifier(tokens.Last());

            // Create and return input statement node
            StandardInputStatementNode statement = new StandardInputStatementNode(tokens.First().SourceLine);
            statement.Identifier = identifier;
            return statement;
        }

        public StatementNode ParseStandardOutputStatement(List<Token> tokens)
        {
            if (tokens.Count <= 1 || tokens.ElementAt(0).Type != TokenType.StandardOutput)
            {
                throw new CobaltSyntaxError($"Ivalid standard output statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse expression
            ExpressionNode expression = ParseExpression(tokens.GetRange(1, tokens.Count - 1));

            // Create and return output statement node
            StandardOutputStatementNode statement = new StandardOutputStatementNode(tokens.First().SourceLine);
            statement.Expression = expression;
            return statement;
        }

        public StatementNode ParseVariableDeclarationStatement(List<Token> tokens)
        {
            if (tokens.Count <= 3 ||
                tokens.ElementAt(0).Type != TokenType.Declaration ||
                tokens.ElementAt(1).Type != TokenType.Identifier ||
                tokens.ElementAt(2).Type != TokenType.Colon ||
                (tokens.ElementAt(3).Type != TokenType.Equal && tokens.ElementAt(3).Type != TokenType.TypeKeyword))
            {
                throw new CobaltSyntaxError($"Ivalid variable declaration statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse identifier
            IdentifierNode identifier = ParseIdentifier(tokens.ElementAt(1));

            // Parse type, if applicable
            TypeKeywordNode type = null;
            if (tokens.ElementAt(3).Type == TokenType.TypeKeyword)
            {
                type = ParseTypeKeyword(tokens.ElementAt(3));
            }

            // Parse expression
            ExpressionNode expression = null;
            if (tokens.Count > 4 && tokens.ElementAt(3).Type == TokenType.Equal)
            {
                expression = ParseExpression(tokens.GetRange(4, tokens.Count - 4));
            }
            else if (tokens.Count > 5 && tokens.ElementAt(4).Type == TokenType.Equal)
            {
                expression = ParseExpression(tokens.GetRange(5, tokens.Count - 5));
            }

            // Validate syntax
            if (type == null && expression == null)
            {
                throw new CobaltSyntaxError("Variable declaration has no explicit type and no expression to infer a type from.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Create and return output statement node
            VariableDeclarationStatementNode statement = new VariableDeclarationStatementNode(tokens.First().SourceLine);
            statement.Identifier = identifier;
            if (type != null)
            {
                statement.TypeKeyword = type;
            }
            if (expression != null)
            {
                statement.Expression = expression;
            }
            return statement;
        }

        public StatementNode ParseVariableAssignmentStatement(List<Token> tokens)
        {
            if (tokens.Count <= 3 ||
                tokens.ElementAt(0).Type != TokenType.Identifier ||
                tokens.ElementAt(1).Type != TokenType.Colon ||
                tokens.ElementAt(2).Type != TokenType.Equal)
            {
                throw new CobaltSyntaxError($"Ivalid variable assignment statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse identifier
            IdentifierNode identifier = ParseIdentifier(tokens.ElementAt(0));

            // Parse expression
            ExpressionNode expression = ParseExpression(tokens.GetRange(3, tokens.Count - 3));

            // Create and return output statement node
            VariableAssignmentStatementNode statement = new VariableAssignmentStatementNode(tokens.First().SourceLine);
            statement.Identifier = identifier;
            statement.Expression = expression;
            return statement;
        }

        #endregion

        #region Expressions

        public ExpressionNode ParseExpression(List<Token> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new CobaltSyntaxError("Empty expression.");
            }

            Stack<Token> operatorStack = new Stack<Token>();
            Stack<AstNode> outputStack = new Stack<AstNode>();

            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Identifier:
                        outputStack.Push(ParseIdentifier(token));
                        break;
                    case TokenType.LiteralValue:
                        outputStack.Push(ParseLitealValue(token));
                        break;
                    case TokenType.LeftParenthesis:
                        operatorStack.Push(token);
                        break;
                    case TokenType.And:
                    case TokenType.Or:
                    case TokenType.Not:
                    case TokenType.Equals:
                    case TokenType.NotEquals:
                    case TokenType.Less:
                    case TokenType.Greater:
                    case TokenType.EqualsOrLess:
                    case TokenType.EqualsOrGreater:
                    case TokenType.Plus:
                    case TokenType.Minus:
                    case TokenType.Asterisk:
                    case TokenType.Slash:
                    case TokenType.Tilde:
                        while (operatorStack.Any() &&
                            operatorStack.Peek().Type != TokenType.LeftParenthesis &&
                            token.GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE) < operatorStack.Peek().GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE))
                        {
                            Token operatorToken = operatorStack.Pop();
                            int operatorArity = operatorToken.GetData<int>(TokenDataKeys.OPERATOR_ARITY);
                            switch (operatorArity)
                            {
                                case 1:
                                    AstNode operand = outputStack.Pop();
                                    outputStack.Push(ParseUnaryExpression(operatorToken, operand));
                                    break;
                                case 2:
                                    AstNode rightOperand = outputStack.Pop();
                                    AstNode leftOperand = outputStack.Pop();
                                    outputStack.Push(ParseBinaryExpresssion(operatorToken, leftOperand, rightOperand));
                                    break;
                                default:
                                    throw new CompilerException($"Operator has illegal arity {operatorArity}.");
                            }
                        }
                        operatorStack.Push(token);
                        break;
                    case TokenType.RightParenthesis:
                        while (operatorStack.Any() && operatorStack.Peek().Type != TokenType.LeftParenthesis)
                        {
                            Token operatorToken = operatorStack.Pop();
                            int operatorArity = operatorToken.GetData<int>(TokenDataKeys.OPERATOR_ARITY);
                            switch (operatorArity)
                            {
                                case 1:
                                    AstNode operand = outputStack.Pop();
                                    outputStack.Push(ParseUnaryExpression(operatorToken, operand));
                                    break;
                                case 2:
                                    AstNode rightOperand = outputStack.Pop();
                                    AstNode leftOperand = outputStack.Pop();
                                    outputStack.Push(ParseBinaryExpresssion(operatorToken, leftOperand, rightOperand));
                                    break;
                                default:
                                    throw new CompilerException($"Operator has illegal arity {operatorArity}.");
                            }
                        }
                        if (operatorStack.Peek().Type != TokenType.LeftParenthesis)
                        {
                            throw new CobaltSyntaxError("Parantheses missmatched in expression.", token.SourceLine, token.PositionOnLine);
                        }
                        operatorStack.Pop();
                        break;
                    default:
                        throw new CobaltSyntaxError($"Illegal token of type `{token.Type}` in expression.", token.SourceLine, token.PositionOnLine);
                }
            }
            while (operatorStack.Any())
            {
                Token operatorToken = operatorStack.Pop();
                if (operatorToken.Type == TokenType.LeftParenthesis || operatorToken.Type == TokenType.RightParenthesis)
                {
                    throw new CobaltSyntaxError("Parantheses missmatched in expression.", operatorToken.SourceLine, operatorToken.PositionOnLine);
                }
                int operatorArity = operatorToken.GetData<int>(TokenDataKeys.OPERATOR_ARITY);
                switch (operatorArity)
                {
                    case 1:
                        AstNode operand = outputStack.Pop();
                        outputStack.Push(ParseUnaryExpression(operatorToken, operand));
                        break;
                    case 2:
                        AstNode rightOperand = outputStack.Pop();
                        AstNode leftOperand = outputStack.Pop();
                        outputStack.Push(ParseBinaryExpresssion(operatorToken, leftOperand, rightOperand));
                        break;
                    default:
                        throw new CompilerException($"Operator has illegal arity {operatorArity}.");
                }
            }
            if (outputStack.Count == 1)
            {
                AstNode result = outputStack.Pop();
                switch (result)
                {
                    case UnaryExpressionNode unaryExpression:
                        return unaryExpression;
                    case BinaryExpressionNode binaryExpression:
                        return binaryExpression;
                    case LiteralValueNode _:
                    case IdentifierNode _:
                        return new SingleLeafExpressionNode(result.SourceLine)
                        {
                            Leaf = result
                        };
                    default:
                        throw new CompilerException($"Illegal node left on output stack after parsing expression: `{result.GetType()}`");
                }
            }
            else
            {
                throw new CompilerException("Parsing expression failed, more than one element left on the output stack.");
            }
        }

        public UnaryExpressionNode ParseUnaryExpression(Token operatorToken, AstNode operand)
        {
            UnaryExpressionNode expression = null;
            switch (operatorToken.Type)
            {
                case TokenType.Not:
                    expression = new LogicalNegationNode(operatorToken.SourceLine);
                    break;
                case TokenType.Tilde:
                    expression = new ArithmeticNegationNode(operatorToken.SourceLine);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with a bad token. Expected an unary operator, token has type `{operatorToken.Type}` instead.");
            }
            expression.Operand = operand;
            return expression;
        }

        public BinaryExpressionNode ParseBinaryExpresssion(Token operatorToken, AstNode leftOperand, AstNode rightOperand)
        {
            BinaryExpressionNode expression = null;
            switch (operatorToken.Type)
            {
                case TokenType.Plus:
                    expression = new AdditionNode(operatorToken.SourceLine);
                    break;
                case TokenType.Minus:
                    expression = new SubstractionNode(operatorToken.SourceLine);
                    break;
                case TokenType.Asterisk:
                    expression = new MultiplicationNode(operatorToken.SourceLine);
                    break;
                case TokenType.Slash:
                    expression = new DivisionNode(operatorToken.SourceLine);
                    break;
                case TokenType.Equals:
                    expression = new EqualsComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.NotEquals:
                    expression = new NotEqualsComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.Less:
                    expression = new LessComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.EqualsOrLess:
                    expression = new EqualsOrLessComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.Greater:
                    expression = new GreaterComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.EqualsOrGreater:
                    expression = new EqualsOrGreaterComparisonNode(operatorToken.SourceLine);
                    break;
                case TokenType.And:
                    expression = new LogicalAndNode(operatorToken.SourceLine);
                    break;
                case TokenType.Or:
                    expression = new LogicalOrNode(operatorToken.SourceLine);
                    break;
                default:
                    throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with a bad token. Expected a binary operator, token has type `{operatorToken.Type}` instead.");
            }
            expression.LeftOperand = leftOperand;
            expression.RightOperand = rightOperand;
            return expression;
        }

        #endregion

        #region Leaf nodes

        public IdentifierNode ParseIdentifier(Token token)
        {
            if (token.Type == TokenType.Identifier)
            {
                string identifierName = token.GetData<string>(TokenDataKeys.IDENTIFIER_NAME);
                IdentifierNode identifier = new IdentifierNode(token.SourceLine, identifierName);
                return identifier;
            }
            else
            {
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with a bad token. Expected a token of type `{TokenType.Identifier}`, got token of type `{token.Type}` instead.");
            }
        }

        public TypeKeywordNode ParseTypeKeyword(Token token)
        {
            if (token.Type == TokenType.TypeKeyword)
            {
                TypeKeywordNode type = null;
                CobaltType cobaltType = token.GetData<CobaltType>(TokenDataKeys.COBALT_TYPE);
                switch (cobaltType)
                {
                    case CobaltType.Boolean:
                        type = new BooleanTypeKeywordNode(token.SourceLine);
                        break;
                    case CobaltType.Float:
                        type = new FloatTypeKeywordNode(token.SourceLine);
                        break;
                    case CobaltType.Integer:
                        type = new IntegerTypeKeywordNode(token.SourceLine);
                        break;
                    default:
                        throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with unknown Cobalt type `{cobaltType}`.");
                }
                return type;
            }
            else
            {
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with a bad token. Expected a token of type `{TokenType.TypeKeyword}`, got token of type `{token.Type}` instead.");
            }
        }

        public LiteralValueNode ParseLitealValue(Token token)
        {
            if (token.Type == TokenType.LiteralValue)
            {
                LiteralValueNode value = null;
                CobaltType cobaltType = token.GetData<CobaltType>(TokenDataKeys.COBALT_TYPE);
                switch (cobaltType)
                {
                    case CobaltType.Boolean:
                        value = new BooleanValueNode(token.SourceLine, token.GetData<bool>(TokenDataKeys.LITERAL_VALUE));
                        break;
                    case CobaltType.Float:
                        value = new FloatValueNode(token.SourceLine, token.GetData<float>(TokenDataKeys.LITERAL_VALUE));
                        break;
                    case CobaltType.Integer:
                        value = new IntegerValueNode(token.SourceLine, token.GetData<int>(TokenDataKeys.LITERAL_VALUE));
                        break;
                    default:
                        throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with unknown Cobalt type `{cobaltType}`.");
                }
                return value;
            }
            else
            {
                throw new CompilerException($"`{MethodBase.GetCurrentMethod().Name}` called with a bad token. Expected a token of type `{TokenType.LiteralValue}`, got token of type `{token.Type}` instead.");
            }
        }

        #endregion
    }
}
