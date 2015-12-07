using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class CompressionTool
    {
        public static byte[] GZipCompress(byte[] rawstream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GZipStream Compress = new GZipStream(ms, CompressionMode.Compress);
                Compress.Write(rawstream, 0, rawstream.Length);
                Compress.Close();
                return ms.ToArray();
            }
        }

        public static byte[] GZipDecompress(byte[] compstream)
        {
            using (MemoryStream tempMs = new MemoryStream())
            {
                using (MemoryStream ms = new MemoryStream(compstream))
                {
                    GZipStream Decompress = new GZipStream(ms, CompressionMode.Decompress);
                    Decompress.CopyTo(tempMs);
                    Decompress.Close();
                    return tempMs.ToArray();
                }
            }
        }
    }
}
