namespace QuickLZ.Tests
{
    using System;
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class QuickLZTests
    {
        [Test]
        public void DLLTests()
        {
            for (int i = 1; i < 3; i++)
            {
                var qlz = new QuickLZ(i);
                Assert.IsFalse(qlz.MemorySafe);
                Assert.IsFalse(qlz.Streaming);
                Assert.AreEqual(i, qlz.CompressionLevel);
                Assert.AreEqual(1, qlz.VersionMajor);
                Assert.AreEqual(5, qlz.VersionMinor);
                Assert.AreEqual(0, qlz.VersionRevision);
            }
            for (int i = 1; i < 3; i++)
            {
                var qlz = new QuickLZ(i, true);
                Assert.IsFalse(qlz.MemorySafe);
                Assert.IsTrue(qlz.Streaming);
                Assert.AreEqual(i, qlz.CompressionLevel);
                Assert.AreEqual(1, qlz.VersionMajor);
                Assert.AreEqual(5, qlz.VersionMinor);
                Assert.AreEqual(0, qlz.VersionRevision);
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BadLevelTest4()
        {
            var qlz = new QuickLZ(4);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BadLevelTest0()
        {
            var qlz = new QuickLZ(0);
        }

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
            var qlz = new QuickLZ(level);
            int sizeC = qlz.SizeCompressed(original);
            var compressedBytes = new byte[sizeC];
            qlz.Compress(original, compressedBytes, original.Length);
            var result = new byte[original.Length];
            qlz.Decompress(compressedBytes, result);
            for (int i = 0; i < original.Length; i++)
            {
                Assert.AreEqual(original[i], result[i]);
            }
        }
    }
}