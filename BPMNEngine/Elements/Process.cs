using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "process")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Process: AParentElement, IProcess
    {
        public Process(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public ImmutableArray<StartEvent> StartEvents
            => Children.OfType<StartEvent>().ToImmutableArray();

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
            => (
                ExtensionElement==null ||
                ExtensionElement.Children.OfType<ConditionSet>().All(cset => cset.Evaluate(variables))
            )
            && isProcessStartValid(this, variables);

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err =(err ?? []).Append("No child elements found in Process.");
                return false;
            }
            return res;
        }
    }
}
