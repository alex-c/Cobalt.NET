namespace Cobalt.Compiler
{
    public interface ITargetFile
    {
        string Name { get; }

        ITargetFileWriter Writer { get; } 
    }
}
