using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Bolt : IIconPart
    {
        private static readonly Point[] _POINTS = new Point[]
        {
            new Point(11f,33f),
            new Point(18f,12f),
            new Point(27f,23f),
            new Point(34f,11f),
            new Point(28f,32f),
            new Point(19f,22f),
            new Point(11f,33f)
        };

        private bool _filled;
        public Bolt(bool filled)
        {
            _filled=filled;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            if (_filled)
                gp.FillPolygon(new SolidBrush(color), _POINTS);
            else
                gp.DrawLines(new Pen(color, 1F), _POINTS);
        }
    }
}
