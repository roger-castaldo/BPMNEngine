using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EventBasedGateway)]
    internal class EventBasedGateway : AGateway
    {
        private static readonly Point[] _POINTS = new Point[]
        {
            new Point(23f,26f),
            new Point(32f,21f),
            new Point(40f,26f),
            new Point(37f,38f),
            new Point(25f,38f),
            new Point(23f,26f)
        };

        protected override void _Draw(Image gp, Color color)
        {
            base._Draw(gp, color);
            gp.DrawEllipse(new Pen(color, 1f), new Rectangle(16, 16, 30, 30));
            gp.DrawEllipse(new Pen(color, 1f), new Rectangle(19, 19, 24, 24));
            gp.DrawLines(new Pen(color, 1f), _POINTS);
        }
    }
}
