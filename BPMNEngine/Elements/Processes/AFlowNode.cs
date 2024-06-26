using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [RequiredAttributeAttribute("id")]
    internal abstract record AFlowNode : AParentElement, IStepElement
    {
        protected AFlowNode(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public IElement Process
            => Parent switch
            {
                (Process proc) => proc,
                (AElement aelem) => aelem.Parent,
                _ => null
            };

        public IElement SubProcess
            => Parent switch
            {
                (SubProcess sub) => sub,
                (AFlowNode flowNode) => flowNode.SubProcess,
                _ => null
            };

        public IElement Lane
            => (Process as Process)?
                .Children
                .OfType<LaneSet>()
                .SelectMany(ls => ls.Children)
                .FirstOrDefault(ln => ln is Lane lane && lane.Nodes.Contains(ID));
            
        public IEnumerable<string> Incoming
            => Array.Empty<string>()
                .Concat(Children
                    .OfType<IncomingFlow>()
                    .Select(elem => elem.Value)
                ).Concat(OwningDefinition.MessageFlows
                    .Where(msgFlow => msgFlow.TargetRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)
                );

        public IEnumerable<string> Outgoing
            =>Array.Empty<string>()
                .Concat(Children
                    .OfType<OutgoingFlow>()
                    .Select(elem => elem.Value)
                ).Concat(OwningDefinition.MessageFlows
                    .Where(msgFlow => msgFlow.SourceRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)
                );
    }
}
