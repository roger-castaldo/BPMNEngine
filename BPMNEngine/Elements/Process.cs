using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn","process")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Process : AParentElement,IProcess
    {
        public IEnumerable<StartEvent> StartEvents => Children
            .OfType<StartEvent>();
        
        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {}

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
        {
            if (ExtensionElement != null && ((ExtensionElements)ExtensionElement)
                .Children
                .Any(elem => elem is ConditionSet && !((ConditionSet)elem).Evaluate(variables)))
                return false;
            return isProcessStartValid(this, variables);
        }

        public override bool IsValid(out string[] err)
        {
            if (!Children.Any())
            {
                err = new string[] { "No child elements found." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
