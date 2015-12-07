using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Vangogh.DataObject;
using COM.MeshStudio.Lib.UIComponent;

namespace Vangogh
{
    public class CategoryArea : Panel
    {
        public delegate void MessageCallBack(string message);
        public MessageCallBack callBack;
        public Panel CategoryDisplayArea;
        public Panel ControlPanel;

        public CategoryArea(int width, int heigh)
        {
            this.Width = width;
            this.Height = heigh;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            CategoryDisplayArea = new Panel();
            CategoryDisplayArea.Width = this.Width - 40;
            CategoryDisplayArea.Height = this.Height;
            ControlPanel = new Panel();
            ControlPanel.Width = 40;
            ControlPanel.Height = this.Height;
            List<Control> controlList = new List<Control>() { CategoryDisplayArea, ControlPanel };
            MLayout.HorizentalLayout(this, controlList);
        }

        private void controlCallBack(string message)
        {
            callBack(message);
        }

        public void Init(List<object> objectList)
        {
            List<Category> categoryList = new List<Category>();
            foreach(var obj in objectList)
            {
                categoryList.Add((Category)obj);
            }
            Init(categoryList);
        }

        public void Init(List<Category> categoryList)
        {
            List<Dictionary<string, string>> controlParamentList = new List<Dictionary<string, string>>();
            foreach(Category category in categoryList)
            {
                controlParamentList.Add(category.GetControlParameter());
            }
            MStyle mstyle = new MStyle();
            List<Control> mbuttonList = new List<Control>();
            mbuttonList.AddRange(MButton.FactoryGenerateMButtonList(controlParamentList, mstyle, controlCallBack));
            MLayout.HorizentalLayout(CategoryDisplayArea, mbuttonList);
        }
    }
}
