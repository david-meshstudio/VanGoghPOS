using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace COM.MeshStudio.Lib.HardwareLibrary
{
    public class MWUsbICReader
    {
        private int icdev;

        public MWUsbICReader()
        {
            Init();
        }

        public int getICDev()
        {
            return icdev;
        }

        public void Init()
        {
            icdev = IC.usb_ic_init();
        }

        public string readVersionInfo()
        {
            string result = "";
            byte[] databuff = new byte[30];
            int res = IC.srd_ver(icdev, 18, databuff);
            result = System.Text.Encoding.Default.GetString(databuff);
            return result;
        }

        public string readData()
        {
            string result = "";
            byte[] data = new byte[16];
            byte[] databuff = new byte[32];
            int res = IC4442.srd_4442(icdev, 64, 16, data);
            if (res == 0)
            {
                int st = IC.hex_asc(data, databuff, 16);
                result = System.Text.Encoding.ASCII.GetString(databuff);
            }
            return result;
        }

        public bool getStatus()
        {
            byte[] status = new byte[5];
            return IC.get_status(icdev, status) == 0;
        }

        public int getCard()
        {
            return IC.chk_card(icdev);
        }

        public void writeData(string _stringIn)
        {
            byte[] buff = System.Text.Encoding.ASCII.GetBytes(_stringIn);
            byte[] databuff = new byte[32];
            IC.asc_hex(buff, databuff, 32);
            int st = IC4442.swr_4442(icdev, 64, 16, databuff);
            //swr_eeprom(icdev, 0, databuff.GetLength(0), databuff);
        }

        public void readCode()
        {
            byte[] buff = new byte[6];
            IC4442.rsc_4442(icdev, 6, buff);
        }

        public void writeCode(string _stringIn)
        {
            if (_stringIn.Length == 3)
            {
                byte[] databuff = System.Text.Encoding.Default.GetBytes(_stringIn);
                int res = IC4442.wsc_4442(icdev, 3, databuff);
            }
        }

        public void writeCode(short[] _codes)
        {
            byte[] databuff = new byte[3];
            if (_codes.GetLength(0) == 6)
            {
                for (int i = 0; i < 3; i++)
                {
                    short[] currentShorts = new short[2];
                    currentShorts[0] = _codes[i * 2];
                    currentShorts[1] = _codes[i * 2 + 1];
                    databuff[i] = shortsToBytes(currentShorts);
                }
            }
            int res = IC4442.wsc_4442(icdev, 3, databuff);
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

        private short[] byteToShort(byte doubleShort)
        {
            short[] result = new short[2];
            result[1] = (short)(doubleShort & 0x0f);
            result[0] = (short)((doubleShort & 0xf0) >> 4);
            return result;
        }

        private short[] bytesToShorts(byte[] doubleShort)
        {
            short[] result = new short[6];
            for (int i = 0; i < 3; i++)
            {
                result[i * 2 + 1] = (short)(doubleShort[i] & 0x0f);
                result[i * 2] = (short)((doubleShort[i] & 0xf0) >> 4);
            }
            return result;
        }

        private byte shortsToBytes(short[] _shorts)
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

        public void beep(short _time)
        {
            IC.dv_beep(icdev, _time);
        }

        public void exit()
        {
            IC.usb_ic_exit(icdev);
        }

        private string decryptString(string _key, string _originalString)
        {
            string result = "";
            return result;
        }

        private string encryptString(string _key, string _code)
        {
            string result = "";
            return result;
        }
    }


    /// <summary>
    /// IC 的摘要说明。
    /// </summary>
    public class IC
    {
        public IC()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public int icdev; // 通讯设备标识符
        public int ICstate;//插卡状态 state=1读写器插有卡；state=0读写器未插卡

        [DllImport("Mwic_32.dll", EntryPoint = "usb_ic_init", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：初始化通讯接口
        public static extern int usb_ic_init();

        [DllImport("Mwic_32.dll", EntryPoint = "usb_ic_exit", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    关闭通讯口
        public static extern Int16 usb_ic_exit(int icdev);

        [DllImport("Mwic_32.dll", EntryPoint = "get_status", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：     返回设备当前状态
        public static extern Int16 get_status(int icdev, [MarshalAs(UnmanagedType.LPArray)]byte[] state);

        [DllImport("Mwic_32.dll", EntryPoint = "dv_beep", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    读写器蜂鸣
        //调用：    icdev:   通讯设备标识符   time:    蜂鸣时间，值范围0～255（单位10ms）
        //返回：    <0       错误  	=0       正确
        public static extern Int16 dv_beep(int icdev, Int16 time);

        [DllImport("Mwic_32.dll", EntryPoint = "chk_card", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    测卡类型，仅适用明华公司生产的IC卡
        public static extern Int16 chk_card(int icdev);

        [DllImport("Mwic_32.dll", EntryPoint = "setsc_md", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    设置设备密码模式mode=0时设置设备密码有效，在设备加电时必须先核对
        //         设备密码才能对设备操作；mode=1时设置设备密码无效。
        public static extern Int16 setsc_md(int icdev, Int16 mode);

        [DllImport("Mwic_32.dll", EntryPoint = "asc_hex", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：     将ASCII码转换为十六进制数据
        //参数 asc:输入要转换的字符串 hex： 存放转换后的字符串 length: 为转换后的字符串长度
        //返回：     =0   正确   <0 错误  
        public static extern Int16 asc_hex([MarshalAs(UnmanagedType.LPArray)]byte[] asc, [MarshalAs(UnmanagedType.LPArray)] byte[] hex, int length);

        [DllImport("Mwic_32.dll", EntryPoint = "hex_asc", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        //说明：     将十六进制数据转换为ASCII码
        //参数：     hex:     输入要转换的字符串 asc：    存放转换后的字符串  length:  为要转换的字符串长度
        //返回：     =0       正确  <0       错误  [MarshalAs(UnmanagedType.AnsiBStr)]String asc
        public static extern Int16 hex_asc([MarshalAs(UnmanagedType.LPArray)] byte[] hex, [MarshalAs(UnmanagedType.LPArray)] byte[] asc, int length);

        [DllImport("Mwic_32.dll", EntryPoint = "srd_ver", SetLastError = true,
             CharSet = CharSet.Ansi, ExactSpelling = false,
             CallingConvention = CallingConvention.Winapi)]
        //说明：    读取硬件版本号  
        //		   调用：    icdev:    通讯设备标识符	len:      字符串长度，其值为18
        //  		  		databuff: 读出数据所存放地址指针
        //返回：     <>0   错误 	=0 正确
        public static extern Int16 srd_ver(int icdev, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] databuff);

        [DllImport("Mwic_32.dll", EntryPoint = "lib_ver", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：     读取软件（动态库）版本号
        public static extern Int16 lib_ver([MarshalAs(UnmanagedType.LPArray)]byte[] ver);


    }

    /// <summary>
    /// IC4442 的摘要说明。
    /// </summary>
    public class IC4442
    {
        public IC4442()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        [DllImport("Mwic_32.dll", EntryPoint = "swr_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    向指定地址写数据
        //调用：    icdev:    通讯设备标识符  offset:   偏移地址，其值范围0～255
        //          len:      字符串长度，其值范围1～256	w_string: 写入数据  [MarshalAs(UnmanagedType.LPArray)] byte[] StringBuilder
        //返回：     <0   错误 =0  正确
        public static extern Int16 swr_4442(int icdev, Int16 offset, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] w_string);

        [DllImport("Mwic_32.dll", EntryPoint = "srd_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    从指定地址读数据  
        //		   调用：    icdev:    通讯设备标识符 offset:   偏移地址，其值范围0～255
        //  		len:      字符串长度，其值范围1～256  		r_string: 读出数据所存放地址指针
        //返回：     <>0   错误 	=0 正确
        public static extern Int16 srd_4442(int icdev, Int16 offset, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] r_string);


        [DllImport("Mwic_32.dll", EntryPoint = "chk_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //	说明：    检查卡型是否正确  
        //调用：    icdev:   通讯设备标识符 
        //返回：     <0   错误   =0   正确
        public static extern Int16 chk_4442(int icdev);


        [DllImport("Mwic_32.dll", EntryPoint = "csc_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.Winapi)]
        //说明：    核对卡密码  
        //调用：    icdev:    通讯设备标识符  len:      密码个数，其值为3 p_string: 密码字符串指针
        //返回：     <0   错误    =0   密码正确
        public static extern Int16 csc_4442(int icdev, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] p_string);


        [DllImport("Mwic_32.dll", EntryPoint = "wsc_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    改写卡密码
        //调用：    icdev:    通讯设备标识符 len: 密码个数，其值为3 p_string: 新密码地址指针
        //返回：    <0   错误   =0   正确
        public static extern Int16 wsc_4442(int icdev, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] p_string);

        [DllImport("Mwic_32.dll", EntryPoint = "rsc_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    读出卡密码  
        //调用：    icdev:    通讯设备标识符  len:      密码个数，其值为3 	p_string: 存放密码地址指针
        // 返回：    <>0   错误   =0   正确	
        public static extern Int16 rsc_4442(int icdev, Int16 len, [MarshalAs(UnmanagedType.LPArray)]byte[] p_string);

        [DllImport("Mwic_32.dll", EntryPoint = "rsct_4442", SetLastError = true,
             CharSet = CharSet.Auto, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall)]
        //说明：    读出密码错误计数器值
        //调用：    icdev:    通讯设备标识符 counter:  密码错误记数值存放指针
        //返回：     <0   错误 >=0   正确
        public static extern Int16 rsct_4442(int icdev, out byte counter);


    }
}
