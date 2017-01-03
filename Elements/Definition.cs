using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","definitions")]
    [RequiredAttribute("id")]
    internal class Definition : AParentElement
    {
        public Definition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public Diagram[] Diagrams
        {
            get
            {
                List<Diagram> ret = new List<Diagram>();
                foreach (IElement elem in Children)
                {
                    if (elem is Diagram)
                        ret.Add((Diagram)elem);
                }
                return ret.ToArray();
            }
        }

        public MessageFlow[] MessageFlows
        {
            get
            {
                List<MessageFlow> ret = new List<MessageFlow>();
                foreach (IElement elem in Children)
                {
                    if (elem is Collaboration)
                    {
                        Collaboration collab = (Collaboration)elem;
                        foreach (IElement msg in collab.Children)
                        {
                            if (msg is MessageFlow)
                                ret.Add((MessageFlow)msg);
                        }
                        break;
                    }
                }
                return ret.ToArray();
            }
        }

        public IElement LocateElement(string id)
        {
            return _RecurLocateElement(this, id);
        }

        private IElement _RecurLocateElement(IElement elem, string id)
        {
            if (elem.id == id)
                return elem;
            IElement ret = null;
            if (elem is IParentElement)
            {
                foreach (IElement selem in ((IParentElement)elem).Children)
                {
                    ret = _RecurLocateElement(selem, id);
                    if (ret != null)
                        break;
                }
            }
            return ret;
        }

        public override bool IsValid(out string[] err)
        {
            if (Children.Length == 0)
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
