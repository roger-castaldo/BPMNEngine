using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.TimerStartEvent)]
    internal class TimerStartEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new Clock()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
