namespace QuickLZ
{
    using System;
    using System.IO;
    using System.IO.Compression;

    /// <summary>  Provides methods and properties used to compress and decompress streams using QuickLZ. </summary>
    public sealed class QuickLZStream : Stream
    {
        private readonly QuickLZ _quickLZ;
        private readonly Stream _stream;
        //read
        private readonly byte[] _header = new byte[9];
        private byte[] _readBuffer;
        private byte[] _unpackedBuffer;
        private int _unpackedLength;
        private int _unpackedOffset;
        //write
        private readonly byte[] _compressedBuffer;
        private readonly byte[] _writeBuffer;
        private int _writeBufferOffset;

        /// <summary>   Initializes a new instance of the QuickLZStream class. </summary> 
        /// <param name="stream"> Stream to compress or decompress. </param>
        /// <param name="mode">   The compression mode. </param>
        /// <param name="level">  The compression level (1-3). </param>
        /// <param name="bufferSize">   (optional) size of the buffer. </param>
        public QuickLZStream(Stream stream, CompressionMode mode, int level, int bufferSize = 1 << 20)
        {
            _quickLZ = new QuickLZ(level);
            _stream = stream;
            CompressionMode = mode;
            Level = level;
            switch (mode)
            {
                case CompressionMode.Decompress:
                    Fill();
                    break;
                case CompressionMode.Compress:
                    _writeBuffer = new byte[bufferSize];
                    _compressedBuffer = new byte[bufferSize + 400];
                    break;
                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
        }

        /// <summary>   Gets the compression level (1-3). </summary>
        public int Level { get; private set; }
        /// <summary>   Gets the compression mode. </summary>
        public CompressionMode CompressionMode { get; private set; }
        public override bool CanRead
        {
            get
            {
                return CompressionMode == CompressionMode.Decompress;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
        public override bool CanWrite
        {
            get
            {
                return CompressionMode == CompressionMode.Compress;
            }
        }
        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            if (_writeBufferOffset > 0)
            {
                var compressedLength = (int)_quickLZ.Compress(_writeBuffer, _compressedBuffer, _writeBufferOffset);
                _stream.Write(_compressedBuffer, 0, compressedLength);
                _writeBufferOffset = 0;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            // we have 3 options here:
            // buffer can still be filled --> we fill
            // buffer is full --> we flush
            // buffer is overflood --> we flush and refill
            // 1. there is enough room, the buffer is not full
            int writeLength = _writeBufferOffset + count;
            if (writeLength <= _writeBuffer.Length)
            {
                Buffer.BlockCopy(buffer, offset, _writeBuffer, _writeBufferOffset, count);
                _writeBufferOffset += count;
                // 2. same size: write
                if (_writeBufferOffset == _writeBuffer.Length)
                {
                    Flush();
                }
            }
                // 3. buffer overflow: we split
            else
            {
                int lengthToCauseFlush = _writeBuffer.Length - _writeBufferOffset;
                // this first Write will cause a flush
                Write(buffer, offset, lengthToCauseFlush);
                // this one will refill
                Write(buffer, offset + lengthToCauseFlush, count - lengthToCauseFlush);
            }
        }

        private void EnsureUnpackedBuffer(byte[] packedBuffer)
        {
            var unpackedLength = (int)_quickLZ.SizeDecompressed(packedBuffer);
            if (_unpackedBuffer == null || _unpackedBuffer.Length < unpackedLength)
            {
                _unpackedBuffer = new byte[unpackedLength];
            }
        }

        private void Fill()
        {
            int headerLength = _stream.Read(_header, 0, _header.Length);
            // the normal end is here
            if (headerLength == 0)
            {
                _unpackedBuffer = null;
                return;
            }
            if (headerLength != _header.Length)
            {
                throw new InvalidDataException("QuickLZ input buffer corrupted (header)");
            }
            var sizeCompressed = (int)_quickLZ.SizeCompressed(_header);
            if (sizeCompressed == 0)
            {
                _unpackedBuffer = null;
                return;
            }
            if (_readBuffer == null || _readBuffer.Length < sizeCompressed)
            {
                _readBuffer = new byte[sizeCompressed];
            }
            Buffer.BlockCopy(_header, 0, _readBuffer, 0, _header.Length);
            int bodyLength = _stream.Read(_readBuffer, _header.Length, sizeCompressed - _header.Length);
            if (bodyLength != sizeCompressed - _header.Length)
            {
                throw new InvalidDataException("QuickLZ input buffer corrupted (body)");
            }
            EnsureUnpackedBuffer(_readBuffer);
            _unpackedLength = (int)_quickLZ.Decompress(_readBuffer, _unpackedBuffer);
            _unpackedOffset = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_unpackedBuffer == null || _unpackedOffset == _unpackedLength)
            {
                Fill();
            }
            // to do: something smarter than the double test
            if (_unpackedBuffer == null)
            {
                return 0;
            }
            // 1. If we don't have enough data available, then split
            if (_unpackedOffset + count > _unpackedLength)
            {
                int available = _unpackedLength - _unpackedOffset;
                // this is the part we're sure to get
                int r1 = Read(buffer, offset, available);
                // this is the part we're not
                int r2 = Read(buffer, offset + available, count - available);
                return r1 + r2;
            }
            // 2. we have enough buffer, use it
            Buffer.BlockCopy(_unpackedBuffer, _unpackedOffset, buffer, offset, count);
            _unpackedOffset += count;
            return count;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && CompressionMode == CompressionMode.Compress)
            {
                Flush();
            }
            base.Dispose(disposing);
            _stream.Dispose();
        }
    }
}