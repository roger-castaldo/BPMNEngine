using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn","endEvent")]
    internal class EndEvent : AEvent
    {
        public EndEvent(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override bool IsValid(out string[] err)
        {
            if (Outgoing != null)
            {
                if (Outgoing.Length > 0)
                {
                    err = new string[] { "End Events cannot have an outgoing path." };
                    return false;
                }
            }
            if (Incoming == null)
            {
                err = new string[] { "End Events must have an incoming path." };
                return false;
            }
            else if (Incoming.Length > 1)
            {
                err = new string[] { "End Events can only have 1 incoming path." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
