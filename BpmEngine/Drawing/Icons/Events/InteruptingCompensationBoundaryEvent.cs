using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingCompensationBoundaryEvent)]
    internal class InteruptingCompensationBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Rewind(false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
