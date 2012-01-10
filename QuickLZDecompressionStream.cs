using System;
using System.IO;

namespace QuickLZ
{
    public sealed class QuickLZDecompressionStream : Stream
    {
        private readonly byte[] _header = new byte[9];
        private readonly QuickLZ _quickLZ;
        private readonly Stream _sourceStream;
        private byte[] _readBuffer;
        private byte[] _unpackedBuffer;
        private int _unpackedLength;
        private int _unpackedOffset;

        public QuickLZDecompressionStream(Stream sourceStream)
        {
            _quickLZ = new QuickLZ();
            _sourceStream = sourceStream;
            Fill();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
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

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        private byte[] Alloc(int length)
        {
            return new byte[length];
        }

        private void Free(byte[] data)
        {
        }

        private void EnsureUnpackedBuffer(byte[] packedBuffer)
        {
            var unpackedLength = (int) _quickLZ.SizeDecompressed(packedBuffer);
            if (_unpackedBuffer == null || _unpackedBuffer.Length < unpackedLength)
            {
                if (_unpackedBuffer != null)
                    Free(_unpackedBuffer);
                _unpackedBuffer = Alloc(unpackedLength);
            }
        }

        private void Fill()
        {
            int headerLength = _sourceStream.Read(_header, 0, _header.Length);
            // the normal end is here
            if (headerLength == 0)
            {
                _unpackedBuffer = null;
                return;
            }
            if (headerLength != _header.Length)
                throw new InvalidDataException("QuickLZ input buffer corrupted (header)");

            var sizeCompressed = (int) _quickLZ.SizeCompressed(_header);

            if (sizeCompressed == 0)
            {
                _unpackedBuffer = null;
                return;
            }
            if (_readBuffer == null || _readBuffer.Length < sizeCompressed)
            {
                if (_readBuffer != null)
                    Free(_readBuffer);
                _readBuffer = Alloc(sizeCompressed);
            }
            Buffer.BlockCopy(_header, 0, _readBuffer, 0, _header.Length);
            int bodyLength = _sourceStream.Read(_readBuffer, _header.Length, sizeCompressed - _header.Length);
            if (bodyLength != sizeCompressed - _header.Length)
                throw new InvalidDataException("QuickLZ input buffer corrupted (body)");
            EnsureUnpackedBuffer(_readBuffer);
            _unpackedLength = (int) _quickLZ.Decompress(_readBuffer, _unpackedBuffer);
            _unpackedOffset = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_unpackedBuffer == null || _unpackedOffset == _unpackedLength)
                Fill();

            // to do: something smarter than the double test
            if (_unpackedBuffer == null)
                return 0;

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
            base.Dispose(disposing);
            _sourceStream.Dispose();
        }
    }
}