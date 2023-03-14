using Microsoft.Maui.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class UpArrow : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = new Point[] {
            new Point(23f,9f),
            new Point(33f,34f),
            new Point(23f,25f),
            new Point(13f,34f),
            new Point(23f,9f)
        };

        private static readonly PathF _PATH;

        static UpArrow()
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p => _PATH.LineTo(p));
            _PATH.Close();
        }

        private bool _filled;
        public UpArrow(bool filled)
        {
            _filled=filled;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            if (_filled)
            {
                surface.FillColor=color;
                surface.FillPath(_PATH);
            }
            else
            {
                surface.StrokeColor=color;
                surface.StrokeSize = _PEN_SIZE;
                surface.StrokeDashPattern=null;
                surface.DrawPath(_PATH);
            }
        }
    }
}
