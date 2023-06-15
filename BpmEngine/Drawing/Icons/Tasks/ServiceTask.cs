using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.ServiceTask)]
    internal class ServiceTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new Cog()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
