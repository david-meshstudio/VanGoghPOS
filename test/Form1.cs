using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COM.MeshStudio.Lib.BasicComponent;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string,string> para = new Dictionary<string,string>();
            para.Add("a", EncryptTool.encrypt("郑珂威", "key", "20140417161616"));
            string ret = APIAdaptor.RawPostRequest("http://vangogh.sinaapp.com/test/test2.php", para);
            byte[] ret0 = CompressionTool.GZipDecompress(Convert.FromBase64String(ret));
            string ret0str = Encoding.UTF8.GetString(ret0);
            ret0str = EncryptTool.decrypt(ret0str, "key", "20140417161616");
            List<object> retobj = JsonTool.JSON_Decode(ret0str);
            textBox1.Text = JsonTool.JSON_Encode(retobj);
            //byte[] ret1 = CompressionTool.GZipCompress(Encoding.UTF8.GetBytes(ret));
            //string ret1str = Encoding.UTF8.GetString(ret1);
            //textBox2.Text = ret1str;
            //byte[] ret2 = CompressionTool.GZipDecompress(ret1);
            //textBox3.Text = Encoding.UTF8.GetString(ret2);
        }
    }
}
