namespace QuickLZ.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class QuickLzTests
    {
        private Task _factory;

        [Test]
        public void Test1()
        {
            var qlz = new QuickLZ();
            //create a stream and pass it through compress and decompress
            //assert it is unchanged//
            byte[] original = File.ReadAllBytes("./Flower.bmp");
            byte[] compressedBytes; // = new byte[sizeCompressed];
            byte[] result;
            using (var compressed = new MemoryStream())
            {
                using (var lz = new QuickLZCompressionStream(compressed))
                {
                    lz.Write(original, 0, original.Length);
                }
                compressedBytes = compressed.ToArray();
            }
            result = new byte[original.Length];
            using (var compressed = new MemoryStream(compressedBytes))
            {
                using (var lzd = new QuickLZDecompressionStream(compressed))
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
        public void ParallelTest1()
        {
            var qlz = new QuickLZ();
            //create a stream and pass it through compress and decompress
            //assert it is unchanged//
            byte[] original = File.ReadAllBytes("./Flower.bmp");
            byte[] compressedBytes; // = new byte[sizeCompressed];
            using (var compressed = new MemoryStream())
            {
                using (var lz = new QuickLZCompressionStream(compressed))
                {
                    lz.Write(original, 0, original.Length);
                }
                compressedBytes = compressed.ToArray();
            }
            var factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning);
            var tasks = new List<Task>();
            for (int j = 0; j < 4; j++)
            {
                Task task = factory.StartNew(() =>
                {
                    var result = new byte[original.Length];
                    for (int k = 0; k < 100; k++)
                    {
                        using (var compressed = new MemoryStream(compressedBytes))
                        using (var lzd = new QuickLZDecompressionStream(compressed))
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