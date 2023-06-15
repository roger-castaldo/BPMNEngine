using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ParallelGateway)]
    internal class ParallelGateway : AGateway
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new CenterPlus()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
