using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using WhiteFang.Threading;

namespace WhiteFang.Services
{
    public class FileReader : IDisposable
    {
        public const int ThreadCapacity = 2;

        private static readonly string readerPath;
        private static readonly FileReader instance;
        private readonly IntPtr semaphore;

        private FileReader()
        {
            semaphore = SemaphoreService.Create(Guid.NewGuid().ToString(), ThreadCapacity);
        }

        static FileReader()
        {
            instance = new FileReader();
            readerPath = typeof(FileCalculator).Assembly.Location;
            readerPath = readerPath.Substring(0, readerPath.LastIndexOf('\\') + 1);
            readerPath += "WhiteFang.Reader.exe";
        }

        public static Thread Read(FileQuery query)
        {
            ReadText(query);
            var completed = new Thread(() => { });
            completed.Start();
            return completed;
        }

        public static Thread ReadParallel(FileQuery query)
        {
            var task = new Thread(ReadText);
            task.Start(query);
            return task;
        }

        public static Thread ReadSynchronized(FileQuery query)
        {
            SemaphoreService.Wait(instance.semaphore);
            var task = new Thread(ReadText);
            task.Start(query);
            return task;
        }

        public static Thread ReadInProcess(FileQuery query)
        {
            var task = new Thread(ReadTextInProcess);
            task.Start(query);
            return task;
        }

        private static void ReadText(object param)
        {
            var query = param as FileQuery;

            using (var reader = new StreamReader(query.Path))
            {
                while (!reader.EndOfStream)
                {
                    query.Output.Add(reader.ReadLine());
                }
            }

            SemaphoreService.Release(instance.semaphore);
        }

        private static void ReadTextInProcess(object param)
        {
            var query = param as FileQuery;
            var slot = Guid.NewGuid().ToString();
            MailSlotService.Create(slot);

            var file = query.Path;
            var readerProc = new ProcessStartInfo(readerPath, $"{file} {slot}")
            {
                UseShellExecute = false
            };
            var proc = Process.Start(readerProc);

            bool exit = false;
            while (true)
            {
                exit = proc.HasExited;
                var line = MailSlotService.Read(slot);
                if (!string.IsNullOrEmpty(line))
                {
                    query.Output.Add(line);
                } else if (exit)
                {
                    break;
                }
            }

            MailSlotService.Close(slot);
        }

        public void Dispose()
        {
            SemaphoreService.Close(semaphore);
        }
    }
}
