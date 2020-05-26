using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Expressions.LiteralValues;
using Cobalt.AbstractSyntaxTree.Leafs;
using Cobalt.AbstractSyntaxTree.Leafs.TypeNodes;
using Cobalt.AbstractSyntaxTree.Statements;
using Cobalt.Compiler.Parser;
using Cobalt.Compiler.Tokens;
using Microsoft.Extensions.Logging;
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
        [InlineData("_test")]
        public void ShouldParseIdentifiers(string identifierName)
        {
            // Arrange
            Token token = new Token(TokenType.Identifier, 1, 0);
            token.SetData(TokenDataKeys.IDENTIFIER_NAME, identifierName);

            // Act
            IdentifierNode node = Parser.ParseIdentifier(token);

            // Assert
            Assert.Equal(identifierName, node.IdentifierName);
        }

        [Theory]
        [InlineData(CobaltType.Boolean)]
        [InlineData(CobaltType.Float)]
        [InlineData(CobaltType.Integer)]
        public void ShouldParseTypeKeywords(CobaltType type)
        {
            // Arrange
            Token token = new Token(TokenType.TypeKeyword, 1, 0);
            token.SetData(TokenDataKeys.COBALT_TYPE, type);

            // Act
            TypeNode node = Parser.ParseTypeKeyword(token);

            // Assert
            switch (type)
            {
                case CobaltType.Boolean:
                    Assert.True(node is BooleanTypeNode);
                    break;
                case CobaltType.Float:
                    Assert.True(node is FloatTypeNode);
                    break;
                case CobaltType.Integer:
                    Assert.True(node is IntegerTypeNode);
                    break;
                default:
                    throw new XunitException("No test implemented for this type.");
            }
        }

        [Theory]
        [InlineData(true, CobaltType.Boolean)]
        [InlineData(false, CobaltType.Boolean)]
        [InlineData(3.14f, CobaltType.Float)]
        [InlineData(0f, CobaltType.Float)]
        [InlineData(-3.14f, CobaltType.Float)]
        [InlineData(-3, CobaltType.Integer)]
        [InlineData(0, CobaltType.Integer)]
        [InlineData(12, CobaltType.Integer)]
        public void ShouldParseLiteralValues(object value, CobaltType type)
        {
            // Arrange
            Token token = new Token(TokenType.LiteralValue, 1, 0);
            token.SetData(TokenDataKeys.LITERAL_VALUE, value);
            token.SetData(TokenDataKeys.COBALT_TYPE, type);

            // Act
            LiteralValueNode node = Parser.ParseLitealValue(token);

            // Assert
            switch (type)
            {
                case CobaltType.Boolean:
                    if (node is BooleanValueNode booleanValue)
                    {
                        Assert.Equal((bool)value, booleanValue.Value);
                    }
                    else
                    {
                        throw new XunitException("Wrong Cobalt type.");
                    }
                    break;
                case CobaltType.Float:
                    if (node is FloatValueNode floatValue)
                    {
                        Assert.Equal((float)value, floatValue.Value);
                    }
                    else
                    {
                        throw new XunitException("Wrong Cobalt type.");
                    }
                    break;
                case CobaltType.Integer:
                    if (node is IntegerValueNode integerValue)
                    {
                        Assert.Equal((int)value, integerValue.Value);
                    }
                    else
                    {
                        throw new XunitException("Wrong Cobalt type.");
                    }
                    break;
                default:
                    throw new XunitException("No test implemented for this type.");
            }
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test123")]
        [InlineData("_test")]
        public void ShouldParseInputStatement(string identifierName)
        {
            // Arrange
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.StandardInput, 1, 0),
                new Token(TokenType.Identifier, 0, 6),
                new Token(TokenType.Semicolon, 0, 7 + identifierName.Length),
            };
            tokens.ElementAt(1).SetData(TokenDataKeys.IDENTIFIER_NAME, identifierName);

            // Act
            CobaltProgram program = Parser.Parse(tokens);

            // Assert
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

        [Theory]
        [InlineData(true, CobaltType.Boolean)]
        [InlineData(false, CobaltType.Boolean)]
        [InlineData(3.14f, CobaltType.Float)]
        [InlineData(0f, CobaltType.Float)]
        [InlineData(-3.14f, CobaltType.Float)]
        [InlineData(-3, CobaltType.Integer)]
        [InlineData(0, CobaltType.Integer)]
        [InlineData(12, CobaltType.Integer)]
        public void ShouldParseOutputStatementWithSingleValue(object value, CobaltType type)
        {
            // Arrange
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.StandardOutput, 0, 0),
                new Token(TokenType.LiteralValue, 0, 0),
                new Token(TokenType.Semicolon, 0, 0),
            };
            tokens.ElementAt(1).SetData(TokenDataKeys.LITERAL_VALUE, value);
            tokens.ElementAt(1).SetData(TokenDataKeys.COBALT_TYPE, type);

            // Act
            CobaltProgram program = Parser.Parse(tokens);

            // Assert
            Assert.Single(program.Code.Statements);

            StatementNode statement = program.Code.Statements.First();
            if (statement is StandardOutputStatementNode outputStatement)
            {
                ExpressionNode expression = outputStatement.Expression;
                if (expression is SingleLeafExpressionNode singleLeafExpression)
                {
                    switch (type)
                    {
                        case CobaltType.Boolean:
                            if (singleLeafExpression.Leaf is BooleanValueNode booleanValue)
                            {
                                Assert.Equal((bool)value, booleanValue.Value);
                            }
                            else
                            {
                                throw new XunitException("Wrong Cobalt type.");
                            }
                            break;

                        case CobaltType.Float:
                            if (singleLeafExpression.Leaf is FloatValueNode floatValue)
                            {
                                Assert.Equal((float)value, floatValue.Value);
                            }
                            else
                            {
                                throw new XunitException("Wrong Cobalt type.");
                            }
                            break;

                        case CobaltType.Integer:
                            if (singleLeafExpression.Leaf is IntegerValueNode integerValue)
                            {
                                Assert.Equal((int)value, integerValue.Value);
                            }
                            else
                            {
                                throw new XunitException("Wrong Cobalt type.");
                            }
                            break;
                        default:
                            throw new XunitException("No test implemented for this type!");
                    }
                }
                else
                {
                    throw new XunitException("Wrong expression type.");
                }
            }
            else
            {
                throw new XunitException("Wrong node type.");
            }
        }
    }
}
