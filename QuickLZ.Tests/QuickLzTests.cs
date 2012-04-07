namespace QuickLZ.Tests
{
    using System.IO;
    using NUnit.Framework;

    [TestFixture]
    public class QuickLZTests
    {
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