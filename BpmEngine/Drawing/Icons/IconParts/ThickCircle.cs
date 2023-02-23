using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using System;
using System.Collections.Generic;

using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class ThickCircle : IIconPart
    {
        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 4f),new Rect(2,2,AIcon.IMAGE_SIZE-5,AIcon.IMAGE_SIZE-5));
        }
    }
}
