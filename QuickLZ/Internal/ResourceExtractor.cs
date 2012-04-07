namespace QuickLZ
{
    using System;
    using System.IO;
    using global::QuickLZ.Properties;

    internal static class ResourceExtractor
    {
        public static void ExtractResourceToFile(int level, bool stream, string filename)
        {
            if (!File.Exists(filename))
            {
                using (var fs = new FileStream(filename, FileMode.Create))
                {
                    byte[] b;
                    if (Environment.Is64BitProcess)
                    {
                        if (stream)
                        {
                            switch (level)
                            {
                                case 1:
                                    b = Resources.quicklz150_64_1_1000000;
                                    break;
                                case 2:
                                    b = Resources.quicklz150_64_2_1000000;
                                    break;
                                case 3:
                                    b = Resources.quicklz150_64_3_1000000;
                                    break;
                                default:
                                    b = Resources.quicklz150_64_1_1000000;
                                    break;
                            }
                        }
                        else
                        {
                            switch (level)
                            {
                                case 1:
                                    b = Resources.quicklz150_64_1;
                                    break;
                                case 2:
                                    b = Resources.quicklz150_64_2;
                                    break;
                                case 3:
                                    b = Resources.quicklz150_64_3;
                                    break;
                                default:
                                    b = Resources.quicklz150_64_1;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (stream)
                        {
                            switch (level)
                            {
                                case 1:
                                    b = Resources.quicklz150_32_1_1000000;
                                    break;
                                case 2:
                                    b = Resources.quicklz150_32_2_1000000;
                                    break;
                                case 3:
                                    b = Resources.quicklz150_32_3_1000000;
                                    break;
                                default:
                                    b = Resources.quicklz150_32_1_1000000;
                                    break;
                            }
                        }
                        else
                        {
                            switch (level)
                            {
                                case 1:
                                    b = Resources.quicklz150_32_1;
                                    break;
                                case 2:
                                    b = Resources.quicklz150_32_2;
                                    break;
                                case 3:
                                    b = Resources.quicklz150_32_3;
                                    break;
                                default:
                                    b = Resources.quicklz150_32_1;
                                    break;
                            }
                        }
                    }
                    fs.Write(b, 0, b.Length);
                }
            }
        }
    }
}