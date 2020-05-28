using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeKeywords
{
    public class BooleanTypeKeywordNode : TypeKeywordNode
    {
        public BooleanTypeKeywordNode(int sourceLine) : base (sourceLine, CobaltType.Boolean) { }
    }
}
