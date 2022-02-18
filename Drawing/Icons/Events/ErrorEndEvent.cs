using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ErrorEndEvent)]
    internal class ErrorEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new Bolt()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
