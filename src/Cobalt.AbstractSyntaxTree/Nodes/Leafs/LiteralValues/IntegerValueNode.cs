﻿using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.LiteralValues
{
    public class IntegerValueNode : LiteralValueNode
    {
        public int Value { get; }

        public IntegerValueNode(int sourceLine, int value) : base(sourceLine, CobaltType.Integer)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"{GetType().Name}:{Value}";
        }
    }
}
