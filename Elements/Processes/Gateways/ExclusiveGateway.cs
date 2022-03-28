using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [XMLTag("bpmn","exclusiveGateway")]
    internal class ExclusiveGateway : AGateway
    {
        public ExclusiveGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override string[] EvaulateOutgoingPaths(Definition definition, IsFlowValid isFlowValid, IReadonlyVariables variables)
        {
            string ret = null;
            foreach (string str in Outgoing)
            {
                if ((Default==null ? "" : Default)!=str)
                {
                    SequenceFlow sf = (SequenceFlow)definition.LocateElement(str);
                    if (sf.IsFlowValid(isFlowValid, variables))
                    {
                        if (ret!=null)
                            throw new MultipleOutgoingPathsException(this);
                        ret = sf.id;
                    }
                }
            }
            return (ret == null ? (this.Default == null ? null : new string[] { Default }) : new string[] { ret });
        }

        public override bool IsIncomingFlowComplete(string incomingID, ProcessPath path)
        {
            return true;
        }
    }
}
