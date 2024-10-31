using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [RequiredAttributeAttribute("id")]
    internal abstract record AFlowNode : AParentElement, IStepElement
    {
        protected AFlowNode(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public IElement? Process
            => GetParent<Process>();

        public IElement? SubProcess
            => GetParent<SubProcess>();

        public IElement? Lane
            => (Process as Process)?
                .Children
                .OfType<LaneSet>()
                .SelectMany(ls => ls.Children.OfType<IElement>())
                .FirstOrDefault(ln => ln is Lane lane && lane.Nodes.Contains(ID));

        public IEnumerable<string> Incoming
            => Array.Empty<string>()
                .Concat(Children
                    .OfType<IncomingFlow>()
                    .Select(elem => elem.Value)
                ).Concat(OwningDefinition?.MessageFlows
                    .Where(msgFlow => msgFlow.TargetRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)?? []
                );

        public IEnumerable<string> Outgoing
            => Array.Empty<string>()
                .Concat(Children
                    .OfType<OutgoingFlow>()
                    .Select(elem => elem.Value)
                ).Concat(OwningDefinition?.MessageFlows
                    .Where(msgFlow => msgFlow.SourceRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)?? []
                );
    }
}
