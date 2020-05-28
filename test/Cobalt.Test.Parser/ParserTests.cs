using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions.BinaryExpressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeNodes;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
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

        #region Leaf node parsing

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

        #endregion

        #region Expression parsing

        [Theory]
        [InlineData(true, CobaltType.Boolean)]
        [InlineData(false, CobaltType.Boolean)]
        [InlineData(3.14f, CobaltType.Float)]
        [InlineData(0f, CobaltType.Float)]
        [InlineData(-3.14f, CobaltType.Float)]
        [InlineData(-3, CobaltType.Integer)]
        [InlineData(0, CobaltType.Integer)]
        [InlineData(12, CobaltType.Integer)]
        public void ShouldParseSingleValueExpressions(object value, CobaltType type)
        {
            // Arrange
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.LiteralValue, 1, 0)
            };
            tokens.First().SetData(TokenDataKeys.LITERAL_VALUE, value);
            tokens.First().SetData(TokenDataKeys.COBALT_TYPE, type);

            // Act
            ExpressionNode expression = Parser.ParseExpression(tokens);

            // Assert
            if (expression is SingleLeafExpressionNode singleLeafExpression)
            {
                if (singleLeafExpression.Leaf is BooleanValueNode booleanValue)
                {
                    Assert.Equal(CobaltType.Boolean, type);
                    Assert.Equal((bool)value, booleanValue.Value);
                }
                else if (singleLeafExpression.Leaf is FloatValueNode floatValue)
                {
                    Assert.Equal(CobaltType.Float, type);
                    Assert.Equal((float)value, floatValue.Value);
                }
                else if (singleLeafExpression.Leaf is IntegerValueNode integerValue)
                {
                    Assert.Equal(CobaltType.Integer, type);
                    Assert.Equal((int)value, integerValue.Value);
                }
                else
                {
                    throw new XunitException("Wrong expression type.");
                }
            }
            else
            {
                throw new XunitException("Wrong expression type.");
            }
        }

        [Theory]
        [InlineData("test")]
        [InlineData("test123")]
        [InlineData("_test")]
        public void ShouldParseSingleIdentifierExpressions(string identifierName)
        {
            // Arrange
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.Identifier, 1, 0)
            };
            tokens.First().SetData(TokenDataKeys.IDENTIFIER_NAME, identifierName);

            // Act
            ExpressionNode expression = Parser.ParseExpression(tokens);

            // Assert
            if (expression is SingleLeafExpressionNode singleLeafExpression)
            {
                if (singleLeafExpression.Leaf is IdentifierNode identifierNode)
                {
                    Assert.Equal(identifierName, identifierNode.IdentifierName);
                }
                else
                {
                    throw new XunitException("Wrong expression type.");
                }
            }
            else
            {
                throw new XunitException("Wrong expression type.");
            }
        }

        [Theory]
        [InlineData(TokenType.Plus, 4)]
        [InlineData(TokenType.Minus, 4)]
        [InlineData(TokenType.Asterisk, 5)]
        [InlineData(TokenType.Slash, 5)]
        public void ShouldParseArithmeticExpressions(TokenType operatorType, int precedence)
        {
            // Arrange
            List<Token> tokens = new List<Token>()
            {
                new Token(TokenType.Identifier, 1, 0),
                new Token(operatorType, 1, 4),
                new Token(TokenType.Identifier, 1, 5)
            };
            tokens.First().SetData(TokenDataKeys.IDENTIFIER_NAME, "left");
            tokens.Last().SetData(TokenDataKeys.IDENTIFIER_NAME, "right");
            Token operatorToken = tokens.ElementAt(1);
            operatorToken.SetData(TokenDataKeys.OPERATOR_ARITY, 2);
            operatorToken.SetData(TokenDataKeys.OPERATOR_PRECEDENCE, precedence);

            // Act
            ExpressionNode expression = Parser.ParseExpression(tokens);

            // Assert
            if (expression is BinaryExpressionNode binaryExpression)
            {
                Assert.True(binaryExpression.LeftOperand is IdentifierNode);
                Assert.True(binaryExpression.RightOperand is IdentifierNode);
                Assert.Equal("left", ((IdentifierNode)binaryExpression.LeftOperand).IdentifierName);
                Assert.Equal("right", ((IdentifierNode)binaryExpression.RightOperand).IdentifierName);
                switch (operatorType)
                {
                    case TokenType.Plus:
                        Assert.True(binaryExpression is AdditionNode);
                        break;
                    case TokenType.Minus:
                        Assert.True(binaryExpression is SubstractionNode);
                        break;
                    case TokenType.Asterisk:
                        Assert.True(binaryExpression is MultiplicationNode);
                        break;
                    case TokenType.Slash:
                        Assert.True(binaryExpression is DivisionNode);
                        break;
                    default:
                        throw new XunitException("No test implemented for this operator type.");
                }
            }
            else
            {
                throw new XunitException("Wrong expression type.");
            }

        }

        #endregion

        #region Statement parsing

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

        #endregion
    }
}
