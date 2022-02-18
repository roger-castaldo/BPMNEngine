using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class OuterCircle : IIconPart
    {
        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1F),0,0,AIcon.IMAGE_SIZE-1,AIcon.IMAGE_SIZE-1);
        }
    }
}
