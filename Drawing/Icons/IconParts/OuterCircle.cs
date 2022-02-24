using System;
using System.Collections.Generic;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class OuterCircle : IIconPart
    {
        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1F),new Rectangle(0,0,AIcon.IMAGE_SIZE-1,AIcon.IMAGE_SIZE-1));
        }
    }
}
