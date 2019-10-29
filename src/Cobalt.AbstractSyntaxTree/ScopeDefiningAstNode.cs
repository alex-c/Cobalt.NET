namespace Cobalt.AbstractSyntaxTree
{
    public class ScopeDefiningAstNode : AstNode
    {
        private SymbolTable SymbolTable { get; }

        public ScopeDefiningAstNode(int sourceLine) : base(sourceLine, true)
        {
            SymbolTable = new SymbolTable();
        }

        // TODO: methods for registering and looking up symbols
    }
}
