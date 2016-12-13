using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","inclusiveGateway")]
    internal class InclusiveGateway : AGateway
    {
        public InclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override string[] EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid,ProcessVariablesContainer variables)
        {
            List<string> ret = new List<string>();
            foreach (string str in Outgoing)
            {
                if ((Default == null ? "" : Default) != str)
                {
                    SequenceFlow sf = (SequenceFlow)definition.LocateElement(str);
                    if (sf.IsFlowValid(isFlowValid, variables))
                        ret.Add(sf.id);
                }
            }
            if (ret.Count == 0)
            {
                if (Default!=null)
                    ret.Add(Default);
            }
            return (ret.Count==0 ? null : ret.ToArray());
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            return true;
        }
    }
}
