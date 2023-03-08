using Microsoft.Maui.Graphics;

using SkiaSharp;
using System;
using System.Collections.Generic;

using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Triangle : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = new Point[] {
            new Point(24f,9f),
            new Point(34f,30f),
            new Point(13f,30f),
            new Point(24f,9f)
        };

        private static readonly PathF _PATH;

        static Triangle()
        {
            _PATH = new PathF(_POINTS[0]);
            for (int idx = 1; idx<_POINTS.Length; idx++)
                _PATH.LineTo(_POINTS[idx]);
            _PATH.Close();
        }

        private bool _filled;

        public Triangle(bool filled)
        {
            _filled = filled;
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
