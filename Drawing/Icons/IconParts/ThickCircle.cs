using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class ThickCircle : IIconPart
    {
        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 4f),2,2,AIcon.IMAGE_SIZE-4,AIcon.IMAGE_SIZE-4);
        }
    }
}
