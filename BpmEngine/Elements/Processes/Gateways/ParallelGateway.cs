using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","parallelGateway")]
    internal class ParallelGateway : AGateway
    {
        public ParallelGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override string[] EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            return Outgoing;
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            if (!IsWaiting(path) && Incoming.Length > 1)
                return false;
            bool ret = true;
            foreach (string str in Incoming)
            {
                if (path.GetStatus(str)!=StepStatuses.Succeeded)
                {
                    ret = false;
                    break;
                }
            }
            if (ret)
            {
                int[] counts = new int[Incoming.Length];
                for(int x = 0; x<counts.Length; x++)
                    counts[x]=path.GetStepSuccessCount(Incoming[x]);
                for(int x = 1; x<counts.Length; x++)
                {
                    if (counts[0]!=counts[1])
                    {
                        ret=false;
                        break;
                    }
                }
            }
            return ret;
        }
    }
}
