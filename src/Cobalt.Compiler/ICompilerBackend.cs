using Cobalt.AbstractSyntaxTree.Nodes;

namespace Cobalt.Compiler
{
    public interface ICompilerBackend
    {
        string GenerateTargetCode(CobaltProgram cobaltProgram);
    }
}
