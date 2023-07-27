using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.State;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Gateways
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
    }
}
