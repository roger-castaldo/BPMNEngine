using Microsoft.Maui.Graphics;
using BPMNEngine.Attributes;
using BPMNEngine.Drawing;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNLabel")]
    [ValidParent(typeof(Edge))]
    [ValidParent(typeof(Shape))]
    internal class Label : AParentElement
    {
        public Bounds Bounds => (Bounds)Children
            .FirstOrDefault(elem => elem is Bounds);

        public Label(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out string[] err)
        {
            if (Bounds == null)
            {
                err = new string[] { "No bounds specified." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
