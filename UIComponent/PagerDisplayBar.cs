using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace COM.MeshStudio.Lib.UIComponent
{
    public class PagerDisplayBar : PictureBox
    {
        public int currentPage, maxPage;
        private Bitmap bp;
        private Graphics g;

        public PagerDisplayBar(int width, int height, int maxPage)
        {
            this.Width = width;
            this.Height = height;
            this.currentPage = 0;
            this.maxPage = maxPage;
            this.BackColor = Color.White;
            bp = new Bitmap(width, height);
            g = Graphics.FromImage(bp);
            for (int i = 0; i <= maxPage; i++)
            {
                g.FillEllipse(i == 0 ? Brushes.Red : Brushes.Black, width / 2 - 3, (i + 1) * height / (2 + maxPage) - 3, 6, 6);
            }
            this.Image = bp;
        }

        public void RefreshMaxPage(int maxPage)
        {
            this.maxPage = maxPage;
            g.Clear(Color.White);
            for (int i = 0; i <= maxPage; i++)
            {
                g.FillEllipse(i == 0 ? Brushes.Red : Brushes.Black, this.Width / 2 - 3, (i + 1) * this.Height / (2 + maxPage) - 3, 6, 6);
            }
            this.Image = bp;
        }

        public void ShowCurrentPage(int currentPage)
        {
            this.currentPage = currentPage;
            for (int i = 0; i <= maxPage; i++)
            {
                g.FillEllipse(i == currentPage ? Brushes.Red : Brushes.Black, this.Width / 2 - 3, (i + 1) * this.Height / (2 + maxPage) - 3, 6, 6);
            }
            this.Image = bp;
        }
    }
}
