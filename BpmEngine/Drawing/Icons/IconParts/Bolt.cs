
using Microsoft.Maui.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Bolt : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly PointF[] _POINTS = new PointF[]
        {
            new PointF(11f,33f),
            new PointF(18f,12f),
            new PointF(27f,23f),
            new PointF(34f,11f),
            new PointF(28f,32f),
            new PointF(19f,22f),
            new PointF(11f,33f)
        };

        private static readonly PathF _PATH;

        static Bolt()
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p=>_PATH.LineTo(p));
            _PATH.Close();
        }

        private readonly bool _filled;
        public Bolt(bool filled)
        {
            _filled=filled;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            if (_filled)
            {
                surface.FillColor= color;
                surface.FillPath(_PATH);
            }
            else
            {
                surface.StrokeColor=color;
                surface.StrokeDashPattern=null;
                surface.StrokeSize = _PEN_SIZE;
                surface.DrawPath(_PATH);
            }
        }
    }
}
