using Cobalt.AbstractSyntaxTree.Nodes;

namespace Cobalt.Compiler
{
    public interface ITargetCodeGenerator
    {
        string Platform { get; }

        TargetProgram GenerateTargetCode(CobaltProgram cobaltProgram);
    }
}
