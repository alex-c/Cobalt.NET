using Cobalt.AbstractSyntaxTree;

namespace Cobalt.Compiler
{
    public interface ICompilerBackend
    {
        string GenerateTargetCode(CobaltProgram cobaltProgram);
    }
}
