/* ProcessClasses
 * Description: 串口IC卡读卡器类库
 * Created by: David Zheng
 * Created on: April-6-2010
 * Last Modified by: David Zheng
 * Last Modified on: April-9-2010
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace COM.MeshStudio.Lib.HardwareLibrary
{
    public class SerialPortICReader
    {
        private int icdev;

        #region Mwic_32.dll

        [DllImport("Mwic_32.dll")]

        private static extern int ic_init(int port, long baud);

        [DllImport("Mwic_32.dll")]

        private static extern int srd_ver(int icdev, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] v_string);

        [DllImport("Mwic_32.dll")]

        private static extern int srd_eeprom(int icdev, int offset, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] r_string);

        [DllImport("Mwic_32.dll")]

        private static extern int get_status(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] state);

        [DllImport("Mwic_32.dll")]

        private static extern int chk_card(int icdev);
        
        [DllImport("Mwic_32.dll")]

        private static extern int swr_eeprom(int icdev, int offset, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] w_string);

        [DllImport("Mwic_32.dll")]

        private static extern int dv_beep(int icdev, int time);

        [DllImport("Mwic_32.dll")]

        private static extern int ic_exit(int icdev);

        [DllImport("Mwic_32.dll")]

        private static extern int setsc_md(int icdev, int mode);

        [DllImport("Mwic_32.dll")]

        private static extern int auto_pull(int icdev);

        [DllImport("Mwic_32.dll")]

        private static extern int srd_snr(int icdev, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] sn_string);

        [DllImport("Mwic_32.dll")]

        private static extern int srd_dvsc(int icdev, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] sc_string);

        [DllImport("Mwic_32.dll")]

        private static extern int swr_dvsc(int icdev, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] sc_string);

        [DllImport("Mwic_32.dll")]

        private static extern int cmp_dvsc(int icdev, int len, [MarshalAs(UnmanagedType.LPArray)]byte[] sc_string);

        [DllImport("Mwic_32.dll")]

        private static extern int turn_on(int icdev);

        [DllImport("Mwic_32.dll")]

        private static extern int turn_off(int icdev);
        #endregion

        public SerialPortICReader()
        {
            Init(0, 115200);
        }

        public SerialPortICReader(int _port, long _baud)
        {
            Init(_port, _baud);
        }

        public int getICDev()
        {
            return icdev;
        }

        public void Init(int _port, long _baud)
        {
            icdev = ic_init(_port, _baud);
        }

        public void turnOn()
        {
            turn_on(icdev);
        }

        public void turnOff()
        {
            turn_off(icdev);
        }

        public string readVersionInfo(int _len)
        {
            string result = "";
            byte[] databuff = new byte[30];
            int res = srd_ver(icdev, _len, databuff);
            result = System.Text.Encoding.Default.GetString(databuff);
            return result;
        }

        public string readSNInfo(int _len)
        {
            string result = "";
            byte[] databuff = new byte[30];
            int res = srd_snr(icdev, _len, databuff);
            result = System.Text.Encoding.Default.GetString(databuff);
            return result;
        }

        public string readDeviceCode()
        {
            string result = "";
            byte[] databuff = new byte[3];
            int res = srd_dvsc(icdev, 3, databuff);
            result = System.Text.Encoding.Default.GetString(databuff);
            return result;
        }

        public string readE2PROM(int _offset, int _len)
        {
            string result = "";
            byte[] databuff = new byte[_len];
            int res = srd_eeprom(icdev, _offset, _len, databuff);
            result = System.Text.Encoding.Default.GetString(databuff);
            return result;
        }

        public bool getStatus()
        {
            byte[] status = new byte[5];
            return get_status(icdev, status) == 0;
        }

        public int getCard()
        {
            return chk_card(icdev);
        }

        public void writeE2PROM(string _stringIn)
        {
            byte[] databuff = System.Text.Encoding.Default.GetBytes(_stringIn);
            swr_eeprom(icdev, 0, databuff.GetLength(0), databuff);
        }

        public void writeCode(string _stringIn)
        {
            if (_stringIn.Length == 3)
            {
                byte[] databuff = System.Text.Encoding.Default.GetBytes(_stringIn);
                int res = swr_dvsc(icdev, 3, databuff);
            }            
        }

        public void writeCode(short[] _codes)
        {
            byte[] databuff = new byte[3];
            if (_codes.GetLength(0) == 6)
            {
                for(int i=0;i<3;i++)
                {
                    short[] currentShorts = new short[2];
                    currentShorts[0] = _codes[i * 2];
                    currentShorts[1] = _codes[i * 2 + 1];
                    databuff[i] = shortsToBytes(currentShorts);
                }
            }
            int res = swr_dvsc(icdev, 3, databuff);
        }

        public void writeCode6(string _codes)
        {
            try
            {
                short[] codesShort = new short[6];
                if (_codes.Length != 6) return;
                for (int i = 0; i < 6; i++)
                {
                    string bitCode = _codes.Substring(i, 1);
                    short bitCodeInt = (short)Convert.ToInt32(bitCode);
                    codesShort[i] = bitCodeInt;
                }
                writeCode(codesShort);
            }
            catch (Exception exp)
            {

            }
            
        }

        public short[] byteToShort(byte doubleShort)
        {
            short[] result = new short[2];
            result[1] = (short)(doubleShort & 0x0f);
            result[0] = (short)((doubleShort & 0xf0) >> 4);
            return result;
        }

        public short[] bytesToShorts(byte[] doubleShort)
        {
            short[] result = new short[6];
            for (int i = 0; i < 3; i++)
            {
                result[i * 2 + 1] = (short)(doubleShort[i] & 0x0f);
                result[i * 2] = (short)((doubleShort[i] & 0xf0) >> 4);
            }
            return result;
        }

        public byte shortsToBytes(short[] _shorts)
        {
            byte result = 0;
            for (int i = 0; i < 2; i++)
            {
                if (_shorts[i] < 0 || _shorts[i] > 9)
                {
                    return 0;
                }
            }
            string result16 = _shorts[0].ToString() + _shorts[1].ToString();
            result = (byte)Convert.ToInt32(result16, 16);
            return result;
        }

        public void beep(int _time)
        {
            dv_beep(icdev, _time);
        }

        public void exit()
        {
            ic_exit(icdev);
        }

        public void autoPullOut()
        {
            auto_pull(icdev);
        }

        public void setCodeMode(int _mode)
        {
            int res = setsc_md(icdev, _mode);
            if (res == 0)
            {
                int i = 0;
            }
            else
            {
                int j = 0;
            }
        }

        public bool compareCode(byte[] _code)
        {
            return cmp_dvsc(icdev, 3, _code) == 0;
        }

        private string decryptString(string _key, string _originalString)
        {
            string result="";
            return result;
        }

        private string encryptString(string _key, string _code)
        {
            string result = "";
            return result;
        }
    }
}
