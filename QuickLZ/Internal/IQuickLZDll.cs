namespace QuickLZ
{
    using System;

    internal interface IQuickLZDll
    {
        uint Compress(byte[] source, byte[] destination, IntPtr size, byte[] scratch);
        uint Decompress(byte[] source, byte[] destination, byte[] scratch);
        uint SizeCompressed(byte[] source);
        uint SizeDecompressed(byte[] source);
        int GetSetting(int setting);
    }
}