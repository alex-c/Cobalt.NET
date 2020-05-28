using Cobalt.Shared;

namespace Cobalt.AbstractSyntaxTree.Nodes.Leafs.TypeKeywords
{
    public class FloatTypeKeywordNode : TypeKeywordNode
    {
        public FloatTypeKeywordNode(int sourceLine) : base (sourceLine, CobaltType.Float) { }
    }
}
