using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements
{
    [XMLTag("bpmn","process")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Process : AParentElement,IProcess
    {
        public bool isExecutable { get { return (this["isExecutable"] == null ? false : bool.Parse(this["isExecutable"])); } }

        public IEnumerable<StartEvent> StartEvents => Children
            .OfType<StartEvent>();

        public IEnumerable<BoundaryEvent> BoundaryEvents => Children
            .OfType<BoundaryEvent>();
        
        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

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
