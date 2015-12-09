using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class LogTool
    {
        private static string LogDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\VanGogh\";

        public static void AddDebugLog(string content)
        {
            AddLog("Debug", content);
        }

        public static void AddErrorLog(string content)
        {
            AddLog("Error", content);
        }

        public static void AddUserLog(string content)
        {
            AddLog("User", content);
        }

        public static void AddLog(string type, string content)
        {
            string fileName = LogDirectory + type + "_" + DateTime.Now.ToString("yyyyMMddhh") + ".vlog";
            FileAdaptor.AppendFile(fileName, content);
        }
    }
}
