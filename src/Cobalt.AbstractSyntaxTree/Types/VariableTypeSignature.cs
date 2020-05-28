using Cobalt.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cobalt.AbstractSyntaxTree.Types
{
    public class VariableTypeSignature : ITypeSignature
    {
        private CobaltType Type { get; }

        public string Signature
        {
            get
            {
                return Type.ToString();
            }
        }

        public VariableTypeSignature(CobaltType type)
        {
            Type = type;
        }
    }
}
