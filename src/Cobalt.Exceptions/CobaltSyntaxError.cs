using System;

namespace Cobalt.Exceptions
{
    /// <summary>
    /// Describes a Cobalt syntac error.
    /// </summary>
    public class CobaltSyntaxError : Exception
    {
        /// <summary>
        /// Creates a Cobalt syntax error exception with a description and line number in the source code.
        /// </summary>
        /// <param name="message">Description of the syntax error.</param>
        /// <param name="sourceLine">Line in the source code where the error is located.</param>
        public CobaltSyntaxError(string message, int sourceLine) : base($"Cobalt syntax error at line {sourceLine}: {message}") { }
    }
}
