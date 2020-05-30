using Cobalt.AbstractSyntaxTree.Nodes;

namespace Cobalt.Compiler
{
    public interface ITargetCodeGenerator
    {
        TargetProgram GenerateTargetCode(CobaltProgram cobaltProgram);
    }
}
