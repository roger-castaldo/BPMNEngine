using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
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
