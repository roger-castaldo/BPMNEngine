using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class InnerCircle : IIconPart
    {
        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1f), new Rectangle(4, 4, AIcon.IMAGE_SIZE - 8, AIcon.IMAGE_SIZE- 8));
        }
    }
}
