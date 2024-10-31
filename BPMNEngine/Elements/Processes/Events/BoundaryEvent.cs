using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "boundaryEvent")]
    [RequiredAttributeAttribute("attachedToRef")]
    internal record BoundaryEvent : AHandlingEvent
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
                    var association = OwningDefinition.LocateElementsOfType<Association>()
                        .FirstOrDefault(asc => asc.SourceRef==ID);
                    if (association!=null)
                        result = result.Concat([association.ID]);
                }
                return result;
            }
        }
        public BoundaryEvent(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            var errs = new List<string>();
            if (!Outgoing.Any())
                errs.Add("Boundary Events must have an outgoing path.");
            if (Outgoing.Count()>1)
                errs.Add("Boundary Events can only have one outgoing path.");
            if (Incoming.Any())
                errs.Add("Boundary Events cannot have an incoming path.");
            return (isValid&&errs.Count==0, errors.Concat(errs));
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
