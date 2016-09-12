using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    internal abstract class AGateway : AFlowNode
    {
        public AGateway(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public abstract string[] EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid,ProcessVariablesContainer variables);
        public abstract bool IsIncomingFlowComplete(string incomingID, ProcessPath path);

        public string Default { get { return _GetAttributeValue("default"); } }
    }
}
