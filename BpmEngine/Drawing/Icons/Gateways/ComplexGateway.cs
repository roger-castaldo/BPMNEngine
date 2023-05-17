using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ComplexGateway)]
    internal class ComplexGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new CenterStar()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
