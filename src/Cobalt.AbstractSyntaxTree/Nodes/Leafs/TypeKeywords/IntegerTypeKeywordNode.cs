using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeKeywords
{
    public class IntegerTypeKeywordNode : TypeKeywordNode
    {
        public IntegerTypeKeywordNode(int sourceLine) : base (sourceLine, CobaltType.Integer) { }
    }
}
