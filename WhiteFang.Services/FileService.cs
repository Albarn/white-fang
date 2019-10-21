using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

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
            var readers = GetReaders(inputFiles);

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
                            .Min();

                        writer.WriteLine(min);
                    }
                }
                finally
                {
                    CloseReaders(readers);
                }
            }
        }

        public void MinParallel(string outputFile, params string[] inputFiles)
        {
            var readers = GetReaders(inputFiles);

            using (var writer = new StreamWriter(new FileStream(outputFile, FileMode.CreateNew)))
            {
                if (inputFiles.Length == 0)
                {
                    return;
                }

                try
                {
                    var contents = new List<List<int>>();
                    var pool = new List<Thread>();
                    foreach(var reader in readers)
                    {
                        CreateReadTask(contents, pool, reader);
                    }

                    foreach (var task in pool)
                    {
                        task.Join();
                    }

                    for (int i = 0; contents.All(c => c.Count > i); i++)
                    {
                        var min = contents
                            .Select(c => c[i])
                            .Min();

                        writer.WriteLine(min);
                    }
                }
                finally
                {
                    CloseReaders(readers);
                }
            }
        }

        private static void CreateReadTask(List<List<int>> contents, List<Thread> pool, StreamReader reader)
        {
            var readTask = new Thread(() =>
            {
                var content = new List<int>();
                while (!reader.EndOfStream)
                {
                    content.Add(int.Parse(reader.ReadLine()));
                }
                contents.Add(content);
            });
            pool.Add(readTask);
            readTask.Start();
        }

        private static void CloseReaders(List<StreamReader> readers)
        {
            foreach (var reader in readers)
            {
                reader.Close();
            }
        }

        private static List<StreamReader> GetReaders(string[] inputFiles)
        {
            return inputFiles
                .Select(file => new StreamReader(file))
                .ToList();
        }
    }
}
