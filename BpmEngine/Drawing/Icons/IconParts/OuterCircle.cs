using System;
using System.Collections.Generic;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class OuterCircle : IIconPart
    {
        private bool _dashed;

        public OuterCircle()
            : this(false) { }
        public OuterCircle(bool dashed)
        {
            _dashed=dashed;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1f, (_dashed ? Constants.DASH_PATTERN : null)), new Rectangle(0,0,AIcon.IMAGE_SIZE-1,AIcon.IMAGE_SIZE-1));
        }
    }
}
