using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using WhiteFang.Threading;

namespace WhiteFang.Services
{
    public class FileCalculator : IDisposable
    {
        public readonly List<List<int>> contents;
        public readonly int lineLength;
        private readonly List<Thread> pool;
        private readonly IntPtr semaphore;
        private StreamWriter resultWriter;

        public FileCalculator(int threadCapacity = 2, int lineLength = 20)
        {
            contents = new List<List<int>>();
            pool = new List<Thread>();

            this.lineLength = lineLength;
            semaphore = SemaphoreService.Create(Guid.NewGuid().ToString(), threadCapacity);
        }

        public void Read(StreamReader reader)
        {
            ReadNumbers(reader);
        }

        public void ReadParallel(StreamReader reader)
        {
            var task = new Thread(ReadNumbers);
            pool.Add(task);
            task.Start(reader);
        }

        public void ReadSynchronized(StreamReader reader)
        {
            SemaphoreService.Wait(semaphore);
            var task = new Thread(ReadNumbers);
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

        private void ReadNumbers(object param)
        {
            var reader = param as StreamReader;

            var content = new List<int>();
            while (!reader.EndOfStream)
            {
                content.Add(int.Parse(reader.ReadLine()));
            }
            contents.Add(content);
            SemaphoreService.Release(semaphore);
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

        public void Dispose()
        {
            SemaphoreService.Close(semaphore);
        }
    }
}
