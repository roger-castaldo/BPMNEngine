using Microsoft.Maui.Graphics;

using BpmEngine.Drawing.Icons.IconParts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InclusiveGateway)]
    internal class InclusiveGateway : AGateway
    {
        protected override void _Draw(ICanvas surface, Color color)
        {
            base._Draw(surface, color);

            surface.DrawEllipse(new Rect(16, 16, 30, 30));
        }
    }
}
