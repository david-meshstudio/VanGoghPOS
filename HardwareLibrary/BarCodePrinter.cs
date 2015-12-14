/* ProcessClasses
 * Description: 条码打印机类库

 * Created by: David Zheng
 * Created on: Dec-11-2010
 * Last Modified by: David Zheng
 * Last Modified on: Dec-11-2010
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
//using WcfService;
using System.IO.Ports;
using System.Drawing.Printing;

namespace COM.MeshStudio.Lib.HardwareLibrary
{

    public class BarCodePrinter
    {
        private PrintDocument docToPrint = new PrintDocument();
        private string[] content;
        private float startX, startY, gapX, gapY;

        public BarCodePrinter()
        {
            startX = 10;
            startY = 10;
            gapX = 2;
            gapY = 2;
        }

        public BarCodePrinter(float startX, float startY, float gapX, float gapY)
        {
            this.startX = startX;
            this.startY = startY;
            this.gapX = gapX;
            this.gapY = gapY;
        }

        public void StartPrint(string[] _content)
        {
            content = _content;
            docToPrint.PrintPage += new PrintPageEventHandler(docToPrint_PrintPage);
            docToPrint.Print();
        }

        public void docToPrint_PrintPage(object sender, PrintPageEventArgs e)
        {
            float yPos = startY;
            float xPos = startX;
            float yAccumulate = yPos;
            for (int i = 0; i < content.GetLength(0); i++)
            {
                string text_1 = content[i].Replace("\r\n", "").TrimEnd();
                string[] textList = text_1.Split(new char[] { '|' });
                string text = "";
                int fontSize = 10;
                string fontFamily = "Arial";
                text = textList[0];
                if (text.StartsWith("{") & text.EndsWith("}"))
                {
                    if (textList.GetLength(0) == 5)
                    {
                        Bitmap bp = new Bitmap(text.Replace("{", "").Replace("}", ""));
                        int sx = Convert.ToInt32(textList[1]);
                        int sy = Convert.ToInt32(textList[2]);
                        int w = Convert.ToInt32(textList[3]);
                        int h = Convert.ToInt32(textList[4]);
                        e.Graphics.DrawImage(bp, sx, sy, w, h);
                    }
                }
                else
                {
                    if (textList.GetLength(0) == 2)
                    {
                        try
                        {
                            fontSize = Convert.ToInt32(textList[1]);
                        }
                        catch (Exception exp)
                        {

                        }
                    }
                    else if (textList.GetLength(0) == 3)
                    {
                        try
                        {
                            fontSize = Convert.ToInt32(textList[1]);
                            fontFamily = textList[2];
                        }
                        catch (Exception exp)
                        {

                        }
                    }
                    else if (textList.GetLength(0) == 5)
                    {
                        try
                        {
                            startX = (float)Convert.ToDouble(textList[1]);
                            startY = (float)Convert.ToDouble(textList[2]);
                            if (i == 0)
                            {
                                yPos = startY;
                                xPos = startX;
                                yAccumulate = yPos;
                            }
                            fontSize = Convert.ToInt32(textList[3]);
                            fontFamily = textList[4];
                        }
                        catch (Exception exp)
                        {

                        }
                    }
                    Font printFont = new Font(fontFamily, fontSize, FontStyle.Regular);
                    float fontHeight = e.Graphics.MeasureString("天", printFont).Height;
                    e.Graphics.DrawString(text, printFont, Brushes.Black, xPos, yAccumulate, new StringFormat());
                    yAccumulate += fontHeight + gapY;
                }
            }
        }
    }
}
