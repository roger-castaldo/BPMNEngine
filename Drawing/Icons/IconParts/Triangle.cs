using System;
using System.Collections.Generic;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Triangle : IIconPart
    {
        private static readonly Point[] _POINTS = new Point[] {
            new Point(24f,9f),
            new Point(34f,30f),
            new Point(13f,30f),
            new Point(24f,9f)
        };

        private bool _filled;

        public Triangle(bool filled)
        {
            _filled = filled;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            if (_filled)
                gp.FillPolygon(new SolidBrush(color), _POINTS);
            else
                gp.DrawLines(new Pen(color,1f),_POINTS);
        }
    }
}
