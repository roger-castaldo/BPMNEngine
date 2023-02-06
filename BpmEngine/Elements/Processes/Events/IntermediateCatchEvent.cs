﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override bool _HandlesEvent(EventSubTypes evnt, AFlowNode source, IReadonlyVariables variables, out int cost)
        {
            cost=int.MaxValue;
            bool ret = true;
            SubProcess sb;
            if (Incoming.Any())
            {
                ret=false;
                if (source.Outgoing.Any(str => Incoming.Contains(str)))
                {
                    ret=true;
                    cost=1;
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
            return ret;
        }
    }
}
