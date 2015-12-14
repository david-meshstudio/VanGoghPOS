/* ProcessClasses
 * Description: 通用打印机控制类
 * Created by: David Zheng
 * Created on: April-21-2010
 * Last Modified by: David Zheng
 * Last Modified on: April-22-2010
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace COM.MeshStudio.Lib.HardwareLibrary
{
    public class GeneralPrinter
    {
        protected string id, port, type;
        protected long baudrate;
        private ParallelPortPrinter ppPrinter;
        private SerialPortPrinter spPrinter;


        #region set get

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        #endregion

        public GeneralPrinter(string _id, string _port, string _type)
        {
            id = _id;
            port = _port;
            type = _type;
            if (type.ToLower() == "parallel")
            {
                ppPrinter = new ParallelPortPrinter(port);
            }
            else if (type.ToLower() == "serial")
            {
                spPrinter = new SerialPortPrinter(port);
            }
        }

        public GeneralPrinter(string _id, string _port, string _type, long _baudrate)
        {
            id = _id;
            port = _port;
            type = _type;
            baudrate = _baudrate;
            if (type.ToLower() == "parallel")
            {
                ppPrinter = new ParallelPortPrinter(port);
            }
            else if (type.ToLower() == "serial")
            {
                spPrinter = new SerialPortPrinter(port);
            }
        }

        public void Print(string[] _content)
        {
            initCom();
            if (type.ToLower() == "parallel")
            {
                ppPrinter = new ParallelPortPrinter(port);
                ppPrinter.Open();
                for (int i = 0; i < _content.GetLength(0); i++)
                {
                    ppPrinter.WriteString(_content[i]);
                }
                ppPrinter.Close();
            }
            else if (type.ToLower() == "serial")
            {
                spPrinter = new SerialPortPrinter(port);
                spPrinter.Open();
                for (int i = 0; i < _content.GetLength(0); i++)
                {
                    spPrinter.WriteString(_content[i]);
                }
                spPrinter.Close();
                //SendComand(new char[] { '\x0d', '\x0a' });
                SendComandCode("\x1d\x56\x00");
            }
        }

        private void initCom()
        {
            SerialPort sp = new SerialPort(port, 9600, Parity.None, 8);
            sp.Open();
            sp.Close();
        }

        private void SendComandCode(string _comand)
        {
            SerialPort sp = new SerialPort(port, 9600, Parity.None, 8);
            sp.Open();
            sp.Write(_comand);
            sp.Close();
        }

        public void SendComand(string _comand)
        {

        }
    }
}
