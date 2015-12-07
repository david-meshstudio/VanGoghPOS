using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class FileAdaptor
    {
        public static string ReadFile(string fileName)
        {
            string result = "";
            FileStream fs = new FileStream(fileName, FileMode.Open);
            byte[] resbyte = new byte[fs.Length];
            fs.Read(resbyte, 0, (int)fs.Length);
            resbyte = CompressionTool.GZipDecompress(resbyte);
            result = EncryptTool.decrypt(Encoding.UTF8.GetString(resbyte), "key", "20151207155900");
            fs.Close();
            return result;
        }

        public static void WriteFile(string fileName,string contentString)
        {
            string result = EncryptTool.encrypt(contentString, "key", "20151207155900");
            byte[] resbyte = CompressionTool.GZipCompress(Encoding.UTF8.GetBytes(result));
            FileStream fs = new FileStream(fileName, FileMode.Create);
            fs.Write(resbyte, 0, resbyte.Length);
            fs.Close();
        }
    }
}
