using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class BasicTool
    {
        public static Dictionary<string, string> CloneMap(Dictionary<string, string> map)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> kv in map)
            {
                result.Add(kv.Key, kv.Value);
            }
            return result;
        }

        public static string GetCurrentTimestamp()
        {
            long intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (long)(DateTime.Now - startTime).TotalSeconds;
            return intResult.ToString();
        }

        public static long GetCurrentTimestampLong()
        {
            long intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (long)(DateTime.Now - startTime).TotalSeconds;
            return intResult;
        }

        public static string Base64_Encode(string str)
        {
            byte[] bytedata = Encoding.UTF8.GetBytes(str);
            string strPath = Convert.ToBase64String(bytedata);
            return strPath;
        }

        public static string Base64_Encode(byte[] bytedata)
        {
            string strPath = Convert.ToBase64String(bytedata);
            return strPath;
        }

        public static string Base64_Encode_Urlsafe(string str)
        {
            string result = Base64_Encode(str);
            result = result.Replace('+', '-');
            result = result.Replace('/', '_');
            return result;
        }

        public static string Base64_Encode_Urlsafe(byte[] bytedata)
        {
            string result = Base64_Encode(bytedata);
            result = result.Replace('+', '-');
            result = result.Replace('/', '_');
            return result;
        }

        public static string Base64_Decode(string str)
        {
            byte[] outputb = Convert.FromBase64String(str);
            string orgStr = Encoding.UTF8.GetString(outputb);
            return orgStr;
        }

        public static string Base64_Decode(byte[] bytedata)
        {
            string orgStr = Encoding.UTF8.GetString(bytedata);
            return orgStr;
        }

        public static string Base64_Decode_Urlsafe(string str)
        {
            str = str.Replace('-', '+');
            str = str.Replace('/', '_');
            return Base64_Decode(str);
        }

        public static string Base64_Decode_Urlsafe(byte[] bytedata)
        {
            string str = Encoding.UTF8.GetString(bytedata);
            str = str.Replace('-', '+');
            str = str.Replace('/', '_');
            return Base64_Decode(bytedata);
        }
    }
}
