using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNLabel")]
    internal class Label : AParentElement
    {
        public Bounds Bounds { get { return (Children.Length>0 ? (Bounds)Children[0] : null);} }

        public Label(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool IsValid(out string err)
        {
            if (Bounds == null)
            {
                err = "No bounds specified.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
