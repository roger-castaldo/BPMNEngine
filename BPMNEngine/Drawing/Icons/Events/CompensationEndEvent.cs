﻿using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.CompensationEndEvent)]
    internal class CompensationEndEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new ThickCircle(),
            new Rewind(true)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}