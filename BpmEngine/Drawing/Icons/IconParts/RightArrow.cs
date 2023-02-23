using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using System;
using System.Collections.Generic;

using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class RightArrow : IIconPart
    {
        private static readonly Point[] _POINTS = new Point[] {
            new Point(11F,18f),
            new Point(26F,18f),
            new Point(26F,11f),
            new Point(39F,23f),
            new Point(26F,35f),
            new Point(26F,29f),
            new Point(11F,29f),
            new Point(11F,18f)
        };

        private readonly bool _fill;
        public RightArrow(bool fill)
        {
            _fill = fill;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            if (_fill)
                gp.FillPolygon(color, _POINTS);
            else
                gp.DrawLines(new Pen(color, 1f), _POINTS);
        }
    }
}
