using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InclusiveGateway)]
    internal class InclusiveGateway : AGateway
    {
        protected override void _Draw(Image gp, Color color)
        {
            base._Draw(gp, color);
            gp.DrawEllipse(new Pen(color, 4f), new Rectangle(16, 16, 30, 30));
        }
    }
}
