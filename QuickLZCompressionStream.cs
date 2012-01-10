using System;
using System.IO;

namespace QuickLZ
{
    public sealed class QuickLZCompressionStream : Stream
    {
        private readonly byte[] _compressedBuffer;
        private readonly QuickLZ _quickLZ;
        private readonly Stream _targetStream;
        private readonly byte[] _writeBuffer;
        private int _writeBufferOffset;

        public QuickLZCompressionStream(Stream targetStream, byte[] writeBuffer, byte[] compressionBuffer)
        {
            _quickLZ = new QuickLZ();
            _targetStream = targetStream;
            _writeBuffer = writeBuffer;
            _compressedBuffer = compressionBuffer;
        }

        public QuickLZCompressionStream(Stream targetStream, int bufferSize = 1 << 20)
            : this(targetStream, new byte[bufferSize], new byte[bufferSize + 400])
        {
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }


        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }


        public override void Flush()
        {
            if (_writeBufferOffset > 0)
            {
                var compressedLength = (int) _quickLZ.Compress(_writeBuffer, _compressedBuffer, _writeBufferOffset);
                _targetStream.Write(_compressedBuffer, 0, compressedLength);
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
                    Flush();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Flush();
                //_quickLZ.Dispose();
            }
            base.Dispose(disposing);
            _targetStream.Dispose();
        }
    }
}