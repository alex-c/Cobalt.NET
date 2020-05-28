using System;

namespace Cobalt.SemanticAnalysis.Exceptions
{
    /// <summary>
    /// Describes a Cobalt type error.
    /// </summary>
    public class CobaltTypeError : Exception
    {
        /// <summary>
        /// Creates a Cobalt type error exception with an error description.
        /// </summary>
        /// <param name="message">Description of the type error.</param>
        public CobaltTypeError(string message) : base($"Cobalt type error: {message}") { }

        /// <summary>
        /// Creates a Cobalt type error exception with an error description and source code line number.
        /// </summary>
        /// <param name="message">Description of the type error.</param>
        /// <param name="sourceLine">Line in the source code where the error is located.</param>
        public CobaltTypeError(string message, int sourceLine) : base($"Cobalt type error at line {sourceLine}: {message}") { }
    }
}
