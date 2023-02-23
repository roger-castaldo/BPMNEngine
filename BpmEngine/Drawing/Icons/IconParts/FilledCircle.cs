
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class FilledCircle : IIconPart
    {
        public void Add(Image gp, int iconSize, Color color)
        {
            gp.FillEllipse(color, new Rect(10, 10, AIcon.IMAGE_SIZE-20, AIcon.IMAGE_SIZE-20));
        }
    }
}
