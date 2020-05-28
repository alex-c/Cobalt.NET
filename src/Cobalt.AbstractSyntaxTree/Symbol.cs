using Cobalt.AbstractSyntaxTree.Types;

namespace Cobalt.AbstractSyntaxTree
{
    public class Symbol
    {
        public string Identifier { get; }

        public ITypeSignature Type { get; }

        public bool ValueAssigned { get; set; }

        public int DefinedOnLine { get; }

        public Symbol(string identifier, ITypeSignature type, bool valueAssigned, int definedOnLine)
        {
            Identifier = identifier;
            Type = type;
            ValueAssigned = valueAssigned;
            DefinedOnLine = definedOnLine;
        }
    }
}
