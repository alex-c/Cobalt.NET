using System;

namespace Cobalt.AbstractSyntaxTree.Exceptions
{
    /// <summary>
    /// Describes a duplicate declaration error.
    /// </summary>
    public class DuplicateDeclarationError : Exception
    {
        /// <summary>
        /// Creates a duplicate declaration error exception.
        /// </summary>
        /// <param name="symbol">The symbol that failed to be registered because of a duplicate declaration.</param>
        public DuplicateDeclarationError(Symbol symbol) : base($"Duplicate declaration of identifier `{symbol.Identifier}` on line {symbol.DefinedOnLine}.") { }
    }
}
