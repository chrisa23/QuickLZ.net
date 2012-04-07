namespace QuickLZ
{
    using System;

    /// <summary>   QuickLZ compression/decompression library wrapper </summary>
    public sealed class QuickLZ
    {
        private readonly IQuickLZDll _dll;
        private readonly byte[] _stateCompress;
        private readonly byte[] _stateDecompress;

        /// <summary>   Initializes a new instance of the QuickLZ class. </summary>
        /// <param name="level">    The level. </param>
        /// <param name="stream">   (optional) Use streaming mode </param>
        public QuickLZ(int level, bool stream = false)
        {
            _dll = GetDLL(level, stream);
            _stateCompress = new byte[_dll.GetSetting(1)];
            _stateDecompress = Streaming ? new byte[_dll.GetSetting(2)] : _stateCompress;
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
                        throw new ArgumentException("Invalid compression level");
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
                    throw new ArgumentException("Invalid compression level");
            }
        }

        /// <summary>   Gets the compression level (1-3). </summary>
        public int CompressionLevel
        {
            get
            {
                return _dll.GetSetting(0);
            }
        }
        /// <summary>   Gets the major version. </summary>
        public int VersionMajor
        {
            get
            {
                return _dll.GetSetting(7);
            }
        }
        /// <summary>   Gets the minor version. </summary>
        public int VersionMinor
        {
            get
            {
                return _dll.GetSetting(8);
            }
        }
        /// <summary>   Gets the version revision. </summary>
        public int VersionRevision
        {
            // negative means beta
            get
            {
                return _dll.GetSetting(9);
            }
        }
        /// <summary>   Gets a value indicating whether streaming mode is enabled. </summary>
        public bool Streaming
        {
            get
            {
                return _dll.GetSetting(3) == 1;
            }
        }
        /// <summary>   Gets a value indicating whether the memory safe option is on. </summary>
        public bool MemorySafe
        {
            get
            {
                return _dll.GetSetting(6) == 1;
            }
        }

        /// <summary>   Compress the source buffer. </summary>
        /// <param name="source">   Source buffer. </param>
        /// <param name="dest">     Destination buffer. </param>
        /// <param name="size">     The size of data in the source buffer. </param>
        /// <returns>  Size of compressed data </returns>
        public int Compress(byte[] source, byte[] dest, int size)
        {
            int s = _dll.Compress(source, dest, (IntPtr)size, _stateCompress);
            return s;
        }

        /// <summary>   Decompress the source buffer. </summary>
        /// <param name="source">   Source buffer. </param>
        /// <param name="dest">     Destination buffer. </param>
        /// <returns> Size of decompressed data </returns>
        public int Decompress(byte[] source, byte[] dest)
        {
            int s = _dll.Decompress(source, dest, _stateDecompress);
            return s;
        }

        /// <summary>   Gets the Size compressed of the source buffer. </summary>
        /// <param name="source">   Source buffer. </param>
        /// <returns>  Size of compressed data </returns>
        public int SizeCompressed(byte[] source)
        {
            return _dll.SizeCompressed(source);
        }

        /// <summary>   Gets the Size decompressed of the source buffer. </summary>
        /// <param name="source">   Source buffer. </param>
        ///  <returns> Size of decompressed data </returns>
        public int SizeDecompressed(byte[] source)
        {
            return _dll.SizeDecompressed(source);
        }
    }
}