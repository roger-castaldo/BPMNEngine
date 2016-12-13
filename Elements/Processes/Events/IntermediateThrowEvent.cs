using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateThrowEvent")]
    internal class IntermediateThrowEvent : AEvent
    {
        public IntermediateThrowEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public override bool IsValid(out string[] err)
        {
            if (Incoming == null)
            {
                err = new string[] { "Intermediate Throw Events must have an incoming path." };
                return false;
            }else if (Incoming.Length != 1)
            {
                err = new string[] { "Intermediate Throw Events must have only 1 incoming path." };
                return false;
            }
            if (Outgoing == null)
            {
                err = new string[] { "Intermediate Throw Events must have an outgoing path." };
                return false;
            }
            else if (Outgoing.Length != 1)
            {
                err = new string[] { "Intermediate Throw Events must have only 1 outgoing path." };
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
