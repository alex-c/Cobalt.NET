using System.Collections.Generic;

namespace Cobalt.Compiler
{
    public class TargetProgram
    {
        public string TargetPlatform { get; }

        private ICollection<ITargetFile> Files { get; }

        public TargetProgram(string targetPlarform)
        {
            TargetPlatform = targetPlarform;
            Files = new List<ITargetFile>();
        }

        public void AddFile(ITargetFile file)
        {
            Files.Add(file);
        }

        public IEnumerable<ITargetFile> GetFiles()
        {
            return Files;
        }
    }
}
