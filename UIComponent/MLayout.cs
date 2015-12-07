using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace COM.MeshStudio.Lib.UIComponent
{
    public class MLayout
    {
        public static void HorizentalLayout(Control mother, List<Control> sonList)
        {
            int width = mother.Width;
            int x = 0;
            int y = 0;
            int maxHeight = 0;
            foreach (Control son in sonList)
            {
                if (x + son.Width <= width)
                {
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    x += son.Width;
                    maxHeight = Math.Max(maxHeight, son.Height);
                }
                else if (x == 0)
                {
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    y += son.Height;
                }
                else
                {
                    x = 0;
                    y += maxHeight;
                    maxHeight = 0;
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    if(son.Width<=width)
                    {
                        x += son.Width;
                        maxHeight = Math.Max(maxHeight, son.Height);
                    }
                    else
                    {
                        y += son.Height;
                    }
                }
            }
        }

        public static void VerticalLayout(Control mother, List<Control> sonList)
        {
            int height = mother.Height;
            int x = 0;
            int y = 0;
            int maxWidth = 0;
            foreach (Control son in sonList)
            {
                if (y + son.Height <= height)
                {
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    y += son.Height;
                    maxWidth = Math.Max(maxWidth, son.Width);
                }
                else if (y == 0)
                {
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    x += son.Width;
                }
                else
                {
                    y = 0;
                    x += maxWidth;
                    maxWidth = 0;
                    son.Location = new Point(x, y);
                    mother.Controls.Add(son);
                    y += son.Height;
                    if (son.Height <= height)
                    {
                        maxWidth = Math.Max(maxWidth, son.Width);
                    }
                    else
                    {
                        x += son.Width;
                    }
                }
            }
        }

        public static List<Panel> HorizentalLayoutInPanelList(Control mother, List<Control> sonList)
        {
            List<Panel> result = new List<Panel>();
            int width = mother.Width;
            int x = 0;
            int y = 0;
            int maxHeight = 0;
            Panel cp = new Panel();
            cp.Width = mother.Width;
            cp.Height = mother.Height;
            foreach (Control son in sonList)
            {
                if (x + son.Width <= width)
                {
                    son.Location = new Point(x, y);
                    cp.Controls.Add(son);
                    x += son.Width;
                    maxHeight = Math.Max(maxHeight, son.Height);
                }
                else if (x == 0)
                {
                    y += son.Height;
                    if (x == 0 && y + son.Height > cp.Height)
                    {
                        result.Add(cp);
                        cp = new Panel();
                        cp.Width = mother.Width;
                        cp.Height = mother.Height;
                        y = 0;
                    }
                    son.Location = new Point(x, y);
                    cp.Controls.Add(son);
                }
                else
                {
                    x = 0;
                    y += maxHeight;
                    if (x == 0 && y + son.Height > cp.Height)
                    {
                        result.Add(cp);
                        cp = new Panel();
                        cp.Width = mother.Width;
                        cp.Height = mother.Height;
                        y = 0;
                    }
                    maxHeight = 0;
                    son.Location = new Point(x, y);
                    cp.Controls.Add(son);
                    if (son.Width <= width)
                    {
                        x += son.Width;
                        maxHeight = Math.Max(maxHeight, son.Height);
                    }
                    else
                    {
                        y += son.Height;
                    }
                }
            }
            if (cp.Controls.Count > 0)
                result.Add(cp);
            return result;
        }
    }
}
