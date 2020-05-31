using Cobalt.AbstractSyntaxTree.Exceptions;
using Cobalt.Shared.Exceptions;

namespace Cobalt.AbstractSyntaxTree.Nodes
{
    public class AstNode
    {
        public int SourceLine { get; }

        public bool DefinesScope { get; }

        public AstNode Parent { get; set; }

        public AstNode(int sourceLine, bool definesScope = false)
        {
            SourceLine = sourceLine;
            DefinesScope = definesScope;
            Parent = null;
        }

        public Symbol LookupSymbol(string identifier)
        {
            if (this is ScopeDefiningAstNode scopeDefiningAstNode)
            {
                if (scopeDefiningAstNode.SymbolTable.TryGetSymbol(identifier, out Symbol symbol))
                {
                    return symbol;
                }
            }
            if (Parent != null)
            {
                return Parent.LookupSymbol(identifier);
            }
            else
            {
                throw new UndeclaredIdentifierError(identifier);
            }
        }

        public void RegisterSymbol(Symbol symbol)
        {
            if (this is ScopeDefiningAstNode scopeDefiningAstNode)
            {
                scopeDefiningAstNode.SymbolTable.RegisterSymbol(symbol);
            }
            else if (Parent != null)
            {
                Parent.RegisterSymbol(symbol);
            }
            else
            {
                throw new CompilerException($"Could not find a scope-defining AST node. Last parentless visited node is of type {GetType()}.");
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
