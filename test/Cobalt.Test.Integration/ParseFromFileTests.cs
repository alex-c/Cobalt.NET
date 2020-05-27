using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Expressions;
using Cobalt.AbstractSyntaxTree.Expressions.BinaryExpressions;
using Cobalt.AbstractSyntaxTree.Leafs;
using Cobalt.AbstractSyntaxTree.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Statements;
using Cobalt.Compiler.Lexer;
using Cobalt.Compiler.Parser;
using Cobalt.Compiler.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace Cobalt.Test.Integration
{
    public class ParseFromFileTests
    {
        private CobaltLexer Lexer { get; }

        private CobaltParser Parser { get; }

        public ParseFromFileTests()
        {
            ILoggerFactory loggerFactory = new NullLoggerFactory();
            Lexer = new CobaltLexer(loggerFactory);
            Parser = new CobaltParser(loggerFactory);
        }

        [Fact]
        public void CalculateMeanOfThreeNumbers()
        {
            // Arrange
            string source = ReadFromFile("mean.co");

            // Act
            List<Token> tokens = Lexer.Tokenize(source);
            CobaltProgram ast = Parser.Parse(tokens);

            // Assert
            Assert.Equal(6, ast.Code.Statements.Count);
            Assert.True(ast.Code.Statements.ElementAt(0) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(1) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(2) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(3) is VariableAssignmentStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(5) is StandardOutputStatementNode);

            StatementNode meanStatement = ast.Code.Statements.ElementAt(ast.Code.Statements.Count - 2);
            if (meanStatement is VariableDeclarationStatementNode variableDeclaration)
            {
                Assert.Equal("mean", variableDeclaration.Identifier.IdentifierName);
                ExpressionNode expression = variableDeclaration.Expression;
                if (expression is DivisionNode division)
                {
                    if (division.LeftOperand is AdditionNode addition)
                    {
                        if (addition.LeftOperand is IdentifierNode identifier)
                        {
                            Assert.Equal("a", identifier.IdentifierName);
                        }
                        else
                        {
                            throw new XunitException("Wrong operand type.");
                        }
                        if (addition.RightOperand is AdditionNode additionNode)
                        {
                            Assert.True(additionNode.LeftOperand is IdentifierNode);
                            Assert.True(additionNode.RightOperand is IdentifierNode);
                        }
                        else
                        {
                            throw new XunitException("Wrong expressin type.");
                        }
                    }
                    else
                    {
                        throw new XunitException("Wrong expressin type.");
                    }
                    if (division.RightOperand is IntegerValueNode integer)
                    {
                        Assert.Equal(3, integer.Value);
                    }
                    else
                    {
                        throw new XunitException("Wrong operand type.");
                    }
                }
                else
                {
                    throw new XunitException("Wrong expression type.");
                }
            }
            else
            {
                throw new XunitException("Wrong statement type.");
            }
        }

        [Fact]
        public void CompareTwoIntegers()
        {
            // Arrange
            string source = ReadFromFile("compare_numbers.co");

            // Act
            List<Token> tokens = Lexer.Tokenize(source);
            CobaltProgram ast = Parser.Parse(tokens);

            // Assert
            Assert.Equal(5, ast.Code.Statements.Count);
            Assert.True(ast.Code.Statements.ElementAt(0) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(1) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(2) is VariableDeclarationStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(3) is VariableAssignmentStatementNode);
            Assert.True(ast.Code.Statements.ElementAt(4) is StandardOutputStatementNode);
        }

        private string ReadFromFile(string fileName)
        {
            return File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Files", fileName));
        }
    }
}
