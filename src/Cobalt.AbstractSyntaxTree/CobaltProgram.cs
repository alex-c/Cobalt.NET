namespace Cobalt.AbstractSyntaxTree
{
    public class CobaltProgram
    {
        public CodeBlockNode Code { get; set; }

        public override string ToString()
        {
            return $"{GetType().Name}({Code})";
        }
    }
}
