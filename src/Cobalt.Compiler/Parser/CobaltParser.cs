using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Leafs;
using Cobalt.AbstractSyntaxTree.Statements;
using Cobalt.Compiler.Tokens;
using Cobalt.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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

            // No syntax error encountered, terminate parsing
            return program;
        }

        private CodeBlockNode ParseCodeBlock(List<Token> tokens, int offset, int limit)
        {
            // Validate arguments
            if (offset < 0 || limit <= offset || limit > tokens.Count())
            {
                throw new CompilerException($"`CobaltParser.ParseCodeBlock` called with bad offset and/or limit parameters (offset: {offset}, limit: {limit}).");
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
                        throw new NotImplementedException();
                    case TokenType.Identifier:
                        throw new NotImplementedException();
                    case TokenType.StandardInput:
                        statementNode = ParseStandardInputStatement(statementTokens);
                        break;
                    case TokenType.StandardOutput:
                        throw new NotImplementedException();
                    default:
                        throw new CobaltSyntaxError($"Expected a statement, but got a {token.Type} token instead.", token.SourceLine, token.PositionOnLine);
                }
                codeBlock.Statements.Add(statementNode);
                position = statementLimit + 1;
            }

            // Reached end of the code block, return
            return codeBlock;
        }

        private StatementNode ParseStandardInputStatement(List<Token> tokens)
        {
            if (tokens.Count != 2 ||
                tokens.ElementAt(0).Type != TokenType.StandardInput ||
                tokens.ElementAt(1).Type != TokenType.Identifier)
            {
                throw new CobaltSyntaxError($"Ivalid standard input statement.", tokens.First().SourceLine, tokens.First().PositionOnLine);
            }

            // Create identifier node
            string identifier = tokens.Last().GetData<string>(TokenDataKeys.IDENTIFIER_NAME);
            IdentifierNode identifierNode = new IdentifierNode(tokens.Last().SourceLine, identifier);

            // Create and return input statement node
            return new StandardInputStatementNode(tokens.First().SourceLine, identifierNode);
        }
    }
}
