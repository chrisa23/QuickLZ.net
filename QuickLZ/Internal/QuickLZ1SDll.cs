namespace QuickLZ
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class QuickLZ1SDll : IQuickLZDll
    {
        public QuickLZ1SDll()
        {
            ResourceExtractor.ExtractResourceToFile(1, true, "quicklz150_1s.dll");
        }

        private const string DllLocation = "quicklz150_1s.dll";

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

        uint IQuickLZDll.Decompress(byte[] source, byte[] destination, byte[] scratch)
        {
            return (uint)qlz_decompress(source, destination, scratch);
        }

        uint IQuickLZDll.SizeCompressed(byte[] source)
        {
            return (uint)qlz_size_compressed(source);
        }

        uint IQuickLZDll.SizeDecompressed(byte[] source)
        {
            return (uint)qlz_size_decompressed(source);
        }

        int IQuickLZDll.GetSetting(int setting)
        {
            return qlz_get_setting(setting);
        }

        uint IQuickLZDll.Compress(byte[] source, byte[] destination, IntPtr size, byte[] scratch)
        {
            return (uint)qlz_compress(source, destination, size, scratch);
        }
    }
}