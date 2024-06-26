using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [ValidParent(typeof(IProcess))]
    internal abstract record AGateway : AFlowNode
    {
        public string Default
            => Outgoing.Any()&&Outgoing.Count()==1 ? Outgoing.First() : this["default"];

        protected AGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public virtual IEnumerable<string> EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = Outgoing
                .Where(o => ((SequenceFlow)definition.LocateElement(o)).IsFlowValid(isFlowValid, variables));
            if (!result.Any() && Default!=null)
                result = [Default];
            return result;
        }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errs = new List<string>();
            if (!Incoming.Any())
                errs.Add($"A {GetType().Name} must have at least 1 incoming path.");
            if (!Outgoing.Any())
                errs.Add($"A {GetType().Name} must have at least 1 outgoing path.");
            err = (err?? []).Concat(errs);
            return res && !errs.Any();
        }
    }
}
