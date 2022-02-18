using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Bolt : IIconPart
    {
        private static readonly PointF[] _POINTS = new PointF[]
        {
            new PointF(11f,33f),
            new PointF(18f,12f),
            new PointF(27f,23f),
            new PointF(34f,11f),
            new PointF(28f,32f),
            new PointF(19f,22f),
            new PointF(11f,33f)
        };

        public void Add(Graphics gp, int iconSize, Color color)
        {
            gp.FillPolygon(new SolidBrush(color), _POINTS);
        }
    }
}
