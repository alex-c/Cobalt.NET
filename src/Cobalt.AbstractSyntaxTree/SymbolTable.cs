using System;
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
                // TODO: define exception
                throw new NotImplementedException();
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

        // TODO: method to update symbol
    }
}
