using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace COM.MeshStudio.Lib.UIComponent
{
    public class MButton : Button
    {
        public delegate void MessageCallBack(string message);
        public MessageCallBack callBack;
        public string MessageName;

        protected override void OnClick(EventArgs e)
        {
            callBack(MessageName);
        }

        public void ApplyStyle(MStyle buttonStyle)
        {
            this.Width = buttonStyle.width;
            this.Height = buttonStyle.height;
            this.ForeColor = buttonStyle.fontColor;
            this.BackColor = buttonStyle.backColor;
        }

        public static List<MButton> FactoryGenerateMButtonList(List<Dictionary<string, string>> dictList, MStyle buttonStyle, MessageCallBack callBack)
        {
            List<MButton> result = new List<MButton>();
            foreach(Dictionary<string,string> dict in dictList)
            {
                MButton mbutton = new MButton();
                mbutton.Text = dict.Keys.Contains("Text") ? dict["Text"] : "";
                mbutton.MessageName = dict.Keys.Contains("MessageName") ? dict["MessageName"] : "";
                mbutton.ApplyStyle(buttonStyle);
                mbutton.callBack = callBack;
                result.Add(mbutton);
            }
            return result;
        }
    }
}
