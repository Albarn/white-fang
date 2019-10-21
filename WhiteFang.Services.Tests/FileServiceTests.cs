using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WhiteFang.Diagnostics;

namespace WhiteFang.Services.Tests
{
    [TestFixture(64 * 1024, 3)]
    [TestFixture(1024 * 1024, 3)]
    [TestFixture(16 * 1024 * 1024, 3)]
    public class FileServiceTests
    {
        private readonly FileService sut;
        private List<string> files;
        private string outputFile;
        private readonly int fileNumber;
        private readonly int fileSize;

        public FileServiceTests(int fileSize = 1024, int fileNumber = 3)
        {
            sut = new FileService();
            files = new List<string>();
            outputFile = string.Empty;
            this.fileNumber = fileNumber;
            this.fileSize = fileSize;
        }

        private List<int> expectedFileContent;
        [SetUp]
        public void CreateFiles()
        {
            var time = DateTime.Now;

            var dir = $"logs\\{time.Year}.{time.Month}.{time.Day}\\{TestContext.CurrentContext.Test.MethodName}\\";
            Directory.CreateDirectory(dir);

            files = new List<string>();
            for(int i = 0; i < fileNumber; i++)
            {
                files.Add($"{dir}{time.Ticks}-{fileSize}-{fileNumber}#{i}");
            }
            outputFile = $"{dir}{time.Ticks}-{fileSize}-{fileNumber}";

            if (fileNumber == 0)
            {
                return;
            }

            var contents = new List<List<int>>();
            foreach(var file in files)
            {
                contents.Add(sut.Create(file, fileSize));
            }

            expectedFileContent = new List<int>();
            for(int i = 0; contents.All(c=>c.Count>i); i++)
            {
                expectedFileContent.Add(contents
                    .Select(c => c[i])
                    .Min());
            }
        }

        private PerfomanceSnapshot setUpSnapshot;
        [SetUp]
        public void LogSetUp()
        {
            setUpSnapshot = new PerfomanceSnapshot();
            setUpSnapshot.Update();
            Console.WriteLine("starting test:");
            Console.WriteLine(setUpSnapshot);
        }

        private PerfomanceSnapshot tearDownSnapshot;
        [TearDown]
        public void LogTearDown()
        {
            tearDownSnapshot = new PerfomanceSnapshot();
            tearDownSnapshot.Update();
            Console.WriteLine("test finished:");
            Console.WriteLine(tearDownSnapshot);
            Console.WriteLine("difference:");
            Console.WriteLine(tearDownSnapshot.Difference(setUpSnapshot));
        }

        [Test]
        public void Min_Any_ShouldWriteMinToFile()
        {
            // arrange

            // act
            sut.Min(outputFile, files.ToArray());
            var output = File.ReadAllText(outputFile);

            // assert
            Assert.True(expectedFileContent.SequenceEqual(output
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => int.Parse(line))));
        }

        [Test]
        public void MinParallel_Any_ShouldWriteMinToFile()
        {
            // arrange

            // act
            sut.MinParallel(outputFile, files.ToArray());
            var output = File.ReadAllText(outputFile);

            // assert
            Assert.True(expectedFileContent.SequenceEqual(output
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => int.Parse(line))));
        }
    }
}
