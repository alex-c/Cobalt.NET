namespace Cobalt.Compiler
{
    public interface ITargetFileWriter 
    {
        void WriteTargetFile(ITargetFile file, string filePath);
    }
}
