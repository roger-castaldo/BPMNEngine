using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class UpArrow : IIconPart
    {
        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF(23f,9f),
            new PointF(33f,34f),
            new PointF(23f,25f),
            new PointF(13f,34f),
            new PointF(23f,9f)
        };

        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.FillPolygon(new SolidBrush(color), _POINTS);
        }
    }
}
