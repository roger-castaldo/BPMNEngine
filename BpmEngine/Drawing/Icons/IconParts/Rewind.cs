using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;

using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Rewind : IIconPart
    {
        private static readonly Point[] _POINTS = new Point[] {
            new Point(9f,23f),
            new Point(23f,15f),
            new Point(23f,23f),
            new Point(37f,15f),
            new Point(37f,31f),
            new Point(23f,23f),
            new Point(23f,31f),
            new Point(9f,23f)
        };

        public void Add(Image gp, int iconSize, Color color)
        {
            gp.FillPolygon(color, _POINTS);
        }
    }
}
