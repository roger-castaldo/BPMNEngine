using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","eventBasedGateway")]
    internal class EventBasedGateway : AGateway
    {
        public EventBasedGateway(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override string[] EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, ProcessVariablesContainer variables)
        {
            return Outgoing;
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            return true;
        }
    }
}
