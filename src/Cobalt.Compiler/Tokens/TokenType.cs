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
        TypeKeyword, // TODO: expand to explicit types?

        // Identifier
        Identifier,

        // Literal values
        LiteralValue, // TODO: expand to explicit types?

        // Operators
        Equals,
        EqualsOrLess,
        EqualsOrGreater,
        NotEquals
    }
}
