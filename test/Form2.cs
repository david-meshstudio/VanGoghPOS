using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COM.MeshStudio.Lib.UIComponent;
using Vangogh;
using Vangogh.DataObject;
using COM.MeshStudio.Lib.BasicComponent;

namespace test
{
    public partial class Form2 : Form
    {
        private MWorker mworker;
        private SocketTool.MServerSocket server;
        HttpServer httpserver = new HttpServer();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            mworker = new MWorker(4);
            mworker.WorkerJobDone = WorkerJobDone;
            AreaCategory ca = new AreaCategory(250, 250);
            ca.Location = new Point(10, 10);
            ca.callBack = MessageCallBack;
            List<Category> categoryList = new List<Category>();
            for (int i = 0; i < 500; i++)
            {
                Category cate = new Category();
                cate.ID = i;
                cate.Name = "Cat" + i;
                cate.SN = "C" + i.ToString().PadLeft(6, '0');
                categoryList.Add(cate);
            }
            string clstr = JsonTool.JSON_Encode_Object(categoryList.ToList<object>());
            FileAdaptor.WriteFile("a.file", clstr);
            string clstr2 = FileAdaptor.ReadFile("a.file");
            List<object> list = JsonTool.JSON_Decode_Object(clstr2, new List<object>() { new Category() });
            ca.Init(list);
            this.Controls.Add(ca);
            server = SocketTool.FactoryGenerateServerTCPSocket();
            server.receiveCallBack = SocketCallBack;
            server.Listen("127.0.0.1", 8885, 10);
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WinMessageTool.WM_COPYDATA:
                    WinMessageTool.COPYDATASTRUCT cdata = new WinMessageTool.COPYDATASTRUCT();
                    cdata = (WinMessageTool.COPYDATASTRUCT)m.GetLParam(cdata.GetType());
                    string message = cdata.lpData;
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void MessageCallBack(string message)
        {
            MWorker.MJob mjob = new MWorker.MJob();
            mjob.id = "Hello";
            mjob.type = "API";
            mjob.task = "test";
            mjob.parameter = message;
            mjob.startTime = BasicTool.GetTimestampLongAfterNSeconds(30);
            mworker.AddJob(JsonTool.JSON_Encode_Object(new List<object>() { mjob }));
        }

        private void WorkerJobDone(string message)
        {
            WinMessageTool.SendMessage("Main", message);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void SocketCallBack(string message)
        {
            MessageBox.Show(message);
            server.SendMessage("test", "You're Welcome!");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            server.Stop();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            server.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //BasicTool.CallScreenKeyboard();
            //CloudStorageTool.QiniuPutFile(@"E:\21C-120140909152845~1.jpg");
            LogTool.AddDebugLog("hello world");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string fileName = openFileDialog1.FileName;
            MessageBox.Show(FileAdaptor.ReadFile(fileName));
        }

        private void button4_Click(object sender, EventArgs e)
        {
            httpserver.getResponse = GetResponse;
            httpserver.Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            httpserver.Stop();
        }

        private string GetResponse(string path, string query)
        {
            return path + "," + query;
        }
    }
}
