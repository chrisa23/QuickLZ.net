namespace QuickLZ.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class QuickLZStreamTests
    {
        private static readonly TaskFactory Factory = new TaskFactory(TaskCreationOptions.LongRunning,
            TaskContinuationOptions.LongRunning);

        [Test]
        public void GeneralCompressDecompressTest()
        {
            RunGeneralTest(1);
            RunGeneralTest(2);
            RunGeneralTest(3);
        }

        private static void RunGeneralTest(int level)
        {
            byte[] original = File.ReadAllBytes("./Flower.bmp");
            byte[] compressedBytes;
            using (var compressed = new MemoryStream())
            {
                using (var lz = new QuickLZStream(compressed, CompressionMode.Compress, level))
                {
                    lz.Write(original, 0, original.Length);
                }
                compressedBytes = compressed.ToArray();
            }
            var result = new byte[original.Length];
            using (var compressed = new MemoryStream(compressedBytes))
            {
                using (var lzd = new QuickLZStream(compressed, CompressionMode.Decompress, level))
                {
                    lzd.Read(result, 0, result.Length);
                }
            }
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], result[i]);
            }
        }

        [Test]
        public void ParallelTests()
        {
            RunParallelTest(1);
            RunParallelTest(2);
            RunParallelTest(3);
        }

        private static void RunParallelTest(int level)
        {
            byte[] original = File.ReadAllBytes("./Flower.bmp");
            byte[] compressedBytes;
            using (var compressed = new MemoryStream())
            {
                using (var lz = new QuickLZStream(compressed, CompressionMode.Compress, level))
                {
                    lz.Write(original, 0, original.Length);
                }
                compressedBytes = compressed.ToArray();
            }
            var tasks = new List<Task>();
            for (int j = 0; j < 4; j++)
            {
                Task task = Factory.StartNew(() =>
                {
                    var result = new byte[original.Length];
                    for (int k = 0; k < 100; k++)
                    {
                        using (var compressed = new MemoryStream(compressedBytes))
                        using (var lzd = new QuickLZStream(compressed, CompressionMode.Decompress, level))
                        {
                            lzd.Read(result, 0, result.Length);
                        }
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}