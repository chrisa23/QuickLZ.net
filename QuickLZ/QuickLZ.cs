namespace QuickLZ
{
    using System;

    internal sealed class QuickLZ
    {
        // The C library passes many integers through the C type size_t which is 32 or 64 bits on 32 or 64 bit 
        // systems respectively. The C# type IntPtr has the same property but because IntPtr doesn't allow 
        // arithmetic we cast to and from int on each reference. To pass constants use (IntPrt)1234.
        private readonly byte[] _stateCompress;
        private readonly byte[] _stateDecompress;
        private readonly IQuickLZDll _dll;

        public QuickLZ(int level)
        {
            _dll = GetDLL(level);
            _stateCompress = new byte[_dll.GetSetting(1)];
            _stateDecompress = QLZ_STREAMING_BUFFER == 0 ? _stateCompress : new byte[_dll.GetSetting(2)];
        }

        private IQuickLZDll GetDLL(int level)
        {
            switch (level)
            {
                case 1:
                    return new QuickLZ1Dll();
                case 2:
                    return new QuickLZ2Dll();
                case 3:
                    return new QuickLZ3Dll();
                default:
                    return new QuickLZ1Dll();
            }
        }

        public int QLZ_COMPRESSION_LEVEL
        {
            get
            {
                return _dll.GetSetting(0);
            }
        }
        public uint QLZ_SCRATCH_COMPRESS
        {
            get
            {
                return (uint)_dll.GetSetting(1);
            }
        }
        public uint QLZ_SCRATCH_DECOMPRESS
        {
            get
            {
                return (uint)_dll.GetSetting(2);
            }
        }
        public int QLZ_VERSION_MAJOR
        {
            get
            {
                return _dll.GetSetting(7);
            }
        }
        public int QLZ_VERSION_MINOR
        {
            get
            {
                return _dll.GetSetting(8);
            }
        }
        public int QLZ_VERSION_REVISION
        {
            // negative means beta
            get
            {
                return _dll.GetSetting(9);
            }
        }
        public uint QLZ_STREAMING_BUFFER
        {
            get
            {
                return (uint)_dll.GetSetting(3);
            }
        }
        public bool QLZ_MEMORY_SAFE
        {
            get
            {
                return _dll.GetSetting(6) == 1 ? true : false;
            }
        }

        public uint Compress(byte[] source, byte[] dest, int size)
        {
            uint s = _dll.Compress(source, dest, (IntPtr)size, _stateCompress);
            return s;
        }

        public uint Decompress(byte[] source, byte[] dest)
        {
            uint s = _dll.Decompress(source, dest, _stateDecompress);
            return s;
        }

        public uint SizeCompressed(byte[] source)
        {
            return _dll.SizeCompressed(source);
        }

        public uint SizeDecompressed(byte[] source)
        {
            return _dll.SizeDecompressed(source);
        }
    }
}