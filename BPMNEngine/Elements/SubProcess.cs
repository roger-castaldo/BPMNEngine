using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "subProcess")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Process))]
    internal record SubProcess : AFlowNode,IProcess
    {
        public SubProcess(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
            => (
                ExtensionElement==null || 
                ((ExtensionElements)ExtensionElement).Children.OfType<ConditionSet>().All(cset=>cset.Evaluate(variables))
            )
            && isProcessStartValid(this, variables);
        

        public ImmutableArray<StartEvent> StartEvents 
            => Children.OfType<StartEvent>().ToImmutableArray();

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            bool hasStart = Children.Any(elem => elem is StartEvent || (elem is IntermediateCatchEvent ice && ice.SubType.HasValue));
            bool hasEnd = Children.Any(elem=>elem is EndEvent);
            bool hasIncoming = Incoming.Any() || Children.Any(elem=>elem is IntermediateCatchEvent ice && ice.SubType.HasValue);
            var terr = new List<string>();
            if (!(hasStart && hasEnd && hasIncoming))
            {
                if (!hasStart)
                    terr.Add("A Sub Process Must have a StartEvent or valid IntermediateCatchEvent");
                if (!hasIncoming)
                    terr.Add("A Sub Process Must have a valid Incoming path, achieved through an incoming flow or IntermediateCatchEvent");
                if (!hasEnd)
                    terr.Add("A Sub Process Must have an EndEvent");
                err = (err??[]).Concat(terr);
            }
            return res && terr.Count==0;
        }
    }
}
