using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.SendTask)]
    internal class SendTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new Envelope(true,true)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
