using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    internal abstract class AFlowNode : AElement
    {
        public string name { get { return _GetAttributeValue("name"); } }

        public string[] Incoming
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        if (n.Name == "bpmn:incoming")
                            ret.Add(n.InnerText);
                    }
                }
                return (ret.Count == 0 ? null : ret.ToArray());
            }
        }

        public string[] Outgoing
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (XmlNode n in SubNodes)
                {
                    if (n.NodeType == XmlNodeType.Element)
                    {
                        if (n.Name == "bpmn:outgoing")
                            ret.Add(n.InnerText);
                    }
                }
                return (ret.Count == 0 ? null : ret.ToArray());
            }
        }

        public AFlowNode(XmlElement elem, XmlPrefixMap map)
            : base(elem, map)
        {
        }
    }
}
