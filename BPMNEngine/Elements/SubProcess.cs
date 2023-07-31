using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn", "subProcess")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Process))]
    internal class SubProcess : AFlowNode,IProcess
    {
        public SubProcess(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) { }

        public bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid)
        {
            if (ExtensionElement != null&&((ExtensionElements)ExtensionElement).Children
                    .Any(ie => ie is ConditionSet set && !set.Evaluate(variables)))
                return false;
            return isProcessStartValid(this, variables);
        }

        public IEnumerable<StartEvent> StartEvents 
            => Children.OfType<StartEvent>();

        public override bool IsValid(out string[] err)
        {
            if (base.IsValid(out err))
            {
                bool hasStart = Children.Any(elem => elem is StartEvent || (elem is IntermediateCatchEvent ice && ice.SubType.HasValue));
                bool hasEnd = Children.Any(elem=>elem is EndEvent);
                bool hasIncoming = this.Incoming!=null || Children.Any(elem=>elem is IntermediateCatchEvent ice && ice.SubType.HasValue);
                if (hasStart && hasEnd && hasIncoming)
                    return true;
                var terr = new List<string>();
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
