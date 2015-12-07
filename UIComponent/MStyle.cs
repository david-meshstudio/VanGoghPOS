using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace COM.MeshStudio.Lib.UIComponent
{
    public class MStyle
    {
        public Color fontColor = Color.Black, backColor = Color.White, borderColor = Color.Black;
        public int width = 50, height = 50, borderWidth = 1;

        public static MStyle GetStyle(string controlName)
        {
            MStyle style = new MStyle();
            return style;
        }
    }
}
