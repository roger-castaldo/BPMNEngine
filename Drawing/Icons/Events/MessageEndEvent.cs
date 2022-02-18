using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.MessageEndEvent)]
    internal class MessageEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new Envelope(true,false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
