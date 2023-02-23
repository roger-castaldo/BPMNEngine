using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using System;
using System.Collections.Generic;

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

        private bool _filled;
        public UpArrow(bool filled)
        {
            _filled=filled;
        }

        public void Add(Image gp, int iconSize, Color color)
        {
            if (_filled)
                gp.FillPolygon(color, _POINTS);
            else
                gp.DrawLines(new Pen(color, 1F), _POINTS);
        }
    }
}
