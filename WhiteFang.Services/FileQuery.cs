using System;
using System.IO;

namespace WhiteFang.Services
{
    public class FileQuery : IDisposable
    {
        public string Path { get; set; }
        public readonly StreamReader Reader;
        public readonly StreamWriter Writer;
        private readonly MemoryStream output;

        public FileQuery(string path)
        {
            Path = path;
            output = new MemoryStream();
            Reader = new StreamReader(output);
            Writer = new StreamWriter(output);
        }

        public void Dispose()
        {
            output.Close();
        }
    }
}
