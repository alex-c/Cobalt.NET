using System;
using System.IO;

namespace Cobalt.Compiler.TargetFiles
{
    public class TextFileWriter : ITargetFileWriter
    {
        public void WriteTargetFile(ITargetFile file, string filePath)
        {
            if (file is TextFile textFile)
            {
                File.WriteAllText(filePath, textFile.Contents);
            }
            else
            {
                throw new Exception("Bad file writer.");
            }
        }
    }
}
