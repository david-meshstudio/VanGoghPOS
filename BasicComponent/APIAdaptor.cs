using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class APIAdaptor
    {
        public static string URL_HEAD = "http://vangogh.sinaapp.com/app/";

        public APIAdaptor()
        {

        }

        private static string GetPostString(string parameter)
        {
            string ret = EncryptTool.encrypt(parameter, "key", "20140417161616");
            byte[] retbyte = CompressionTool.GZipCompress(Encoding.UTF8.GetBytes(ret));
            string result = BasicTool.Base64_Encode_Urlsafe(retbyte);
            return result;
        }

        private static string GetResponseString(string response)
        {
            string res = BasicTool.Base64_Decode_Urlsafe(response);
            byte[] ret = CompressionTool.GZipDecompress(Encoding.UTF8.GetBytes(res));
            string retstr = Encoding.UTF8.GetString(ret);
            string result = EncryptTool.decrypt(retstr, "key", "20140417161616");
            return result;
        }

        public static string CallAPIByFunction(string functionName, string parameter)
        {
            string result = "";
            string uri = Configuration.GetAPIURI(functionName);
            if (uri != "")
            {
                //string url = URL_HEAD + Configuration.AppName + "/" + uri;
                string url = uri;
                // Add Sign
                Dictionary<string, string> para = new Dictionary<string, string>();
                para.Add("token", Configuration.Token);
                para.Add("timestamp", BasicTool.GetCurrentTimestamp());
                para.Add("sing", "");
                parameter = GetPostString(parameter);
                para.Add("parameter", parameter);
                result = RawPostRequest(url, para);
                //result = GetResponseString(result);
            }
            return result;
        }

        public static string RawGetRequest(string url)
        {
            try
            {
                WebRequest wReq = WebRequest.Create(url);
                WebResponse wResp = wReq.GetResponse();
                Stream respStream = wResp.GetResponseStream();
                using (TextReader reader = new StreamReader(respStream, Encoding.UTF8))
                {
                    return reader.ReadToEnd().Trim((char)65279);
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }

        public static string RawPostRequest(string url, Dictionary<string, string> parameter)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "post";
                request.Accept = "text/html, application/xhtml+xml, */*";
                request.ContentType = "application/x-www-form-urlencoded";
                string strPostdata = "";
                foreach (KeyValuePair<string, string> kv in parameter)
                {
                    strPostdata += "&" + kv.Key + "=" + kv.Value;
                }
                strPostdata = strPostdata.Substring(1);
                byte[] buffer = Encoding.UTF8.GetBytes(strPostdata);
                request.ContentLength = buffer.Length;
                request.GetRequestStream().Write(buffer, 0, buffer.Length);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (TextReader reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    return reader.ReadToEnd().Trim((char)65279);
                }
            }
            catch (System.Exception ex)
            {
                //errorMsg = ex.Message;
            }
            return "";
        }
    }
}
