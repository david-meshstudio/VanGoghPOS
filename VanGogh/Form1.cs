using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using COM.MeshStudio.Lib.BasicComponent;

namespace Vangogh
{
    public partial class Form1 : Form
    {
        ContextMenu notifyContextMenu = new ContextMenu();
        Icon ico = new Icon("vangogh.ico");
        SocketTool.MClientSocket socket;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Main";
            notifyIcon1.Text = "VanGoghPOS";
            socket = SocketTool.FactoryGenerateClientTCPSocket("test");
            socket.receiveCallBack = ControlMessageCallBack;
            socket.Connect("127.0.0.1", 8885);
        }

        private void ControlMessageCallBack(string message)
        {
            MessageController.SelfControlMessage(message);
        }

        protected override void DefWndProc(ref Message m)
        {
            switch(m.Msg)
            {
                case WinMessageTool.WM_COPYDATA:
                    WinMessageTool.COPYDATASTRUCT cdata = new WinMessageTool.COPYDATASTRUCT();
                    cdata = (WinMessageTool.COPYDATASTRUCT)m.GetLParam(cdata.GetType());
                    string message = cdata.lpData;
                    MessageController.WindowsMessage(this, message);
                    WindowState = FormWindowState.Normal;
                    this.Activate();
                    this.ShowInTaskbar = true;
                    notifyIcon1.Visible = false;
                    break;
                case 0x0010:
                    base.DefWndProc(ref m);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Icon = ico;
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            if(WindowState==FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.Activate();
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            socket.SendMessage(textBox1.Text);
        }
    }
}
