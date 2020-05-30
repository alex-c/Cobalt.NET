using System;

namespace Cobalt.SemanticAnalysis.Exceptions
{
    /// <summary>
    /// Describes an uninitialized variable error.
    /// </summary>
    public class UninitializedVariableError : Exception
    {
        /// <summary>
        /// Creates an uninitialized variable error.
        /// </summary>
        /// <param name="identifier">The identifier of the variable that is not initialized.</param>
        public UninitializedVariableError(string identifier) : base($"The variable with identifier `{identifier}` is being used without having been initialized.") { }
    }
}
