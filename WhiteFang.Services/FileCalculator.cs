using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
                var contents = new List<List<int>>();
                for (int i = 0; i < inputFiles.Length; i++)
                {
                    int pos = i;
                    contents.Add(new List<int>());
                    queries[pos].Output.CollectionChanged += (sender, args) =>
                    {
                        if (args.Action != NotifyCollectionChangedAction.Add)
                        {
                            return;
                        }
                        var items = args.NewItems;
                        foreach (var item in items)
                        {
                            int.TryParse(item.ToString(), out var num);
                            contents[pos].Add(num);
                        }
                        queries[pos].Output.Clear();
                    };
                }

                var pool = queries.Select(query => readStrategy(query)).ToList();


                foreach (var task in pool)
                {
                    task.Join();
                }

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
