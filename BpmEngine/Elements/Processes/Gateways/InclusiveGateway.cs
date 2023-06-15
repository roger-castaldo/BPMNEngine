using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","inclusiveGateway")]
    internal class InclusiveGateway : AGateway
    {
        public InclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override IEnumerable<string> EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = Outgoing
                .Where(o => ((SequenceFlow)definition.LocateElement(o)).IsFlowValid(isFlowValid, variables));
            if (!result.Any() && Default==null)
                result = new string[] { Default };
            return result;
        }
    }
}
