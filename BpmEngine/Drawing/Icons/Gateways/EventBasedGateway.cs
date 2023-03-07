using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;

using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EventBasedGateway)]
    internal class EventBasedGateway : AGateway
    {
        private const float _CIRCLE_PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = new Point[]
        {
            new Point(23f,26f),
            new Point(32f,21f),
            new Point(40f,26f),
            new Point(37f,38f),
            new Point(25f,38f),
            new Point(23f,26f)
        };

        protected override void _Draw(ICanvas surface, Color color)
        {
            base._Draw(surface, color);

            for (int idx = 1; idx<_POINTS.Length; idx++)
                surface.DrawLine(_POINTS[idx-1], _POINTS[idx]);

            surface.DrawLine(_POINTS[_POINTS.Length-1], _POINTS[0]);

            surface.StrokeSize = _CIRCLE_PEN_SIZE;

            surface.DrawEllipse(new Rect(16, 16, 30, 30));
            surface.DrawEllipse(new Rect(19, 19, 24, 24));
        }
    }
}
