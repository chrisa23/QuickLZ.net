namespace QuickLZ
{
    using System;

    internal interface IQuickLZDll
    {
        int Compress(byte[] source, byte[] destination, IntPtr size, byte[] scratch);
        int Decompress(byte[] source, byte[] destination, byte[] scratch);
        int SizeCompressed(byte[] source);
        int SizeDecompressed(byte[] source);
        int GetSetting(int setting);
    }
}