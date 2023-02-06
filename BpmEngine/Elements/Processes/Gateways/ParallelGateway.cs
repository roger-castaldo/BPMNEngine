using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","parallelGateway")]
    internal class ParallelGateway : AGateway
    {
        public ParallelGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override IEnumerable<string> EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            return Outgoing;
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            if (!IsWaiting(path) && Incoming.Count()>1)
                return false;
            if (Incoming.Any(i => path.GetStatus(i)!=StepStatuses.Succeeded))
                return false;
            var counts = Incoming
                .Select(i => path.GetStepSuccessCount(i));
            if (counts.Any(c => c!=counts.Max()))
                return false;
            return true;
        }
    }
}
