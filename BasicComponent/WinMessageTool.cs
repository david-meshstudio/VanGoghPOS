using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace COM.MeshStudio.Lib.BasicComponent
{
    public class WinMessageTool
    {
        public const int WM_COPYDATA = 0x004A;
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }
        [DllImport("User32.dll")]
        private static extern int SendMessage(int hwnd, int msg, int wParam, ref COPYDATASTRUCT IParam);
        [DllImport("User32.dll")]
        private static extern int FindWindow(string lpClassName, string lpWindowName);

        public static int SendMessage(string windowName, string message)
        {
            int result = 0;
            int WINDOW_HANDLE = FindWindow(null, windowName);
            if(WINDOW_HANDLE != 0)
            {
                byte[] msg = Encoding.UTF8.GetBytes(message);
                int len = msg.Length;
                COPYDATASTRUCT cdata;
                cdata.dwData = (IntPtr)100;
                cdata.lpData = message;
                cdata.cData = len + 1;
                SendMessage(WINDOW_HANDLE, WM_COPYDATA, 0, ref cdata);
            }
            return result;
        }
    }
}
