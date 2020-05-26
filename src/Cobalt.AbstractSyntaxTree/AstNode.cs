﻿namespace Cobalt.AbstractSyntaxTree
{
    public class AstNode
    {
        public int SourceLine { get; }

        public bool DefinesScope { get; }

        public AstNode(int sourceLine, bool definesScope = false)
        {
            SourceLine = sourceLine;
            DefinesScope = definesScope;
        }

        public override string ToString()
        {
            return $"{GetType().Name}";
        }
    }
}
