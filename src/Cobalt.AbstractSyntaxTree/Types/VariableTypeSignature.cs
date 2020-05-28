using Cobalt.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cobalt.AbstractSyntaxTree.Types
{
    public class VariableTypeSignature : ITypeSignature
    {
        public CobaltType CobaltType { get; }

        public string Signature
        {
            get
            {
                return CobaltType.ToString();
            }
        }

        public VariableTypeSignature(CobaltType type)
        {
            CobaltType = type;
        }
    }
}
