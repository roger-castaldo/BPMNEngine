using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class InnerCircle : IIconPart
    {
        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1f), 4, 4, AIcon.IMAGE_SIZE - 8, AIcon.IMAGE_SIZE- 8);
        }
    }
}
