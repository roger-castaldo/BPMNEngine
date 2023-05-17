using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.EscalationEndEvent)]
    internal class EscalationEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new UpArrow(true)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
