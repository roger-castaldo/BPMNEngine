using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "boundaryEvent")]
    [RequiredAttribute("attachedToRef")]
    internal class BoundaryEvent : AHandlingEvent
    {
        public string AttachedToID => this["attachedToRef"];

        public bool CancelActivity => this["cancelActivity"]==null || bool.Parse(this["cancelActivity"]);

        public new IEnumerable<string> Outgoing
        {
            get
            {
                var result = base.Outgoing;
                if (Children.Any(c => c is CompensationEventDefinition))
                {
                    var association = Definition.LocateElementsOfType<Association>()
                        .FirstOrDefault(asc => asc.SourceRef==ID);
                    if (association!=null)
                        result = result.Concat(new string[] { association.ID });
                }
                return result;
            }
        }
        public BoundaryEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errs = new List<string>();
            if (!Outgoing.Any())
                errs.Add("Boundary Events must have an outgoing path.");
            if (Outgoing.Count()>1)
                errs.Add("Boundary Events can only have one outgoing path.");
            if (Incoming.Any())
                errs.Add("Boundary Events cannot have an incoming path.");
            err = (err??Array.Empty<string>()).Concat(errs);
            return res && !errs.Any();
        }

        protected override int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables)
        {
            var cost = int.MaxValue;
            if (source.ID==AttachedToID)
                cost=0;
            else if (source.SubProcess!=null)
            {
                SubProcess sb = (SubProcess)source.SubProcess;
                cost=2;
                while (sb!=null && sb.ID!=this.AttachedToID)
                {
                    sb=(SubProcess)sb.SubProcess;
                    cost+=2;
                }
                if (sb==null || sb.ID!=this.AttachedToID)
                    cost=int.MaxValue;
            }
            return cost;
        }
    }
}
