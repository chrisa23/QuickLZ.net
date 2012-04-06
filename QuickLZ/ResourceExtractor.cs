namespace QuickLZ
{
    using System.IO;
    using global::QuickLZ.Properties;

    public static class ResourceExtractor
    {
        public static void ExtractResourceToFile(bool is64BitProcess, string filename)
        {
            if (!File.Exists(filename))
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] b = is64BitProcess ? Resources.quicklz150_64_1_1000000 : Resources.quicklz150_32_1_1000000;
                    fs.Write(b, 0, b.Length);
                }
            }
        }
    }
}