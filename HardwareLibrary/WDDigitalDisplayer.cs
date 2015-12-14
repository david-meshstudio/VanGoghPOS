using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;

namespace COM.MeshStudio.Lib.HardwareLibrary
{
    public class WDDigitalDisplayer
    {
        public static string showData(string _port, List<string> _message)
        {
            string result = "";
            List<string> commandHead = new List<string>();
            commandHead.Add("\x1B\x71\x41");
            commandHead.Add("\x1B\x71\x42");
            commandHead.Add("\x1B\x71\x43");
            commandHead.Add("\x1B\x71\x44");
            string commandTail = "\x0D";
            for (int i = 0; i < 4; i++)
            {
                string message = "";
                try
                {
                    message = _message[i];
                }
                catch (Exception exp)
                {

                }
                string _command = commandHead[i] + _message[i] + commandTail;
                Encoding utf8 = Encoding.Default;
                Encoding gb2312 = Encoding.GetEncoding("gb2312");
                byte[] temp = utf8.GetBytes(_command);
                byte[] temp1 = Encoding.Convert(utf8, gb2312, temp);
                result = gb2312.GetString(temp1);
                SerialPort sp = new SerialPort(_port, 9600, Parity.None, 8);
                sp.Open();
                sp.Write(temp1, 0, temp1.GetLength(0));
                sp.Close();
            }
            return result;
        }
    }
}
