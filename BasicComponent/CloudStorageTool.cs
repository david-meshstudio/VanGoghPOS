using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qiniu.IO;
using Qiniu.RS;
using Qiniu.Util;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class CloudStorageTool
    {
        protected static string Bucket = "vangogh";

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

    }
}
