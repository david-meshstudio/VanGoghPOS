/* ProcessClasses
 * Description: 串口打印机类库

 * Created by: David Zheng
 * Created on: April-12-2010
 * Last Modified by: David Zheng
 * Last Modified on: April-12-2010
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace COM.MeshStudio.Lib.HardwareLibrary
{
    public class SerialPortPrinter
    {
        private string comStr = "com1";

        public SerialPortPrinter(string _comStr)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            comStr = _comStr;
        }

        [StructLayout(LayoutKind.Sequential)]

        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }

        [DllImport("kernel32.dll")]

        private static extern int CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            int dwShareMode,
            int lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            int hTemplateFile
            );

        [DllImport("kernel32.dll")]

        private static extern bool WriteFile(
            int hFile,
            byte[] lpBuffer,
            int nNumberOfBytesToWrite,
            ref int lpNumberOfBytesWritten,
            ref OVERLAPPED lpOverlapped
            );

        [DllImport("kernel32.dll")]

        private static extern bool CloseHandle(
            int hObject
            );

        private int iHandle;

        public bool Open()
        {
            iHandle = CreateFile(comStr, 0x40000000, 0, 0, 3, 0, 0);
            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool WriteString(String Mystring)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;

                byte[] mybyte = System.Text.Encoding.Default.GetBytes(Mystring);
                bool b = WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
                return b;
            }
            else
            {
                throw new Exception("不能连接到打印机!");
            }
        }

        public bool WriteString(byte[] mybyte)
        {
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                WriteFile(iHandle, mybyte, mybyte.Length,
                    ref i, ref x);
                return true;
            }
            else
            {
                throw new Exception("不能连接到打印机!");
            }
        }

        public bool Close()
        {
            return CloseHandle(iHandle);
        }
    }
}
