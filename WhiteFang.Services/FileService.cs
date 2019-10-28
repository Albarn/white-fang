using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WhiteFang.Services
{
    public class FileService
    {
        public readonly List<List<int>> contents;
        public int lineLength;
        private StreamWriter resultWriter;
        private List<Thread> pool;

        public FileService(int lineSize = 20)
        { 
            contents = new List<List<int>>();
            pool = new List<Thread>();
        }

        public void Read(StreamReader reader)
        {
            ReadNumber(reader);
        }

        public void ReadParallel(StreamReader reader)
        {
            var task = new Thread(ReadNumber);
            pool.Add(task);
            task.Start(reader);
        }

        public void Min(string[] inputFiles, string outputFile, Action<StreamReader> readStrategy)
        {
            var readers = GetReaders(inputFiles);

            if (inputFiles.Length == 0)
            {
                return;
            }

            contents.Clear();
            pool.Clear();

            try
            {
                resultWriter = new StreamWriter(new FileStream(outputFile, FileMode.CreateNew));

                foreach (var reader in readers)
                {
                    readStrategy(reader);
                }

                foreach (var task in pool)
                {
                    task.Join();
                }

                WriteContent();
            }
            finally
            {
                resultWriter.Close();
                CloseReaders(readers);
            }
        }

        private void ReadNumber(object param)
        {
            var reader = param as StreamReader;

            var content = new List<int>();
            while (!reader.EndOfStream)
            {
                content.Add(int.Parse(reader.ReadLine()));
            }
            contents.Add(content);
        }

        private void WriteContent()
        {
            for (int i = 0; contents.All(c => c.Count > i); i++)
            {
                var min = contents
                    .Select(c => c[i])
                    .Min();

                resultWriter.WriteLine(min.ToString().PadLeft(lineLength, '0'));
            }
        }

        private static void CloseReaders(IEnumerable<StreamReader> readers)
        {
            foreach (var reader in readers)
            {
                reader.Close();
            }
        }

        private static List<StreamReader> GetReaders(IEnumerable<string> inputFiles)
        {
            return inputFiles
                .Select(file => new StreamReader(file))
                .ToList();
        }
    }
}
