using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using System;
using System.Collections.Generic;

using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class OuterCircle : IIconPart
    {
        private readonly bool _dashed;

        public OuterCircle()
            : this(false) { }
        public OuterCircle(bool dashed)
        {
            _dashed=dashed;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1f, (_dashed ? Constants.DASH_PATTERN : null)), new Rect(0,0,AIcon.IMAGE_SIZE-1,AIcon.IMAGE_SIZE-1));
        }
    }
}
