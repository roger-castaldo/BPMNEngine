using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SignalStartEvent)]
    internal class SignalStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new Triangle(false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
