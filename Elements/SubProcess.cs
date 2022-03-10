using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn", "subProcess")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Process))]
    internal class SubProcess : AFlowNode,IProcess
    {
        public SubProcess(XmlElement elem, XmlPrefixMap map, AElement parent) : base(elem, map, parent)
        {
        }

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
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

        public override bool IsValid(out string[] err)
        {
            if (base.IsValid(out err))
            {
                bool hasStart = false;
                bool hasEnd = false;
                bool hasIncoming = this.Incoming!=null;
                foreach (IElement elem in Children)
                {
                    if (elem is EndEvent)
                        hasEnd = true;
                    else if (elem is StartEvent)
                        hasStart = true;
                    else if (elem is IntermediateCatchEvent)
                    {
                        IntermediateCatchEvent ice = (IntermediateCatchEvent)elem;
                        if (ice.SubType.HasValue)
                        {
                            hasStart = true;
                            hasIncoming = true;
                        }
                    }
                }
                if (hasStart && hasEnd && hasIncoming)
                    return true;
                List<string> terr = new List<string>();
                if (!hasStart)
                    terr.Add("A Sub Process Must have a StartEvent or valid IntermediateCatchEvent");
                if (!hasIncoming)
                    terr.Add("A Sub Process Must have a valid Incoming path, achieved through an incoming flow or IntermediateCatchEvent");
                if (!hasEnd)
                    terr.Add("A Sub Process Must have an EndEvent");
                err = terr.ToArray();
            }
            return false;
        }
    }
}
