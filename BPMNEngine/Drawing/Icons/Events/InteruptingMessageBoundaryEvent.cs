﻿using BPMNEngine.Drawing.Icons.IconParts;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.Events
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InteruptingMessageBoundaryEvent)]
    internal class InteruptingMessageBoundaryEvent : AIcon
    {
        private static readonly IIconPart[] _PARTS = new IIconPart[] {
            new OuterCircle(),
            new InnerCircle(),
            new Envelope(false,false)
        };

        protected override IIconPart[] _parts
        {
            get { return _PARTS; }
        }
    }
}