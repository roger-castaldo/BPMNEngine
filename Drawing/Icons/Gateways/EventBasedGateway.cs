using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EventBasedGateway)]
    internal class EventBasedGateway : AGateway
    {
        private static readonly PointF[] _POINTS = new PointF[]
        {
            new PointF(23f,26f),
            new PointF(32f,21f),
            new PointF(40f,26f),
            new PointF(37f,38f),
            new PointF(25f,38f),
            new PointF(23f,26f)
        };

        protected override void _Draw(Graphics gp, Color color)
        {
            base._Draw(gp, color);
            gp.DrawEllipse(new Pen(color, 1f), new Rectangle(16, 16, 30, 30));
            gp.DrawEllipse(new Pen(color, 1f), new Rectangle(19, 19, 24, 24));
            gp.DrawLines(new Pen(color, 1f), _POINTS);
        }
    }
}
