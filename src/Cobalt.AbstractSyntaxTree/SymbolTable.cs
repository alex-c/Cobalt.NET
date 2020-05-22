using System.Collections.Generic;

namespace Cobalt.AbstractSyntaxTree
{
    public class SymbolTable
    {
        private Dictionary<string, Symbol> Symbols { get; }

        public SymbolTable()
        {
            Symbols = new Dictionary<string, Symbol>();
        }

        // TODO: methods for registering and looking up symbols
    }
}
