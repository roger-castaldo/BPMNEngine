using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Rewind : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly PointF[] _POINTS = new PointF[] {
            new PointF(9f,23f),
            new PointF(23f,15f),
            new PointF(23f,23f),
            new PointF(37f,15f),
            new PointF(37f,31f),
            new PointF(23f,23f),
            new PointF(23f,31f),
            new PointF(9f,23f)
        };

        private static readonly PathF _PATH;

        static Rewind()
        {
            _PATH = new PathF(_POINTS[0]);
            for (int idx = 1; idx<_POINTS.Length; idx++)
                _PATH.LineTo(_POINTS[idx]);
            _PATH.Close();
        }

        private readonly bool _filled;

        public Rewind(bool filled)
        {
            _filled=filled;
        }

        public void Add(ICanvas surface,int iconSize, Color color)
        {
            if (_filled) {
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
