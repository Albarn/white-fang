﻿using NUnit.Framework;
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
        private readonly int fileSize;
        private readonly int fileNumber;

        public FileServiceTests(int fileSize = 1024, int fileNumber = 3)
        {
            sut = new FileService();
            this.fileSize = fileSize;
            this.fileNumber = fileNumber;
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
            CreateFiles(out var inputFiles, out var outputFile, out List<int> min);

            // act
            sut.Min(inputFiles, outputFile, sut.Read);
            var output = File.ReadAllText(outputFile);

            // assert
            Assert.True(min.SequenceEqual(output
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => int.Parse(line))));
        }

        [Test]
        public void MinParallel_Any_ShouldWriteMinToFile()
        {
            // arrange
            CreateFiles(out var inputs, out var outputFile, out List<int> min);

            // act
            sut.Min(inputs, outputFile, sut.ReadParallel);
            var output = File.ReadAllText(outputFile);

            // assert
            Assert.True(min.SequenceEqual(output
                .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => int.Parse(line))));
        }

        private void CreateFiles(out string[] inputFiles, out string outputFile, out List<int> min)
        {
            var time = DateTime.Now;

            var dir = $"logs\\{time.Year}.{time.Month}.{time.Day}\\{TestContext.CurrentContext.Test.MethodName}\\";
            Directory.CreateDirectory(dir);

            inputFiles = new string[fileNumber];
            min = new List<int>();
            for (int i = 0; i < fileNumber; i++)
            {
                inputFiles[i] = $"{dir}{time.Ticks}-{fileSize}-{fileNumber}#{i}";
                CreateInputFile(inputFiles[i], min);
            }
            outputFile = $"{dir}{time.Ticks}-{fileSize}-{fileNumber}";
        }

        private void CreateInputFile(string fileName, List<int> min)
        {
            using (var writer = new StreamWriter(new FileStream(fileName, FileMode.Create)))
            {
                var f = new Random((int)DateTime.Now.Ticks);

                for (int i = 0; writer.BaseStream.Length < fileSize; i++)
                {
                    var num = f.Next();
                    if (min.Count <= i)
                    {
                        min.Add(num);
                    }
                    min[i] = Math.Min(min[i], num);
                    writer.WriteLine(num.ToString().PadLeft(20, '0'));
                }
            }
        }
    }
}
