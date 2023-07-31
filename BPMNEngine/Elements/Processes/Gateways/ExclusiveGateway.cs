﻿using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","exclusiveGateway")]
    internal class ExclusiveGateway : AGateway
    {
        public ExclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override IEnumerable<string> EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            var result = Outgoing
                .Where(o => (Default??"")!=o && ((SequenceFlow)definition.LocateElement(o)).IsFlowValid(isFlowValid, variables));
            if (result.Count()>1)
                throw new MultipleOutgoingPathsException(this);
            if (!result.Any() && Default!=null)
                result = new string[] { Default };
            return result;
        }
    }
}
