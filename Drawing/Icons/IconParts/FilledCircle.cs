using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class FilledCircle : IIconPart
    {
        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.FillEllipse(new SolidBrush(color), new Rectangle(10, 10, AIcon.IMAGE_SIZE-20, AIcon.IMAGE_SIZE-20));
        }
    }
}
