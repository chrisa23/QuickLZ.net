namespace QuickLZ
{
    using System;

    public sealed class QuickLZ
    {
        // The C library passes many integers through the C type size_t which is 32 or 64 bits on 32 or 64 bit 
        // systems respectively. The C# type IntPtr has the same property but because IntPtr doesn't allow 
        // arithmetic we cast to and from int on each reference. To pass constants use (IntPrt)1234.
        private readonly byte[] _stateCompress;
        private readonly byte[] _stateDecompress;
        private readonly IQuickLZDll _dll;

        public QuickLZ(int level, bool stream = false)
        {
            _dll = GetDLL(level, stream);
            _stateCompress = new byte[_dll.GetSetting(1)];
            _stateDecompress = StreamingBuffer ? new byte[_dll.GetSetting(2)] : _stateCompress;
        }

        private IQuickLZDll GetDLL(int level, bool stream)
        {
            if (stream)
            {
                switch (level)
                {
                    case 1:
                        return new QuickLZ1SDll();
                    case 2:
                        return new QuickLZ2SDll();
                    case 3:
                        return new QuickLZ3SDll();
                    default:
                        return new QuickLZ1SDll();
                }
            }
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

        public int CompressionLevel
        {
            get
            {
                return _dll.GetSetting(0);
            }
        }
        public int ScratchCompress
        {
            get
            {
                return _dll.GetSetting(1);
            }
        }
        public int ScratchDecompress
        {
            get
            {
                return _dll.GetSetting(2);
            }
        }
        public int VersionMajor
        {
            get
            {
                return _dll.GetSetting(7);
            }
        }
        public int VersionMinor
        {
            get
            {
                return _dll.GetSetting(8);
            }
        }
        public int VersionRevision
        {
            // negative means beta
            get
            {
                return _dll.GetSetting(9);
            }
        }
        public bool StreamingBuffer
        {
            get
            {
                return _dll.GetSetting(3) == 1;
            }
        }
        public bool MemorySafe
        {
            get
            {
                return _dll.GetSetting(6) == 1;
            }
        }

        public int Compress(byte[] source, byte[] dest, int size)
        {
            uint s = _dll.Compress(source, dest, (IntPtr)size, _stateCompress);
            return (int)s;
        }

        public int Decompress(byte[] source, byte[] dest)
        {
            uint s = _dll.Decompress(source, dest, _stateDecompress);
            return (int)s;
        }

        public int SizeCompressed(byte[] source)
        {
            return (int)_dll.SizeCompressed(source);
        }

        public int SizeDecompressed(byte[] source)
        {
            return (int)_dll.SizeDecompressed(source);
        }
    }
}