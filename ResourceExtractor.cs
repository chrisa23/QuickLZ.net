using System.IO;
using QuickLZ.Properties;

namespace QuickLZ
{
    public static class ResourceExtractor
    {
        public static void ExtractResourceToFile(bool is64, string filename)
        {
            if (!File.Exists(filename))
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] b = is64? Resources.quicklz150_64_1_1000000:Resources.quicklz150_32_1_1000000;
                    fs.Write(b, 0, b.Length);
                }
            }
        }
    }
}