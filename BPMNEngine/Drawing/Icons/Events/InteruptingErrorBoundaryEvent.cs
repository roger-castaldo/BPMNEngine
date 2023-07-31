﻿using BPMNEngine.Drawing.Icons.IconParts;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingErrorBoundaryEvent)]
    internal class InteruptingErrorBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Bolt(false)
        };

        protected override IIconPart[] Parts
        {
            get { return _PARTS; }
        }
    }
}
