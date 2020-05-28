namespace Cobalt.AbstractSyntaxTree
{
    public class ScopeDefiningAstNode : AstNode
    {
        public SymbolTable SymbolTable { get; }

        public ScopeDefiningAstNode(int sourceLine) : base(sourceLine, true)
        {
            SymbolTable = new SymbolTable();
        }
    }
}
