using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class InnerCircle : IIconPart
    {
        private bool _dashed;

        public InnerCircle()
            : this(false) { }
        public InnerCircle(bool dashed)
        {
            _dashed=dashed;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            gp.DrawEllipse(new Pen(color, 1f,(_dashed ? Constants.DASH_PATTERN : null)), new Rectangle(4, 4, AIcon.IMAGE_SIZE - 8, AIcon.IMAGE_SIZE- 8));
        }
    }
}
