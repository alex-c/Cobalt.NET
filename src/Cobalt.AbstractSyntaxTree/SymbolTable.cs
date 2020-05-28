using Cobalt.AbstractSyntaxTree.Exceptions;
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

        public void RegisterSymbol(Symbol symbol)
        {
            if (Symbols.ContainsKey(symbol.Identifier))
            {
                throw new DuplicateDeclarationError(symbol);
            }
        }

        public bool TryGetSymbol(string identifier, out Symbol symbol)
        {
            symbol = null;
            if (Symbols.ContainsKey(identifier))
            {
                symbol = Symbols[identifier];
                return true;
            }
            return false;
        }
    }
}
