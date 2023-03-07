using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;

using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
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

            for (int idx = 1; idx<_POINTS.Length; idx++)
                surface.DrawLine(_POINTS[idx-1], _POINTS[idx]);

            surface.DrawLine(_POINTS[_POINTS.Length-1], _POINTS[0]);
        }
    }
}
