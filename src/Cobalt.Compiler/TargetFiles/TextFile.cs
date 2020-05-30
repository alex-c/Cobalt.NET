namespace Cobalt.Compiler.TargetFiles
{
    public class TextFile : ITargetFile
    {
        public string Name { get; }

        public ITargetFileWriter Writer { get; }

        public string Contents { get; }

        public TextFile(string name, string contents)
        {
            Name = name;
            Contents = contents;
            Writer = new TextFileWriter();
        }
    }
}
