using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qiniu.IO;
using Qiniu.RS;
using Qiniu.Util;
using System.Net;
using System.IO;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class CloudStorageTool
    {
        private static string Bucket = "vangogh";

        public static void QiniuPutFile(string fileName)
        {
            Qiniu.Conf.Config.ACCESS_KEY = "SbWsVObx9qs_V1A92TlClwQjrK9oRPgPss3BAjJV";
            Qiniu.Conf.Config.SECRET_KEY = "yzpctby_ZSeMgQOrq8IGne7rHsEwI7TWDeE30pdH";
            IOClient target = new IOClient();

            PutExtra extra = new PutExtra(); // TODO: 初始化为适当的值
            extra.MimeType = "text/plain";
            extra.Crc32 = 123;
            extra.CheckCrc = CheckCrcType.CHECK;
            extra.Params = new System.Collections.Generic.Dictionary<string, string>();
            PutPolicy put = new PutPolicy(Bucket);
            string[] fnps = fileName.Split(new char[] { '\\' });
            string key = fnps[fnps.Length - 1];
            PutRet ret = target.PutFile(put.Token(), key, fileName, extra);
        }

        public static void QiniuGetFile(string fileName)
        {
            string[] fnps = fileName.Split(new char[] { '\\' });
            string key = fnps[fnps.Length - 1];
            string path = fileName.Replace(key,"");
            //if (File.Exists(fileName)) fileName = path + "new_" + key;
            string url = @"http://7xo64k.com2.z0.glb.qiniucdn.com/" + key;
            try
            {
                WebRequest wReq = WebRequest.Create(url);
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                FileStream fs = new FileStream(fileName, FileMode.Create);
                while (true)
                {
                    int res = respStream.ReadByte();
                    if (res < 0) break;
                    fs.WriteByte((byte)res);
                }
                fs.Close();
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
        }

    }
}
