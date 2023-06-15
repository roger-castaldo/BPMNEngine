using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingMessageBoundaryEvent)]
    internal class NonInteruptingMessageBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new Envelope(false,false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
