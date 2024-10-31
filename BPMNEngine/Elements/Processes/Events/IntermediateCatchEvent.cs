using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTagAttribute("bpmn", "intermediateCatchEvent")]
    internal record IntermediateCatchEvent : AHandlingEvent
    {
        public IntermediateCatchEvent(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            var errs = new List<string>();
            if (!Outgoing.Any())
                errs.Add("Intermediate Catch Events must have an outgoing path.");
            else if (Outgoing.Count() != 1)
                errs.Add("Intermediate Catch Events must have only 1 outgoing path.");
            return (isValid&&errs.Count==0, errors.Concat(errs));
        }

        protected override int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables)
        {
            var cost = int.MaxValue;
            SubProcess sb;
            if (Incoming.Any())
            {
                if (source.Outgoing.Any(str => Incoming.Contains(str)))
                    cost=1;
            }
            else if (source.SubProcess!=null)
            {
                sb = (SubProcess)source.SubProcess;
                cost = 3;
                var sid = SubProcess?.ID;
                if (sid==null)
                {
                    while (sb!=null)
                    {
                        sb = (SubProcess)sb.SubProcess;
                        cost+=2;
                    }
                }
                else
                {
                    while (sb!=null&&sid!=sb.ID)
                    {
                        sb = (SubProcess)sb.SubProcess;
                        cost+=2;
                    }
                    if (sb==null)
                        cost=int.MaxValue;
                }
            }
            else if (this.SubProcess!=null)
            {
                cost=3;
                sb=(SubProcess)this.SubProcess;
                while (sb!=null)
                {
                    sb = (SubProcess)sb.SubProcess;
                    cost+=2;
                }
            }
            else
                cost=2;
            return cost;
        }
    }
}
