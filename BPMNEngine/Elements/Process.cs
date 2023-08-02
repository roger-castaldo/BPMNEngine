using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn", "process")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal class Process : AParentElement, IProcess
    {
        public IEnumerable<StartEvent> StartEvents
            => Children.OfType<StartEvent>();

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
                err =(err ?? Array.Empty<string>()).Concat(new string[] { "No child elements found." });
                return false;
            }
            return res;
        }
    }
}
