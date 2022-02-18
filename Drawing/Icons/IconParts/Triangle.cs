using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Triangle : IIconPart
    {
        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF(24f,9f),
            new PointF(34f,30f),
            new PointF(13f,30f),
            new PointF(24f,9f)
        };

        private bool _filled;

        public Triangle(bool filled)
        {
            _filled = filled;
        }

        public void Add(Graphics gp, int iconSize, Color color)
        {
            if (_filled)
                gp.FillPolygon(new SolidBrush(color), _POINTS);
            else
                gp.DrawLines(new Pen(color,1f),_POINTS);
        }
    }
}
