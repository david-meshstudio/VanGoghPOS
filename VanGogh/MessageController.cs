using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vangogh
{
    public class MessageController
    {
        public static void SelfControlMessage(string message)
        {

        }

        public static void WindowsMessage(Form1 f, string message)
        {
            //MessageBox.Show(message);
            (f.Controls.Find("textBox1", false)[0] as TextBox).Text = message;
        }
    }
}
