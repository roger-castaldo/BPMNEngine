using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using System.Linq;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateCatchEvent")]
    internal class IntermediateCatchEvent : AHandlingEvent
    {
        public IntermediateCatchEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            var errs = new List<string>();
            if (!Outgoing.Any())
                errs.Add("Intermediate Catch Events must have an outgoing path.");
            else if (Outgoing.Count() != 1)
                errs.Add("Intermediate Catch Events must have only 1 outgoing path.");
            err = (err??Array.Empty<string>()).Concat(errs);
            return res && !errs.Any();
        }

        protected override int GetEventCost(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables)
        {
            var cost=int.MaxValue;
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
