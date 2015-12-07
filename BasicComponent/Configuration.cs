using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class Configuration
    {
        public static string AppName = "demo";
        public static string Token = "";
        public static string SecretKey = "";

        public static string GetAPIURI(string functionName)
        {
            Dictionary<string, string> map = new Dictionary<string, string>();
            return map.ContainsKey(functionName) ? map[functionName] : "";
        }
    }
}
