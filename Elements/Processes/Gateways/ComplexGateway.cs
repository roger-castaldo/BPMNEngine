using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","complexGateway")]
    internal class ComplexGateway : AGateway
    {
        public ComplexGateway(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public override string[] EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, ProcessVariablesContainer variables)
        {
            List<string> ret = new List<string>();
            foreach (string str in Outgoing)
            {
                SequenceFlow sf = (SequenceFlow)definition.LocateElement(str);
                if (sf.IsFlowValid(isFlowValid,variables))
                    ret.Add(sf.id);
            }
            if (ret.Count == 0)
            {
                if (Default != null)
                    ret.Add(Default);
            }
            return (ret.Count == 0 ? null : ret.ToArray());
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            return true;
        }
    }
}
