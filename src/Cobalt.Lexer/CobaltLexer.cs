using Cobalt.Exceptions;
using Cobalt.Tokens;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cobalt.Lexer
{
    /// <summary>
    /// A lexer for the Cobalt programming language.
    /// </summary>
    public class CobaltLexer
    {
        /// <summary>
        /// The characters that can be delimiters to multi-char keywords, identifiers and literal values.
        /// </summary>
        private readonly char[] Delimiters = new char[] { ' ', '\t', '\r', '\n', ':', ';', '&', '|', '=', '!', '>', '<', '+', '-', '*', '/', '~', '(', ')' };

        /// <summary>
        /// A regular expression to match valid Cobalt identifiers.
        /// </summary>
        private readonly Regex IdentifierRegex = new Regex("^[a-z,A-Z]\\w*$");

        /// <summary>
        /// A regular expression to match valid Cobalt integer literals.
        /// </summary>
        private readonly Regex IntegerRegex = new Regex("^[0-9]+$");

        /// <summary>
        /// A regular expression to match valid Cobalt floating point numbers.
        /// </summary>
        private readonly Regex FloatRegex = new Regex("^[0-9]+\\.[0-9]+$");

        /// <summary>
        /// A logger for logging inside the lexer.
        /// </summary>
        private ILogger Logger { get; }
        // TODO: Add debug logging to Lexer.

        /// <summary>
        /// Creates a lexer instance with a given logger factory to create a local logger from.
        /// </summary>
        /// <param name="loggerFactory">The logger factory to create a local logger from.</param>
        public CobaltLexer(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<CobaltLexer>();
        }

        /// <summary>
        /// Decomposes the input Cobald code into a collection of <see cref="Token">tokens</see>.
        /// </summary>
        /// <param name="code">The input Cobalt code.</param>
        /// <exception cref="CobaltSyntaxError">Thrown when the input Cobalt code is syntactically incorrect.</exception>
        /// <returns>Returns a collection of tokens.</returns>
        public List<Token> Tokenize(string code)
        {
            List<Token> tokens = new List<Token>();

            int position = 0;
            int line = 1;
            int positionOnLine = 0;

            while (position < code.Length)
            {
                char firstChar = code[position];

                // Ignore whitespace
                if (firstChar == ' ' || firstChar == '\t')
                {
                    position++;
                    positionOnLine++;
                }

                // Check for EOL
                else if (firstChar == '\r' || firstChar == '\n')
                {
                    if (firstChar == '\r' && code.Length > position + 1 && code[position + 1] == '\n')
                    {
                        position++; // \r\n is only one line break
                    }
                    position++;
                    line++;
                    positionOnLine = 0;
                }

                // Non whitespace/line break
                else
                {
                    switch (firstChar)
                    {
                        case ':':
                            tokens.Add(new Token(TokenType.Colon, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case ';':
                            tokens.Add(new Token(TokenType.Semicolon, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '&':
                            tokens.Add(new Token(TokenType.And, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '|':
                            tokens.Add(new Token(TokenType.Or, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '=':
                            tokens.Add(new Token(TokenType.Equal, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '!':
                            tokens.Add(new Token(TokenType.Not, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '<':
                            tokens.Add(new Token(TokenType.Less, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '>':
                            tokens.Add(new Token(TokenType.Greater, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '+':
                            tokens.Add(new Token(TokenType.Plus, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '-':
                            tokens.Add(new Token(TokenType.Minus, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '*':
                            tokens.Add(new Token(TokenType.Asterisk, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '/':
                            // Ignore content of EOL comments
                            if (code.Length > position + 1 && code[position + 1] == '/')
                            {
                                int eolPosition = -1;
                                try
                                {
                                    eolPosition = code.IndexOfAny(new char[] { '\r', '\n' }, position + 2);
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    throw new CobaltSyntaxError("Could not find an EOL to terminate the EOL comment.", line, positionOnLine);
                                }
                                if (eolPosition == -1)
                                {
                                    throw new CobaltSyntaxError("Could not find an EOL to terminate the EOL comment.", line, positionOnLine);
                                }
                                else
                                {
                                    position = eolPosition;
                                }
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.Slash, line, positionOnLine));
                                position++;
                                positionOnLine++;
                            }
                            break;
                        case '~':
                            tokens.Add(new Token(TokenType.Tilde, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case '(':
                            tokens.Add(new Token(TokenType.LeftParenthesis, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;
                        case ')':
                            tokens.Add(new Token(TokenType.RightParenthesis, line, positionOnLine));
                            position++;
                            positionOnLine++;
                            break;

                        // Handle multi-char keywords, identifiers and values
                        default:
                            // Find the limit of the next multi-char word
                            int nextWordLimit = code.IndexOfAny(Delimiters, position);
                            if (nextWordLimit < 0)
                            {
                                throw new CobaltSyntaxError("Failed finding end of word, encountered EOF instead. Make sure any statement is terminated by a semicolon (`;`).", line, positionOnLine);
                            }

                            // Get the word length
                            int nextWordLength = nextWordLimit - position;
                            if (nextWordLength > 0)
                            {
                                // Parse the word! Incrementing of position counter at the end.
                                string nextWord = code.Substring(position, nextWordLength);
                                switch (nextWord)
                                {
                                    case "def":
                                        tokens.Add(new Token(TokenType.Declaration, line, positionOnLine));
                                        break;
                                    case "stdin":
                                        tokens.Add(new Token(TokenType.StandardInput, line, positionOnLine));
                                        break;
                                    case "stdout":
                                        tokens.Add(new Token(TokenType.StandardOutput, line, positionOnLine));
                                        break;
                                    case "bool":
                                        tokens.Add(CreateTypeKeywordToken(CobaltType.Boolean, line, positionOnLine));
                                        break;
                                    case "int":
                                        tokens.Add(CreateTypeKeywordToken(CobaltType.Integer, line, positionOnLine));
                                        break;
                                    case "float":
                                        tokens.Add(CreateTypeKeywordToken(CobaltType.Float, line, positionOnLine));
                                        break;
                                    case "true":
                                        tokens.Add(CreateLiteralValueToken(CobaltType.Boolean, true, line, positionOnLine));
                                        break;
                                    case "false":
                                        tokens.Add(CreateLiteralValueToken(CobaltType.Boolean, false, line, positionOnLine));
                                        break;
                                    default:
                                        // Try to parse number literals
                                        if (FloatRegex.Match(nextWord).Success && float.TryParse(nextWord, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                                        {
                                            tokens.Add(CreateLiteralValueToken(CobaltType.Float, floatValue, line, positionOnLine));
                                        }
                                        else if (IntegerRegex.Match(nextWord).Success && int.TryParse(nextWord, out int intValue))
                                        {
                                            tokens.Add(CreateLiteralValueToken(CobaltType.Integer, intValue, line, positionOnLine));
                                        }

                                        // Only identifiers are left to check!
                                        else if (IdentifierRegex.Match(nextWord).Success)
                                        {
                                            tokens.Add(new Token(TokenType.Identifier, line, positionOnLine));
                                        }
                                        else
                                        {
                                            throw new CobaltSyntaxError($"Failed to parse word `{nextWord}`, which is no valid Cobalt keyword, literal or identifier.", line, positionOnLine);
                                        }
                                        break;
                                }

                                // Increment position in source code by word length
                                position += nextWord.Length;
                                positionOnLine += nextWord.Length;
                            }
                            else if (nextWordLimit == 0)
                            {
                                throw new CobaltSyntaxError("Expected a multi-char token, but encountered a delimiter.", line, positionOnLine);
                            }
                            else
                            {
                                throw new CompilerException("Found a multi-char token limit prior to it's start...");
                            }
                            break;
                    }
                }
            }

            // Coalesce operators
            List<Token> tokensWithCoalescedOperators = CoalesceOperators(tokens);

            // Enrich operators with precedence and arity information
            EnrichOperators(tokensWithCoalescedOperators);

            // The end of the input code was reached and no syntax error could be dectected so far!
            return tokensWithCoalescedOperators;
        }

        /// <summary>
        /// Creates a token for a type keyword.
        /// </summary>
        /// <param name="type">The Cobalt type this is a keyword for.</param>
        /// <param name="line">The line in the source code where the token is from.</param>
        /// <returns>Returns the newly created token.</returns>
        private Token CreateTypeKeywordToken(CobaltType type, int line, int positionOnLine)
        {
            Token token = new Token(TokenType.TypeKeyword, line, positionOnLine);
            token.SetData(TokenDataKeys.COBALT_TYPE, type);
            return token;
        }

        /// <summary>
        /// Creates a token for a literal value.
        /// </summary>
        /// <param name="type">The Cobalt type of the literal vbalue.</param>
        /// <param name="value">The literal value.</param>
        /// <param name="line">The line in the source code where the token is from.</param>
        /// <returns>Returns the newly created token.</returns>
        private Token CreateLiteralValueToken(CobaltType type, object value, int line, int positionOnLine)
        {
            Token token = new Token(TokenType.LiteralValue, line, positionOnLine);
            token.SetData(TokenDataKeys.COBALT_TYPE, type);
            token.SetData(TokenDataKeys.LITERAL_VALUE, value);
            return token;
        }

        /// <summary>
        /// Creates a new list of tokens by coalescing operators (eg. making a "equals or less" operator from a "less" token followed by an "equal" token).
        /// </summary>
        /// <param name="tokens">The input tokens to process.</param>
        /// <returns>The newly created token list with coalesced operators.</returns>
        private List<Token> CoalesceOperators(List<Token> tokens)
        {
            List<Token> result = new List<Token>();
            int position = 0;

            while (position < tokens.Count)
            {
                Token token = tokens.ElementAt(position);
                if (position == tokens.Count - 1)
                {
                    result.Add(token);
                    position++;
                }
                else if (token.Type == TokenType.Equal && tokens.ElementAt(position + 1).Type == TokenType.Equal)
                {
                    result.Add(new Token(TokenType.Equals, token.SourceLine, token.PositionOnLine));
                    position += 2;
                }
                else if (token.Type == TokenType.Less && tokens.ElementAt(position + 1).Type == TokenType.Equal)
                {
                    result.Add(new Token(TokenType.EqualsOrLess, token.SourceLine, token.PositionOnLine));
                    position += 2;
                }
                else if (token.Type == TokenType.Greater && tokens.ElementAt(position + 1).Type == TokenType.Equal)
                {
                    result.Add(new Token(TokenType.EqualsOrGreater, token.SourceLine, token.PositionOnLine));
                    position += 2;
                }
                else if (token.Type == TokenType.Not && tokens.ElementAt(position + 1).Type == TokenType.Equal)
                {
                    result.Add(new Token(TokenType.NotEquals, token.SourceLine, token.PositionOnLine));
                    position += 2;
                }
                else
                {
                    result.Add(token);
                    position++;
                }
            }

            return result;
        }

        /// <summary>
        /// Enriches operators by adding precedence and arity information.
        /// </summary>
        /// <param name="tokens">The tokens for which to enrich operator tokens.</param>
        private void EnrichOperators(ICollection<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.And:
                    case TokenType.Or:
                        EnrichOperatorToken(token, 1, 2);
                        break;
                    case TokenType.Not:
                        EnrichOperatorToken(token, 2, 1);
                        break;
                    case TokenType.Equals:
                    case TokenType.NotEquals:
                    case TokenType.Less:
                    case TokenType.Greater:
                    case TokenType.EqualsOrLess:
                    case TokenType.EqualsOrGreater:
                        EnrichOperatorToken(token, 3, 2);
                        break;
                    case TokenType.Plus:
                    case TokenType.Minus:
                        EnrichOperatorToken(token, 4, 2);
                        break;
                    case TokenType.Asterisk:
                    case TokenType.Slash:
                        EnrichOperatorToken(token, 5, 2);
                        break;
                    case TokenType.Tilde:
                        EnrichOperatorToken(token, 6, 1);
                        break;
                }
            }
        }

        /// <summary>
        /// Enriches an operator token with precedence and arity information.
        /// </summary>
        /// <param name="token">The token to enrich.</param>
        /// <param name="precedence">The operator precedence.</param>
        /// <param name="arity">The operator arity.</param>
        private void EnrichOperatorToken(Token token, int precedence, int arity)
        {
            token.SetData(TokenDataKeys.OPERATOR_PRECEDENCE, precedence);
            token.SetData(TokenDataKeys.OPERATOR_ARITY, arity);
        }
    }
}
