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
        private PagerDisplayBar pdb;
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
            pdb.RefreshMaxPage(maxPage);
        }

        public void InitControl()
        {
            //int height = this.Height / 4;
            int width = Math.Min(this.Height / 6, controlWidth);
            List<Control> cbList = new List<Control>();
            for (int i = 0; i < 4; i++)
            {
                MButton cb = new MButton();
                cb.Width = width;
                cb.Height = width;
                cb.MessageName = "cb" + i.ToString();
                cb.callBack = CBClick;
                cbList.Add(cb);
            }
            pdb = new PagerDisplayBar(width, this.Height - 4 * width, 2);
            cbList.Insert(2, pdb);
            MLayout.VerticalLayout(ControlPanel, cbList);
        }

        private void CBClick(string message)
        {
            DisplayArea.Controls.Clear();
            switch (message)
            {
                case "cb0":
                    currnetPage = 0;
                    break;
                case "cb1":
                    currnetPage = currnetPage > 0 ? currnetPage - 1 : 0;
                    break;
                case "cb2":
                    currnetPage = currnetPage < maxPage ? currnetPage + 1 : maxPage;
                    break;
                case "cb3":
                    currnetPage = maxPage;
                    break;
                default:
                    break;
            }
            DisplayArea.Controls.Add(pList[currnetPage]);
            pdb.ShowCurrentPage(currnetPage);
        }
    }
}
