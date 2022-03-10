using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateCatchEvent")]
    internal class IntermediateCatchEvent : AHandlingEvent
    {
        public IntermediateCatchEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public override bool IsValid(out string[] err)
        {
            if (Outgoing == null)
            {
                err = new string[] { "Intermediate Catch Events must have an outgoing path." };
                return false;
            }else if (Outgoing.Length != 1)
            {
                err = new string[] { "Intermediate Catch Events must have only 1 outgoing path." };
                return false;
            }
            return base.IsValid(out err);
        }

        protected override bool _HandlesEvent(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables, out int cost)
        {
            cost=int.MaxValue;
            bool ret = true;
            SubProcess sb;
            if (this.Incoming!=null)
            {
                ret=false;
                if (source.Outgoing!=null)
                {
                    List<string> inc = new List<string>(this.Incoming);
                    foreach (string str in source.Outgoing)
                    {
                        if (inc.Contains(str))
                        {
                            ret=true;
                            cost=1;
                            break;
                        }
                    }
                }
            }
            else if (source.SubProcess!=null)
            {
                sb = (SubProcess)source.SubProcess;
                cost = 3;
                string sid = (this.SubProcess==null ? null : this.SubProcess.id);
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
                    while (sb!=null&&sid!=sb.id)
                    {
                        sb = (SubProcess)sb.SubProcess;
                        cost+=2;
                    }
                    if (sb==null)
                        ret=false;
                }
            }else if (this.SubProcess!=null)
            {
                cost=3;
                sb=(SubProcess)this.SubProcess;
                while (sb!=null)
                {
                    sb = (SubProcess)sb.SubProcess;
                    cost+=2;
                }
            }
            return ret;
        }
    }
}
