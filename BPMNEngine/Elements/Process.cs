using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn", "process")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Process : AParentElement, IProcess
    {
        public ImmutableArray<StartEvent> StartEvents
            => Children.OfType<StartEvent>().ToImmutableArray();

        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
        {
            if (ExtensionElement != null && ((ExtensionElements)ExtensionElement)
                .Children
                .Any(elem => elem is ConditionSet set && !set.Evaluate(variables)))
                return false;
            return isProcessStartValid(this, variables);
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err =(err ?? Array.Empty<string>()).Concat(new string[] { "No child elements found in Process." });
                return false;
            }
            return res;
        }
    }
}
