using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingEscalationBoundaryEvent)]
    internal class NonInteruptingEscalationBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new UpArrow(false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
