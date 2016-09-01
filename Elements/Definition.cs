using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","definitions")]
    internal class Definition : AParentElement
    {
        public Definition(XmlElement elem)
            : base(elem) { }

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
    }
}
