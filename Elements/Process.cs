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
    [RequiredAttribute("id")]
    internal class Process : AParentElement
    {
        public bool isExecutable { get { return (this["isExecutable"] == null ? false : bool.Parse(this["isExecutable"])); } }

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

        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        internal bool IsProcessStartvalid(ProcessVariablesContainer variables, IsProcessStartValid isProcessStartValid)
        {
            if (ExtensionElement != null)
            {
                ExtensionElements ee = (ExtensionElements)ExtensionElement;
                if (ee.Children != null)
                {
                    foreach (IElement ie in ee.Children)
                    {
                        if (ie is ConditionSet)
                        {
                            if (!((ConditionSet)ie).Evaluate(variables))
                                return false;
                        }
                    }
                }
            }
            return isProcessStartValid(this, variables);
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
