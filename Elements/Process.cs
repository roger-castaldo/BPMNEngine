using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","process")]
    internal class Process : AParentElement
    {
        public bool isExecutable { get { return (_GetAttributeValue("isExecutable") == "" ? false : bool.Parse(_GetAttributeValue("isExecutable"))); } }

        public StartEvent[] StartEvents
        {
            get
            {
                List<StartEvent> ret = new List<StartEvent>();
                foreach (IElement elem in Children)
                {
                    if (elem is StartEvent)
                        ret.Add((StartEvent)elem);
                }
                return ret.ToArray();
            }
        }

        public Process(XmlElement elem)
            : base(elem) { }
    }
}
