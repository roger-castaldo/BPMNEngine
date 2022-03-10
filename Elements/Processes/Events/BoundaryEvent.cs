using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "boundaryEvent")]
    [RequiredAttribute("attachedToRef")]
    internal class BoundaryEvent : AHandlingEvent
    {
        public BoundaryEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public string AttachedToID
        {
            get { return this["attachedToRef"]; }
        }

        public bool CancelActivity
        {
            get { return (this["cancelActivity"]==null ? false : bool.Parse(this["cancelActivity"])); }
        }

        public override bool IsValid(out string[] err)
        {
            if (Outgoing == null)
            {
                err = new string[] { "Boundary Events must have an outgoing path." };
                return false;
            }
            if (Outgoing.Length>1)
            {
                err = new string[] { "Boundary Events can only have one outgoing path." };
                return false;
            }
            if (Incoming != null)
            {
                err = new string[] { "Boundary Events cannot have an incoming path." };
                return false;
            }
            return base.IsValid(out err);
        }

        protected override bool _HandlesEvent(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables, out int cost)
        {
            bool ret = false;
            cost=int.MaxValue;
            if (source.id==AttachedToID)
            {
                ret=true;
                cost=0;
            }else if (source.SubProcess!=null)
            {
                SubProcess sb = (SubProcess)source.SubProcess;
                cost=2;
                while (sb!=null && sb.id!=this.AttachedToID)
                {
                    sb=(SubProcess)sb.SubProcess;
                    cost+=2;
                }
                ret = sb!=null&&sb.id==this.AttachedToID;
            }
            return ret;
        }
    }
}
