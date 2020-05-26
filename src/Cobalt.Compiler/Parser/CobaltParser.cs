using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Leafs;
using Cobalt.AbstractSyntaxTree.Leafs.TypeNodes;
using Cobalt.AbstractSyntaxTree.Statements;
using Cobalt.Compiler.Tokens;
using Cobalt.Exceptions;
using Microsoft.Extensions.Logging;
using System;
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
            CobaltProgram program = new CobaltProgram
            {
                Code = ParseCodeBlock(tokens, 0, tokens.Count())
            };

            // No error encountered, terminate parsing
            return program;
        }

        private CodeBlockNode ParseCodeBlock(List<Token> tokens, int offset, int limit)
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
                codeBlock.Statements.Add(statementNode);
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
            return new StandardInputStatementNode(tokens.First().SourceLine)
            {
                Identifier = identifier
            };
        }

        private StatementNode ParseStandardOutputStatement(List<Token> tokens)
        {
            if (tokens.Count <= 2 ||
                tokens.ElementAt(0).Type != TokenType.StandardOutput ||
                tokens.ElementAt(1).Type != TokenType.Equal)
            {
                throw new CobaltSyntaxError($"Ivalid standard output statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse expression
            ExpressionNode expression = ParseExpression(tokens.GetRange(2, tokens.Count - 2));

            // Create and return output statement node
            return new StandardOutputStatementNode(tokens.First().SourceLine)
            {
                Expression = expression
            };
        }

        private StatementNode ParseVariableDeclarationStatement(List<Token> tokens)
        {
            if (tokens.Count <= 4 ||
                tokens.ElementAt(0).Type != TokenType.Declaration ||
                tokens.ElementAt(1).Type != TokenType.Identifier ||
                tokens.ElementAt(2).Type != TokenType.Colon ||
                (tokens.ElementAt(3).Type != TokenType.Equal && tokens.ElementAt(4).Type != TokenType.Equal))
            {
                throw new CobaltSyntaxError($"Ivalid variable declaration statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse identifier
            IdentifierNode identifier = ParseIdentifier(tokens.ElementAt(1));

            // Parse type, if applicable
            TypeNode type = null;
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
            return new VariableDeclarationStatement(tokens.First().SourceLine)
            {
                Identifier = identifier,
                Type = type,
                Expression = expression
            };
        }

        private StatementNode ParseVariableAssignmentStatement(List<Token> tokens)
        {
            if (tokens.Count <= 4 ||
                tokens.ElementAt(0).Type != TokenType.Declaration ||
                tokens.ElementAt(1).Type != TokenType.Identifier ||
                tokens.ElementAt(2).Type != TokenType.Colon ||
                tokens.ElementAt(3).Type != TokenType.Equal)
            {
                throw new CobaltSyntaxError($"Ivalid variable assignment statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Parse identifier
            IdentifierNode identifier = ParseIdentifier(tokens.ElementAt(1));

            // Parse expression
            ExpressionNode expression = ParseExpression(tokens.GetRange(4, tokens.Count - 4));

            // Create and return output statement node
            return new VariableAssignmentStatement(tokens.First().SourceLine)
            {
                Identifier = identifier,
                Expression = expression
            };
        }

        #endregion

        private ExpressionNode ParseExpression(List<Token> tokens)
        {
            throw new NotImplementedException();
        }

        #region Leaf nodes

        private IdentifierNode ParseIdentifier(Token token)
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

        private TypeNode ParseTypeKeyword(Token token)
        {
            if (token.Type == TokenType.TypeKeyword)
            {
                TypeNode type = null;
                CobaltType cobaltType = token.GetData<CobaltType>(TokenDataKeys.IDENTIFIER_NAME);
                switch (cobaltType)
                {
                    case CobaltType.Boolean:
                        type = new BooleanTypeNode(token.SourceLine);
                        break;
                    case CobaltType.Float:
                        type = new FloatTypeNode(token.SourceLine);
                        break;
                    case CobaltType.Integer:
                        type = new IntegerTypeNode(token.SourceLine);
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

        #endregion
    }
}
