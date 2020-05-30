using Cobalt.AbstractSyntaxTree.Types;

namespace Cobalt.AbstractSyntaxTree
{
    public class Symbol
    {
        public string Identifier { get; }

        public ITypeSignature Type { get; }

        public bool Initialized { get; set; }

        public int DefinedOnLine { get; }

        public Symbol(string identifier, ITypeSignature type, bool initialized, int definedOnLine)
        {
            Identifier = identifier;
            Type = type;
            Initialized = initialized;
            DefinedOnLine = definedOnLine;
        }
    }
}
