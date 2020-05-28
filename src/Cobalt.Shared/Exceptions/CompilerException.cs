using System;

namespace Cobalt.Shared.Exceptions
{
    /// <summary>
    /// Describes a Cobalt compiler exception. This should be used for cases where an error does not stem from the Cobalt code being compiled,
    /// but rather from the compiler not working as intended.
    /// </summary>
    public class CompilerException : Exception
    {
        /// <summary>
        /// Creates a Cobalt compiler exception with an error description.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        public CompilerException(string message) : base($"Cobalt compiler exception: {message}") { }

        /// <summary>
        /// Creates a Cobalt compiler exception with an error description and an inner exception.
        /// </summary>
        /// <param name="message">Description of the error.</param>
        /// <param name="innerException">Inner exception that triggered this error.</param>
        public CompilerException(string message, Exception innerException) : base($"Cobalt compiler exception: {message}", innerException) { }
    }
}
