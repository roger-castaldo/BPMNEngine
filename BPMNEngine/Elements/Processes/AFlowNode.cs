using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Processes
{
    [RequiredAttribute("id")]
    internal abstract class AFlowNode : AParentElement, IStepElement
    {
        public IElement Process
            => (Parent is Process proc ? proc : (Parent is AElement elem ? elem.Parent : null));

        public IElement SubProcess
            => (Parent is SubProcess sub ? sub : (Parent is AFlowNode flowNode ? flowNode.SubProcess : null));

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
                ).Concat(Definition.MessageFlows
                    .Where(msgFlow => msgFlow.TargetRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)
                );

        public IEnumerable<string> Outgoing
            =>Array.Empty<string>()
                .Concat(Children
                    .OfType<OutgoingFlow>()
                    .Select(elem => elem.Value)
                ).Concat(Definition.MessageFlows
                    .Where(msgFlow => msgFlow.SourceRef==this.ID)
                    .Select(msgFlow => msgFlow.ID)
                );

        public AFlowNode(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) {}
    }
}
