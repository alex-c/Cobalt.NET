namespace Cobalt.AbstractSyntaxTree.Nodes
{
    public class CobaltProgram : AstNode
    {
        private CodeBlockNode _code;

        public CodeBlockNode Code
        {
            get
            {
                return _code;
            }
            set
            {
                value.Parent = this;
                _code = value;
            }
        }

        public CobaltProgram() : base(0) { }

        public override string ToString()
        {
            return $"{GetType().Name}({Code})";
        }
    }
}
