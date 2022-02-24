using System;
using System.Collections.Generic;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class UpArrow : IIconPart
    {
        private static readonly Point[] _POINTS = new Point[] {
            new Point(23f,9f),
            new Point(33f,34f),
            new Point(23f,25f),
            new Point(13f,34f),
            new Point(23f,9f)
        };

        public void Add(Image gp, int iconSize, Color color)
        {
            gp.FillPolygon(new SolidBrush(color), _POINTS);
        }
    }
}
