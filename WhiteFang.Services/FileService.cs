using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WhiteFang.Services
{
    public class FileService
    {
        public List<int> Create(string name, int size)
        {
            var content = new List<int>();
            using (var writer = new StreamWriter(new FileStream(name, FileMode.Create)))
            {
                var f = new Random((int)DateTime.Now.Ticks);

                while(writer.BaseStream.Length < size)
                {
                    var num = f.Next();
                    content.Add(num);
                    writer.WriteLine(num);
                }
            }

            return content;
        }

        public void Min(string outputFile, params string[] inputFiles)
        {
            var readers = inputFiles
                .Select(file => new StreamReader(file))
                .ToList();

            using (var writer = new StreamWriter(new FileStream(outputFile, FileMode.CreateNew)))
            {
                if (inputFiles.Length == 0)
                {
                    return;
                }

                try
                {
                    while (!readers.Any(r => r.EndOfStream))
                    {
                        var min = readers
                            .Select(r => int.Parse(r.ReadLine()))
                            .ToList()
                            .Min();

                        writer.WriteLine(min);
                    }
                }
                finally
                {
                    foreach(var reader in readers)
                    {
                        reader.Close();
                    }
                }
            }
        }
    }
}
