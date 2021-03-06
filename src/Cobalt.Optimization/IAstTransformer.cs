﻿using Cobalt.AbstractSyntaxTree.Nodes;

namespace Cobalt.Optimization
{
    public interface IAstTransformer<NodeType> where NodeType : AstNode
    {
        AstNode Transform(NodeType ast);
    }
}
