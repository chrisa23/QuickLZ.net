namespace QuickLZ
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class QuickLZ3SDll : IQuickLZDll
    {
        public QuickLZ3SDll()
        {
            ResourceExtractor.ExtractResourceToFile(3, true, "quicklz150_3s.dll");
        }

        private const string DllLocation = "quicklz150_3s.dll";

        [DllImport(DllLocation)]
        private static extern IntPtr qlz_compress(byte[] source, byte[] destination, IntPtr size, byte[] scratch);

        [DllImport(DllLocation)]
        private static extern IntPtr qlz_decompress(byte[] source, byte[] destination, byte[] scratch);

        [DllImport(DllLocation)]
        private static extern IntPtr qlz_size_compressed(byte[] source);

        [DllImport(DllLocation)]
        private static extern IntPtr qlz_size_decompressed(byte[] source);

        [DllImport(DllLocation)]
        private static extern int qlz_get_setting(int setting);

        int IQuickLZDll.Decompress(byte[] source, byte[] destination, byte[] scratch)
        {
            return qlz_decompress(source, destination, scratch).ToInt32();
        }

        int IQuickLZDll.SizeCompressed(byte[] source)
        {
            return qlz_size_compressed(source).ToInt32();
        }

        int IQuickLZDll.SizeDecompressed(byte[] source)
        {
            return qlz_size_decompressed(source).ToInt32();
        }

        int IQuickLZDll.GetSetting(int setting)
        {
            return qlz_get_setting(setting);
        }

        int IQuickLZDll.Compress(byte[] source, byte[] destination, IntPtr size, byte[] scratch)
        {
            return qlz_compress(source, destination, size, scratch).ToInt32();
        }
    }
}