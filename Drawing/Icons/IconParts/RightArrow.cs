using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class RightArrow : IIconPart
    {
        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF(11F,18f),
            new PointF(26F,18f),
            new PointF(26F,11f),
            new PointF(39F,23f),
            new PointF(26F,35f),
            new PointF(26F,29f),
            new PointF(11F,29f),
            new PointF(11F,18f)
        };

        private bool _fill;
        public RightArrow(bool fill)
        {
            _fill = fill;
        }

        public void Add(Graphics gp, int iconSize, Color color)
        {
            if (_fill)
                gp.FillPolygon(new SolidBrush(color), _POINTS);
            else
                gp.DrawLines(new Pen(color, 1f), _POINTS);
        }
    }
}
