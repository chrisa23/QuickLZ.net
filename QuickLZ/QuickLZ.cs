namespace QuickLZ
{
    using System;
    using System.Runtime.InteropServices;

    public class QuickLZ
    {
        // The C library passes many integers through the C type size_t which is 32 or 64 bits on 32 or 64 bit 
        // systems respectively. The C# type IntPtr has the same property but because IntPtr doesn't allow 
        // arithmetic we cast to and from int on each reference. To pass constants use (IntPrt)1234.
        private const string DllLocation = "quicklz150.dll";
        private readonly byte[] _stateCompress;
        private readonly byte[] _stateDecompress;

        public QuickLZ()
        {
            ResourceExtractor.ExtractResourceToFile(Environment.Is64BitProcess, "quicklz150.dll");
            _stateCompress = new byte[qlz_get_setting(1)];
            _stateDecompress = QLZ_STREAMING_BUFFER == 0 ? _stateCompress : new byte[qlz_get_setting(2)];
        }

        public uint QLZ_COMPRESSION_LEVEL
        {
            get
            {
                return (uint)qlz_get_setting(0);
            }
        }
        public uint QLZ_SCRATCH_COMPRESS
        {
            get
            {
                return (uint)qlz_get_setting(1);
            }
        }
        public uint QLZ_SCRATCH_DECOMPRESS
        {
            get
            {
                return (uint)qlz_get_setting(2);
            }
        }
        public uint QLZ_VERSION_MAJOR
        {
            get
            {
                return (uint)qlz_get_setting(7);
            }
        }
        public uint QLZ_VERSION_MINOR
        {
            get
            {
                return (uint)qlz_get_setting(8);
            }
        }
        public int QLZ_VERSION_REVISION
        {
            // negative means beta
            get
            {
                return qlz_get_setting(9);
            }
        }
        public uint QLZ_STREAMING_BUFFER
        {
            get
            {
                return (uint)qlz_get_setting(3);
            }
        }
        public bool QLZ_MEMORY_SAFE
        {
            get
            {
                return qlz_get_setting(6) == 1 ? true : false;
            }
        }

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

        public uint Compress(byte[] source, byte[] dest, int size)
        {
            var s = (uint)qlz_compress(source, dest, (IntPtr)size, _stateCompress);
            return s;
        }

        public uint Decompress(byte[] source, byte[] dest)
        {
            var s = (uint)qlz_decompress(source, dest, _stateDecompress);
            return s;
        }

        public uint SizeCompressed(byte[] source)
        {
            return (uint)qlz_size_compressed(source);
        }

        public uint SizeDecompressed(byte[] source)
        {
            return (uint)qlz_size_decompressed(source);
        }
    }
}