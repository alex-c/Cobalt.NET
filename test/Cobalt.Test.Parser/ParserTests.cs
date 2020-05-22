using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Statements;
using Cobalt.Compiler.Parser;
using Cobalt.Compiler.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Cobalt.Test.Parser
{
    /// <summary>
    /// Tests for the <see cref="CobaltParser" />.
    /// </summary>
    public class ParserTests
    {
        /// <summary>
        /// The parser instance to run the tests with.
        /// </summary>
        private CobaltParser Parser { get; }

        /// <summary>
        /// Sets up the test class with a parser instance to run the tests with.
        /// </summary>
        public ParserTests()
        {
            Parser = new CobaltParser(new LoggerFactory());
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test123")]
        public void ShouldParseInputStatement(string identifierName)
        {
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.StandardInput, 1, 0),
                new Token(TokenType.Identifier, 0, 6),
                new Token(TokenType.Semicolon, 0, 7 + identifierName.Length),
            };
            tokens.ElementAt(1).SetData(TokenDataKeys.IDENTIFIER_NAME, identifierName);

            CobaltProgram program = Parser.Parse(tokens);

            Assert.Single(program.Code.Statements);

            StatementNode statement = program.Code.Statements.First();
            if (statement is StandardInputStatementNode inputStatement)
            {
                Assert.Equal(identifierName, inputStatement.Identifier.IdentifierName);
            }
            else
            {
                throw new XunitException("Wrong node type.");
            }
        }
    }
}
