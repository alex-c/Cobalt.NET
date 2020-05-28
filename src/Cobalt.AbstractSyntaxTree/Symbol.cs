namespace Cobalt.AbstractSyntaxTree
{
    public class Symbol
    {
        public string Identifier { get; }

        // TODO: design type signature...
        public object Type { get; }

        public bool ValueAssigned { get; set; }

        public int DefinedOnLine { get; }
    }
}
