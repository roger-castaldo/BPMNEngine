using BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.NonInteruptingSignalBoundaryEvent)]
    internal class NonInteruptingSignalBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(true),
            new InnerCircle(true),
            new Triangle(false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
