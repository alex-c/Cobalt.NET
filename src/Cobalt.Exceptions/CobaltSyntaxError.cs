using System;

namespace Cobalt.Exceptions
{
    /// <summary>
    /// Describes a Cobalt syntac error.
    /// </summary>
    public class CobaltSyntaxError : Exception
    {
        /// <summary>
        /// Creates a Cobalt syntax error exception with an error description.
        /// </summary>
        /// <param name="message">Description of the syntax error.</param>
        public CobaltSyntaxError(string message) : base($"Cobalt syntax error: {message}") { }

        /// <summary>
        /// Creates a Cobalt syntax error exception with an error description, source code line number and position on that line.
        /// </summary>
        /// <param name="message">Description of the syntax error.</param>
        /// <param name="sourceLine">Line in the source code where the error is located.</param>
        /// <param name="positionOnline">Position on the line in the source code.</param>
        public CobaltSyntaxError(string message, int sourceLine, int positionOnline): base($"Cobalt syntax error at line {sourceLine}, position {positionOnline}: {message}") { }
    }
}
