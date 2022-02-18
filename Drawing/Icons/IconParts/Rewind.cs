using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Rewind : IIconPart
    {
        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF(9f,23f),
            new PointF(23f,15f),
            new PointF(23f,23f),
            new PointF(37f,15f),
            new PointF(37f,31f),
            new PointF(23f,23f),
            new PointF(23f,31f),
            new PointF(9f,23f)
        };

        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.FillPolygon(new SolidBrush(color), _POINTS);
        }
    }
}
