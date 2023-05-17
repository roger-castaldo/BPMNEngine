using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EscalationIntermediateThrowEvent)]
    internal class EscalationIntermediateThrowEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new UpArrow(true)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
