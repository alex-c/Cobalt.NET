namespace Cobalt.Compiler.Tokens
{
    /// <summary>
    /// Defines the existing types of Cobalt tokens.
    /// </summary>
    public enum TokenType
    {
        // Single-char keywords
        Colon,
        Semicolon,
        And,
        Or,
        Equal,
        Not,
        Less,
        Greater,
        Plus,
        Minus,
        Asterisk,
        Slash,
        Tilde,
        LeftParenthesis,
        RightParenthesis,

        // Multi-char keywords
        Declaration,
        StandardInput,
        StandardOutput,
        TypeKeyword,

        // Identifier
        Identifier,

        // Literal values
        LiteralValue,

        // Operators
        Equals,
        EqualsOrLess,
        EqualsOrGreater,
        NotEquals
    }
}
