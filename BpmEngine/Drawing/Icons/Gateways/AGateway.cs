using Microsoft.Maui.Graphics;

using BpmEngine.Drawing.Icons.IconParts;
using BpmEngine.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BpmEngine.Drawing.Icons.Gateways
{
    internal abstract class AGateway : AIcon
    {
        private const float _PEN_SIZE = 2.0f;
        public new const int IMAGE_SIZE = 63;

        private static readonly Point[] _POINTS = new Point[] {
            new Point((float)AGateway.IMAGE_SIZE/2f,0),
            new Point((float)AGateway.IMAGE_SIZE-2,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,(float)AGateway.IMAGE_SIZE-2),
            new Point(0,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,0)
        };

        protected override sealed int _ImageSize
        {
            get { return IMAGE_SIZE; }
        }

        protected override void _Draw(ICanvas surface, Color color)
        {
            base._Draw(surface, color);

            surface.StrokeColor=color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = _PEN_SIZE;

            Diagram.DrawLines(surface, _POINTS);

            surface.DrawLine(_POINTS[_POINTS.Length-1], _POINTS[0]);
        }
    }
}
