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
    public class AreaGeneral : Panel
    {
        public delegate void MessageCallBack(string message);
        public MessageCallBack callBack;
        public Panel DisplayArea;
        public Panel ControlPanel;
        public int currnetPage = 0, maxPage = 0, controlWidth = 40;
        public List<Panel> pList = new List<Panel>() { new Panel() };

        public AreaGeneral()
        {

        }

        public AreaGeneral(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            DisplayArea = new Panel();
            DisplayArea.Width = this.Width - controlWidth;
            DisplayArea.Height = this.Height;
            ControlPanel = new Panel();
            ControlPanel.Width = controlWidth;
            ControlPanel.Height = this.Height;
            List<Control> controlList = new List<Control>() { DisplayArea, ControlPanel };
            MLayout.HorizentalLayout(this, controlList);
            InitControl();
        }

        private void controlCallBack(string message)
        {
            callBack(message);
        }

        public void Init(List<object> objectList)
        {
            List<BusinessEntity> entityList = new List<BusinessEntity>();
            foreach (var obj in objectList)
            {
                entityList.Add((BusinessEntity)obj);
            }
            Init(entityList);
        }

        private void Init(List<BusinessEntity> entityList)
        {
            List<Dictionary<string, string>> controlParamentList = new List<Dictionary<string, string>>();
            foreach (BusinessEntity entity in entityList)
            {
                controlParamentList.Add(entity.GetControlParameter());
            }
            MStyle mstyle = new MStyle();
            List<Control> mbuttonList = new List<Control>();
            mbuttonList.AddRange(MButton.FactoryGenerateMButtonList(controlParamentList, mstyle, controlCallBack));
            pList = MLayout.HorizentalLayoutInPanelList(DisplayArea, mbuttonList);
            DisplayArea.Controls.Add(pList[0]);
            maxPage = pList.Count - 1;
        }

        public void InitControl()
        {
            int height = this.Height / 4;
            List<Control> cbList = new List<Control>();
            for (int i = 0; i < 4; i++)
            {
                MButton cb = new MButton();
                cb.Width = controlWidth;
                cb.Height = height;
                cb.MessageName = "cb" + i.ToString();
                cb.callBack = CBClick;
                cbList.Add(cb);
            }
            MLayout.VerticalLayout(ControlPanel, cbList);
        }

        private void CBClick(string message)
        {
            switch (message)
            {
                case "cb0":
                    DisplayArea.Controls.Clear();
                    currnetPage = 0;
                    DisplayArea.Controls.Add(pList[currnetPage]);
                    break;
                case "cb1":
                    DisplayArea.Controls.Clear();
                    currnetPage = currnetPage > 0 ? currnetPage - 1 : 0;
                    DisplayArea.Controls.Add(pList[currnetPage]);
                    break;
                case "cb2":
                    DisplayArea.Controls.Clear();
                    currnetPage = currnetPage < maxPage ? currnetPage + 1 : maxPage;
                    DisplayArea.Controls.Add(pList[currnetPage]);
                    break;
                case "cb3":
                    DisplayArea.Controls.Clear();
                    currnetPage = maxPage;
                    DisplayArea.Controls.Add(pList[currnetPage]);
                    break;
                default:
                    break;
            }
        }
    }
}
