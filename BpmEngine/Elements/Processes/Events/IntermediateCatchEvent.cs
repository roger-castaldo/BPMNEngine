using Microsoft.CodeAnalysis.CSharp.Syntax;
using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Events
{
    [XMLTag("bpmn", "intermediateCatchEvent")]
    internal class IntermediateCatchEvent : AHandlingEvent
    {
        public IntermediateCatchEvent(XmlElement elem, XmlPrefixMap map,AElement parent)
            : base(elem, map,parent) { }

        public override bool IsValid(out string[] err)
        {
            if (!Outgoing.Any())
            {
                err = new string[] { "Intermediate Catch Events must have an outgoing path." };
                return false;
            }else if (Outgoing.Count() != 1)
            {
                err = new string[] { "Intermediate Catch Events must have only 1 outgoing path." };
                return false;
            }
            return base.IsValid(out err);
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
