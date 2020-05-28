using Cobalt.Shared.Exceptions;
using System.Collections.Generic;

namespace Cobalt.Compiler.Tokens
{
    /// <summary>
    /// Represents a token of the Cobalt language.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Holds custom data fields used to enrich this token with further data.
        /// </summary>
        private Dictionary<string, object> Data { get; }

        /// <summary>
        /// The type of the token.
        /// </summary>
        public TokenType Type { get; }

        /// <summary>
        /// The line in the source code this token has been parsed from.
        /// </summary>
        public int SourceLine { get; }

        /// <summary>
        /// The position on the source code line this token starts at.
        /// </summary>
        public int PositionOnLine { get; }

        /// <summary>
        /// Creates a token of a given type from a given line in the source code.
        /// </summary>
        /// <param name="type">Type of the token to create.</param>
        /// <param name="sourceLine">Line in the source code the token has been parsed from.</param>
        /// <param name="positionOnLine">Position on the source code line this token starts at</param>
        public Token(TokenType type, int sourceLine, int positionOnLine)
        {
            Type = type;
            SourceLine = sourceLine;
            PositionOnLine = positionOnLine;
            Data = new Dictionary<string, object>();
        }

        /// <summary>
        /// Sets a custom data field.
        /// </summary>
        /// <typeparam name="T">Type of the data to set.</typeparam>
        /// <param name="key">Key under which to set the data.</param>
        /// <param name="value">The value to set.</param>
        public void SetData<T>(string key, T value)
        {
            Data.Add(key, value);
        }

        /// <summary>
        /// Gets a custom data field.
        /// </summary>
        /// <typeparam name="T">Type of the data to get.</typeparam>
        /// <param name="key">Key under which to look for the data.</param>
        /// <returns>Returns the custom data value.</returns>
        /// <exception cref="CompilerException">Thrown if there is no data under the given key or the data type does not match.</exception>
        public T GetData<T>(string key)
        {
            if (Data.ContainsKey(key))
            {
                object value = Data[key];
                if (typeof(T) == value.GetType())
                {
                    return (T)value;
                }
                else
                {
                    throw new CompilerException($"Tried to read value of type '{typeof(T)}' at the key '{key}' from token of type '{Type}' (from source line {SourceLine}), but got a value of type '{value.GetType()}' instead.");
                }
            }
            else
            {
                throw new CompilerException($"Tried to read missing value of type '{typeof(T)}' at the key '{key}' from token of type '{Type}' (from source line {SourceLine}).");
            }
        }

        /// <summary>
        /// Stringifies the token by taking into account the token type and source line.
        /// </summary>
        /// <returns>Returns a string representation of this token.</returns>
        public override string ToString()
        {
            return $"({Type}:{SourceLine},{PositionOnLine})";
        }
    }
}
