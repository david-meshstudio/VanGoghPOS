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
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            AreaCategory ca = new AreaCategory(250, 250);
            ca.Location = new Point(10, 10);
            ca.callBack = MessageCallBack;
            List<Category> categoryList = new List<Category>();
            for (int i = 0; i < 100; i++)
            {
                Category cate = new Category();
                cate.ID = i;
                cate.Name = "Cat" + i;
                categoryList.Add(cate);
            }
            string clstr = JsonTool.JSON_Encode_Object(categoryList.ToList<object>());
            FileAdaptor.WriteFile("a.file", clstr);
            string clstr2 = FileAdaptor.ReadFile("a.file");
            List<object> list = JsonTool.JSON_Decode_Object(clstr2, new List<object>() { new Category() });
            ca.Init(list);
            this.Controls.Add(ca);
        }

        private void MessageCallBack(string message)
        {
            MessageBox.Show(message);
        }
    }
}
