using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNLabel")]
    [ValidParent(typeof(Edge))]
    [ValidParent(typeof(Shape))]
    internal class Label : AParentElement
    {
        public Bounds Bounds {
            get {
                Bounds ret = null;
                foreach (IElement ie in Children)
                {
                    if (ie is Bounds)
                    {
                        ret = (Bounds)ie;
                        break;
                    }
                }
                return ret;
            }
        }

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
