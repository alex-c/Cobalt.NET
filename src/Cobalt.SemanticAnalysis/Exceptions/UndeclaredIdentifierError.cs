using System;

namespace Cobalt.SemanticAnalysis.Exceptions
{
    /// <summary>
    /// Describes an undeclared identifier error.
    /// </summary>
    public class UndeclaredIdentifierError : Exception
    {
        /// <summary>
        /// Creates an undeclared identifier error.
        /// </summary>
        /// <param name="identifier">The identifier that is not declared.</param>
        public UndeclaredIdentifierError(string identifier) : base($"The identifier `{identifier}` is being used without having been declared.") { }
    }
}
