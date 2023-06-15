using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Tasks
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.UserTask)]
    internal class UserTask : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new Person()
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}
