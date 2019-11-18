using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace WhiteFang.Services
{
    public class FileCalculator
    {
        public const int LineLength = 20;

        public void Min(string[] inputFiles, string outputFile, Func<FileQuery, Thread> readStrategy)
        {
            using (var writer = new StreamWriter(outputFile))
            {
                var queries = inputFiles.Select(file => new FileQuery(file)).ToList();

                var pool = queries
                    .Select(query => readStrategy(query));

                foreach (var task in pool)
                {
                    task.Join();
                }

                var contents = queries.Select(q => ReadContent(q.Reader)).ToList();
                WriteContent(writer, contents);
            }
        }

        private static List<int> ReadContent(TextReader reader)
        {
            var content = new List<int>();

            for(var line = reader.ReadLine();
                !string.IsNullOrEmpty(line);
                line = reader.ReadLine())
            {
                int.TryParse(line.Trim(), out var number);
                content.Add(number);
            }

            return content;
        }

        private static void WriteContent(TextWriter writer, IEnumerable<List<int>> contents)
        {
            for (int i = 0; contents.All(c => c.Count > i); i++)
            {
                var min = contents
                    .Select(c => c[i])
                    .Min();

                writer.WriteLine(min.ToString().PadLeft(LineLength, '0'));
            }
        }
    }
}
