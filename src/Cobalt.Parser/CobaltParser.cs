using Cobalt.AbstractSyntaxTree;
using Cobalt.Exceptions;
using Cobalt.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cobalt.Parser
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
                throw new CompilerException($"CobaltParser.ParseCodeBlock called with bad offset and/or limit parameters (offset: {offset}, limit: {limit}).");
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
                    throw new CobaltSyntaxError("Could not find a statement", token.SourceLine, token.PositionOnLine);
                }
                var statementTokens = tokens.GetRange(position, statementLimit - position);
                switch (token.Type)
                {
                    case TokenType.Declaration:
                        throw new NotImplementedException();
                    case TokenType.Identifier:
                        throw new NotImplementedException();
                    case TokenType.StandardInput:
                        throw new NotImplementedException();
                    case TokenType.StandardOutput:
                        throw new NotImplementedException();
                    default:
                        throw new CobaltSyntaxError($"Expected a statement, bug got a {token.Type} token instead.", token.SourceLine, token.PositionOnLine);
                }
            }

            // Reached end of the code block, return
            return codeBlock;
        }
    }
}
