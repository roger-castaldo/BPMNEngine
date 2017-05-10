using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn","lane")]
    [RequiredAttribute("id")]
    internal class Lane : AParentElement
    {
        public string[] Nodes
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (AElement elem in Children)
                {
                    if (elem is FlowNodeRef)
                        ret.Add(((FlowNodeRef)elem).Value);
                }
                return ret.ToArray();
            }
        }

        public string name { get { return this["name"]; } }

        public Lane(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map,parent) { }
    }
}
