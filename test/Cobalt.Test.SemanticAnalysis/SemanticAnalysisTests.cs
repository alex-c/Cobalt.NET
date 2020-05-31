using Cobalt.AbstractSyntaxTree;
using Cobalt.AbstractSyntaxTree.Exceptions;
using Cobalt.AbstractSyntaxTree.Nodes;
using Cobalt.AbstractSyntaxTree.Nodes.Expressions;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues;
using Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeKeywords;
using Cobalt.AbstractSyntaxTree.Nodes.Statements;
using Cobalt.AbstractSyntaxTree.Types;
using Cobalt.SemanticAnalysis;
using Cobalt.SemanticAnalysis.Exceptions;
using Cobalt.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Xunit.Sdk;

namespace Cobalt.Test.SemanticAnalysis
{
    public class SemanticAnalysisTests
    {
        private CobaltSemanticAnalyzer Analyzer { get; }

        public SemanticAnalysisTests()
        {
            Analyzer = new CobaltSemanticAnalyzer(new NullLoggerFactory());
        }

        [Fact]
        public void ShouldSuccessfullyAnalyzeSimpleStandardOutputStatement()
        {
            // Arrange
            string identifier = "test";
            CobaltProgram program = new CobaltProgram()
            {
                Code = new CodeBlockNode(1)
            };
            program.Code.AddStatement(new VariableDeclarationStatementNode(1)
            {
                Identifier = new IdentifierNode(1, identifier),
                TypeKeyword = new IntegerTypeKeywordNode(1),
                Expression = new SingleLeafExpressionNode(1)
                {
                    Leaf = new IntegerValueNode(1, 42)
                }
            });
            program.Code.AddStatement(new StandardOutputStatementNode(2)
            {
                Expression = new SingleLeafExpressionNode(2)
                {
                    Leaf = new IdentifierNode(2, identifier)
                }
            });

            // Act
            Analyzer.Analyze(program);

            // Assert
            if (program.Code.SymbolTable.TryGetSymbol(identifier, out Symbol symbol))
            {
                Assert.Equal(1, symbol.DefinedOnLine);
                Assert.Equal(identifier, symbol.Identifier);
                Assert.True(symbol.Initialized);
                if (symbol.Type is VariableTypeSignature variableType)
                {
                    Assert.Equal(CobaltType.Integer, variableType.CobaltType);
                }
                else
                {
                    throw new XunitException("Bad variable type signature.");
                }
            }
            else
            {
                throw new XunitException("Missing symbol in program scope symbol table.");
            }
        }

        [Fact]
        public void ShouldFailToAnalyzeStandardOutputStatementWithUndeclaredVariable()
        {
            // Arrange
            string identifier = "test";
            CobaltProgram program = new CobaltProgram()
            {
                Code = new CodeBlockNode(1)
            };
            program.Code.AddStatement(new StandardOutputStatementNode(1)
            {
                Expression = new SingleLeafExpressionNode(1)
                {
                    Leaf = new IdentifierNode(1, identifier)
                }
            });

            // Act / Assert
            Assert.Throws<UndeclaredIdentifierError>(() =>
            {
                Analyzer.Analyze(program);
            });
        }

        [Fact]
        public void ShouldFailToAnalyzeStandardOutputStatementWithUnassignedVariable()
        {
            // Arrange
            string identifier = "test";
            CobaltProgram program = new CobaltProgram()
            {
                Code = new CodeBlockNode(1)
            };
            program.Code.AddStatement(new VariableDeclarationStatementNode(1)
            {
                Identifier = new IdentifierNode(1, identifier),
                TypeKeyword = new IntegerTypeKeywordNode(1)
            });
            program.Code.AddStatement(new StandardOutputStatementNode(2)
            {
                Expression = new SingleLeafExpressionNode(2)
                {
                    Leaf = new IdentifierNode(2, identifier)
                }
            });

            // Act / Assert
            Assert.Throws<UninitializedVariableError>(() =>
            {
                Analyzer.Analyze(program);
            });
        }
    }
}
