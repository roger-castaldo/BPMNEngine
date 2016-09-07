using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
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

        internal bool IsProcessStartvalid(ProcessVariablesContainer variables, IsProcessStartValid isProcessStartValid)
        {
            if (ExtensionElement != null)
            {
                if (((ExtensionElements)ExtensionElement).IsInternalExtension)
                {
                    ExtensionElements ee = (ExtensionElements)ExtensionElement;
                    if (ee.Children != null)
                    {
                        if (ee.Children[0] is ConditionSet)
                        {
                            return ((ConditionSet)ee.Children[0]).Evaluate(variables);
                        }
                    }
                }
            }
            return isProcessStartValid(this, variables);
        }
    }
}
