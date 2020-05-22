using Cobalt.Compiler.Lexer;
using Cobalt.Compiler.Tokens;
using Cobalt.Exceptions;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cobalt.Test.Lexer
{
    /// <summary>
    /// Tests for the <see cref="CobaltLexer" />.
    /// </summary>
    public class LexerTests
    {
        /// <summary>
        /// The lexer instance to run the tests with.
        /// </summary>
        private CobaltLexer Lexer { get; }

        /// <summary>
        /// Sets up the test class with a lexer instance to run the tests with.
        /// </summary>
        public LexerTests()
        {
            Lexer = new CobaltLexer(new LoggerFactory());
        }

        [Fact]
        public void ShouldParseSingleCharKeywords()
        {
            ICollection<Token> tokens = Lexer.Tokenize(":;&|=!<>+-*/~()");

            List<TokenType> expectedTokenTypes = new List<TokenType>()
            {
                TokenType.Colon,
                TokenType.Semicolon,
                TokenType.And,
                TokenType.Or,
                TokenType.Equal,
                TokenType.Not,
                TokenType.Less,
                TokenType.Greater,
                TokenType.Plus,
                TokenType.Minus,
                TokenType.Asterisk,
                TokenType.Slash,
                TokenType.Tilde,
                TokenType.LeftParenthesis,
                TokenType.RightParenthesis
            };

            Assert.Equal(expectedTokenTypes, tokens.Select(t => t.Type));
        }

        [Fact]
        public void ShouldIgnoreWhitespace()
        {
            ICollection<Token> tokens = Lexer.Tokenize("~    ~~~  ~\t~ \t ~");

            Assert.Equal(7, tokens.Count);
            Assert.Equal(7, tokens.Where(t => t.Type == TokenType.Tilde).Count());
        }

        [Fact]
        public void ShouldTrackSourceLineCount()
        {
            ICollection<Token> tokens = Lexer.Tokenize("~\n~\r~\r\n~");

            for (int i = 1; i <= 4; i++)
            {
                Assert.Equal(i, tokens.ElementAt(i - 1).SourceLine);
            }
        }

        [Fact]
        public void ShouldTrackPositionOnSourceLine()
        {
            ICollection<Token> tokens = Lexer.Tokenize("~\n~~\r~~~\r\n~~~~~");

            Assert.Equal(0, tokens.ElementAt(0).PositionOnLine);
            Assert.Equal(0, tokens.ElementAt(1).PositionOnLine);
            Assert.Equal(1, tokens.ElementAt(2).PositionOnLine);
            Assert.Equal(0, tokens.ElementAt(3).PositionOnLine);
            Assert.Equal(1, tokens.ElementAt(4).PositionOnLine);
            Assert.Equal(2, tokens.ElementAt(5).PositionOnLine);
            Assert.Equal(0, tokens.ElementAt(6).PositionOnLine);
            Assert.Equal(1, tokens.ElementAt(7).PositionOnLine);
            Assert.Equal(2, tokens.ElementAt(8).PositionOnLine);
            Assert.Equal(3, tokens.ElementAt(9).PositionOnLine);
        }

        [Fact]
        public void ShouldIgnoreEolComments()
        {
            ICollection<Token> tokens = Lexer.Tokenize("~//Hello this is an EOL comment and `~` is the Tilde character!\r\n~");
            
            Assert.Equal(TokenType.Tilde, tokens.ElementAt(0).Type);
            Assert.Equal(TokenType.Tilde, tokens.ElementAt(1).Type);
            Assert.Equal(1, tokens.ElementAt(0).SourceLine);
            Assert.Equal(2, tokens.ElementAt(1).SourceLine);
        }

        [Fact]
        public void ShouldParseMultiCharKeywords()
        {
            ICollection<Token> tokens = Lexer.Tokenize("def stdin stdout bool int float ");

            List<TokenType> expectedTokenTypes = new List<TokenType>()
            {
                TokenType.Declaration,
                TokenType.StandardInput,
                TokenType.StandardOutput,
                TokenType.TypeKeyword,
                TokenType.TypeKeyword,
                TokenType.TypeKeyword
            };

            Assert.Equal(expectedTokenTypes, tokens.Select(t => t.Type));
            Assert.Equal(CobaltType.Boolean, tokens.ElementAt(3).GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
            Assert.Equal(CobaltType.Integer, tokens.ElementAt(4).GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
            Assert.Equal(CobaltType.Float, tokens.ElementAt(5).GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
        }

        [Theory]
        [InlineData("true ")]
        [InlineData("false ")]
        public void ShouldParseBooleanLiterals(string literalValueString)
        {
            ICollection<Token> tokens = Lexer.Tokenize(literalValueString);

            Assert.Single(tokens);
            Assert.Equal(TokenType.LiteralValue, tokens.First().Type);
            Assert.Equal(CobaltType.Boolean, tokens.First().GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
        }

        [Theory]
        [InlineData("0 ")]
        [InlineData("123 ")]
        [InlineData("12345 ")]
        public void ShouldParseIntegerLiterals(string literalValueString)
        {
            ICollection<Token> tokens = Lexer.Tokenize(literalValueString);

            Assert.Single(tokens);
            Assert.Equal(TokenType.LiteralValue, tokens.First().Type);
            Assert.Equal(CobaltType.Integer, tokens.First().GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
        }

        [Theory]
        [InlineData("0.0 ")]
        [InlineData("1.23 ")]
        [InlineData("12.345 ")]
        public void ShouldParseFloatLiterals(string literalValueString)
        {
            ICollection<Token> tokens = Lexer.Tokenize(literalValueString);

            Assert.Single(tokens);
            Assert.Equal(TokenType.LiteralValue, tokens.First().Type);
            Assert.Equal(CobaltType.Float, tokens.First().GetData<CobaltType>(TokenDataKeys.COBALT_TYPE));
        }

        [Theory]
        [InlineData(".0 ")]
        [InlineData(".123 ")]
        [InlineData("0. ")]
        [InlineData("123. ")]
        public void ShouldFailToParseMalformedFloatLiterals(string malformedLiteralValueString)
        {
            Assert.Throws<CobaltSyntaxError>(() => Lexer.Tokenize(malformedLiteralValueString));
        }

        [Fact]
        public void ShouldParseCorrectLiteralValues()
        {
            ICollection<Token> tokens = Lexer.Tokenize("true false 0 123 0.0 1.23 ");

            // Check token types
            Assert.Equal(6, tokens.Count);
            Assert.Equal(6, tokens.Where(t => t.Type == TokenType.LiteralValue).Count());

            // Check boolean values
            Assert.True(tokens.ElementAt(0).GetData<bool>(TokenDataKeys.LITERAL_VALUE));
            Assert.False(tokens.ElementAt(1).GetData<bool>(TokenDataKeys.LITERAL_VALUE));

            // Check integer values
            Assert.Equal(0, tokens.ElementAt(2).GetData<int>(TokenDataKeys.LITERAL_VALUE));
            Assert.Equal(123, tokens.ElementAt(3).GetData<int>(TokenDataKeys.LITERAL_VALUE));

            // Check floating point values
            Assert.Equal(.0f, tokens.ElementAt(4).GetData<float>(TokenDataKeys.LITERAL_VALUE));
            Assert.Equal(1.23f, tokens.ElementAt(5).GetData<float>(TokenDataKeys.LITERAL_VALUE));
        }

        [Fact]
        public void ShouldCoalesceOperators()
        {
            ICollection<Token> tokens = Lexer.Tokenize("==<=>=!=");

            // Check token count
            Assert.Equal(4, tokens.Count);

            // Check token types
            List<TokenType> expectedTokenTypes = new List<TokenType>()
            {
                TokenType.Equals,
                TokenType.EqualsOrLess,
                TokenType.EqualsOrGreater,
                TokenType.NotEquals
            };
            Assert.Equal(expectedTokenTypes, tokens.Select(t => t.Type));

        }

        [Fact]
        public void ShouldEnrichOperators()
        {
            ICollection<Token> tokens = Lexer.Tokenize("!&|==<=>=!=<>+-*/~");

            // Check token count
            Assert.Equal(14, tokens.Count);

            // Check token types
            List<TokenType> expectedTokenTypes = new List<TokenType>()
            {
                TokenType.Not,
                TokenType.And,
                TokenType.Or,
                TokenType.Equals,
                TokenType.EqualsOrLess,
                TokenType.EqualsOrGreater,
                TokenType.NotEquals,
                TokenType.Less,
                TokenType.Greater,
                TokenType.Plus,
                TokenType.Minus,
                TokenType.Asterisk,
                TokenType.Slash,
                TokenType.Tilde
            };
            Assert.Equal(expectedTokenTypes, tokens.Select(t => t.Type));

            // Check precedence and arity information
            Assert.Equal(2, tokens.ElementAt(0).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(1, tokens.ElementAt(0).GetData<int>(TokenDataKeys.OPERATOR_ARITY));

            Assert.Equal(1, tokens.ElementAt(1).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(1).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(1, tokens.ElementAt(2).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(2).GetData<int>(TokenDataKeys.OPERATOR_ARITY));

            Assert.Equal(3, tokens.ElementAt(3).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(3).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(3, tokens.ElementAt(4).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(4).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(3, tokens.ElementAt(5).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(5).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(3, tokens.ElementAt(6).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(6).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(3, tokens.ElementAt(7).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(7).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(3, tokens.ElementAt(8).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(8).GetData<int>(TokenDataKeys.OPERATOR_ARITY));

            Assert.Equal(4, tokens.ElementAt(9).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(9).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(4, tokens.ElementAt(10).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(10).GetData<int>(TokenDataKeys.OPERATOR_ARITY));

            Assert.Equal(5, tokens.ElementAt(11).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(11).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
            Assert.Equal(5, tokens.ElementAt(12).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(2, tokens.ElementAt(12).GetData<int>(TokenDataKeys.OPERATOR_ARITY));

            Assert.Equal(6, tokens.ElementAt(13).GetData<int>(TokenDataKeys.OPERATOR_PRECEDENCE));
            Assert.Equal(1, tokens.ElementAt(13).GetData<int>(TokenDataKeys.OPERATOR_ARITY));
        }

        [Theory]
        [InlineData("test ")]
        [InlineData("test_ ")]
        [InlineData("te_st ")]
        [InlineData("te_12 ")]
        [InlineData("te12_ ")]
        public void ShouldParseIdentifiers(string identifier)
        {
            ICollection<Token> tokens = Lexer.Tokenize(identifier);

            Assert.Single(tokens);

            Token token = tokens.First();
            Assert.Equal(TokenType.Identifier, token.Type);
            Assert.Equal(identifier.Substring(0, identifier.Length - 1), token.GetData<string>(TokenDataKeys.IDENTIFIER_NAME));
        }

        [Theory]
        [InlineData("_test")]
        [InlineData("123test")]
        [InlineData("12_test ")]
        public void ShouldFailToParseMalformedIdentifier(string malformedidentifier)
        {
            Assert.Throws<CobaltSyntaxError>(() => Lexer.Tokenize(malformedidentifier));
        }
    }
}
