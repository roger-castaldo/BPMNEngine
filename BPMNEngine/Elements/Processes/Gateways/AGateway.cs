using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [ValidParent(typeof(IProcess))]
    internal abstract record AGateway : AFlowNode
    {
        public string Default
            => Outgoing.Any()&&Outgoing.Count()==1 ? Outgoing.First() : this["default"];

        protected AGateway(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public virtual async ValueTask<IEnumerable<string>> EvaulateOutgoingPathsAsync(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables, ILogger? logger)
        {
            var result = await Outgoing
                .WhereAsync(o => ((SequenceFlow)definition.LocateElement(o)).IsFlowValidAsync(isFlowValid, variables, logger));
            if (!result.Any() && Default!=null)
                result = [Default];
            return result;
        }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            var errs = new List<string>();
            if (!Incoming.Any())
                errs.Add($"A {GetType().Name} must have at least 1 incoming path.");
            if (!Outgoing.Any())
                errs.Add($"A {GetType().Name} must have at least 1 outgoing path.");
            return (isValid&&errs.Count==0,errors.Concat(errs));
        }
    }
}
