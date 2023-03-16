using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using System.Linq;

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
            if (ExtensionElement != null&&((ExtensionElements)ExtensionElement).Children
                    .Any(ie => ie is ConditionSet && !((ConditionSet)ie).Evaluate(variables)))
                return false;
            return isProcessStartValid(this, variables);
        }

        public IEnumerable<StartEvent> StartEvents => Children
                .OfType<StartEvent>();

        public override bool IsValid(out string[] err)
        {
            if (base.IsValid(out err))
            {
                bool hasStart = Children.Any(elem => elem is StartEvent || (elem is IntermediateCatchEvent ice && ice.SubType.HasValue));
                bool hasEnd = Children.Any(elem=>elem is EndEvent);
                bool hasIncoming = this.Incoming!=null || Children.Any(elem=>elem is IntermediateCatchEvent ice && ice.SubType.HasValue);
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
