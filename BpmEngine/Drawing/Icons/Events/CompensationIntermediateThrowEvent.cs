using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.CompensationIntermediateThrowEvent)]
    internal class CompensationIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Rewind(true)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
