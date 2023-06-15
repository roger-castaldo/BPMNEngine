using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ManualTask)]
    internal class ManualTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new Hand()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
