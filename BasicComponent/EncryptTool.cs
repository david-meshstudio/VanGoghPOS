using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class EncryptTool
    {        
        #region SHA1
        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string EncryptBySHA1(string input)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] bytes = Encoding.Unicode.GetBytes(input);
            byte[] result = sha.ComputeHash(bytes);
            return BitConverter.ToString(result);
        }

        /// <summary>
        /// hmacSha1加密
        /// </summary>
        /// <param name="encryptText">加密明文</param>
        /// <param name="encryptKey">加密秘钥</param>
        /// <returns></returns>
        public static string hmacSha1(string encryptText, string SecretKey)
        {
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Encoding.Default.GetBytes(SecretKey));
            byte[] RstRes = myHMACSHA1.ComputeHash(Encoding.Default.GetBytes(encryptText));
            StringBuilder EnText = new StringBuilder();
            foreach (byte Byte in RstRes)
            {
                EnText.AppendFormat("{0:x2}", Byte);
            }
            return EnText.ToString();
        }
        #endregion

        public static string get_md5(string inStr)
        {
            string result = "";
            MD5 md5 = MD5.Create();
            byte[] data = Encoding.Default.GetBytes(inStr);
            byte[] res = md5.ComputeHash(data);
            for (int i = 0; i < res.Length; i++)
            {
                result += res[i].ToString("X2");
            }
            return result.ToLower();
        }

        public static string generate_key()
        {
            string result = "";
            Random r = new Random();
            long randNumber = r.Next(100, 999) * 1000 + r.Next(1, 999);
            result = get_md5(randNumber.ToString());
            return result;
        }

        private static byte[] passport_key(byte[] txtBytes, string ekey)
        {
            ekey = get_md5(ekey);
            int ctr = 0;
            byte[] keyBytes = Encoding.ASCII.GetBytes(ekey);
            byte[] res = new byte[txtBytes.Length];
            for (int i = 0; i < txtBytes.Length; i++)
            {
                if (ctr == keyBytes.Length)
                {
                    ctr = 0;
                }
                res[i] = (byte)(txtBytes[i] ^ keyBytes[ctr++]);
            }
            return res;
        }

        public static string encrypt(string txt, string key, string time)
        {
            string result = "";
            Random r = new Random(DateTime.Now.Millisecond);
            string ekey = get_md5(r.Next(320000).ToString());
            int ctr = 0;
            byte[] txtBytes = Encoding.UTF8.GetBytes(txt);
            byte[] keyBytes = Encoding.UTF8.GetBytes(ekey);
            byte[] res = new byte[txtBytes.Length * 2];
            for (int i = 0; i < txtBytes.Length; i++)
            {
                if (ctr == keyBytes.Length)
                {
                    ctr = 0;
                }
                res[i * 2] = keyBytes[ctr];
                res[i * 2 + 1] = (byte)(txtBytes[i] ^ keyBytes[ctr++]);
            }
            result = Convert.ToBase64String(passport_key(res, key));
            result = dict_mapping(result, time);
            return result;
        }

        public static string decrypt(string txt, string key, string time)
        {
            string result = "";
            txt = dict_demapping(txt, time);
            byte[] txtBytes = passport_key(Convert.FromBase64String(txt), key);
            byte[] res = new byte[txtBytes.Length / 2];
            for (int i = 0; i < txtBytes.Length; i++)
            {
                byte md5 = txtBytes[i];
                res[i / 2] = (byte)(txtBytes[++i] ^ md5);
            }
            result = Encoding.UTF8.GetString(res);
            return result;
        }

        public static string dict_mapping(string txt, string time)
        {
            char[] resArr = txt.ToCharArray();
            string dict0 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string dict1 = getDict1(time);
            char[] dict1Arr = dict1.ToCharArray();
            for (int i = 0; i < resArr.Length; i++)
            {
                int ind = dict0.IndexOf(resArr[i]);
                //isisis
                if (ind >= 0)
                {
                    resArr[i] = dict1Arr[ind];
                }
            }
            string result = new string(resArr);
            return result;
        }

        public static string dict_demapping(string txt, string time)
        {
            char[] resArr = txt.ToCharArray();
            string dict0 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string dict1 = getDict1(time);
            char[] dict0Arr = dict0.ToCharArray();
            for (int i = 0; i < resArr.Length; i++)
            {
                int ind = dict1.IndexOf(resArr[i]);
                //isisis
                if (ind >= 0)
                {
                    resArr[i] = dict0Arr[ind];
                }
            }
            string result = new string(resArr);
            return result;
        }

        private static string getDict1(string time)
        {
            string dict1 = "JABTU45XVKSQ01LMRWGHI273DEF68YZNOPC9";
            char[] dictArr = dict1.ToCharArray();
            string tm = time.Substring(10, 2);
            string ts = time.Substring(12, 2);
            int tp = Convert.ToInt32(tm) * 60 + Convert.ToInt32(ts);
            if (tp < 1000)
                tp = 3600 - tp;
            char[] tps = tp.ToString().ToCharArray();
            int s0 = Convert.ToInt32(tps[0].ToString());
            int s1 = Convert.ToInt32(tps[1].ToString());
            int s2 = Convert.ToInt32(tps[2].ToString());
            int s3 = Convert.ToInt32(tps[3].ToString());
            if (s0 == 1)
            {
                char st = dictArr[s1];
                dictArr[s1] = dictArr[10 + s2];
                dictArr[10 + s2] = dictArr[20 + s3];
                dictArr[20 + s3] = st;
            }
            else if (s0 == 2)
            {
                char st = dictArr[10 + s1];
                dictArr[10 + s1] = dictArr[20 + s2];
                dictArr[20 + s2] = dictArr[s3];
                dictArr[s3] = st;
            }
            else if (s0 == 3)
            {
                char st = dictArr[20 + s1];
                dictArr[20 + s1] = dictArr[s2];
                dictArr[s2] = dictArr[10 + s3];
                dictArr[10 + s3] = st;
            }
            dict1 = new string(dictArr);
            return dict1;
        }

    }
}
