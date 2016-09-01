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

        public Label(XmlElement elem) : base(elem) { }
    }
}
