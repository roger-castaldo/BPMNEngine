using Org.Reddragonit.BpmEngine.Attributes;
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

        public override bool IsValid(out string[] err)
        {
            List<string> errs = new List<string>();
            if ((Incoming==null ? new string[0] : Incoming).Length == 0)
                errs.Add("A " + this.GetType().Name + " must have at least 1 incoming path.");
            if ((Outgoing == null ? new string[0] : Outgoing).Length == 0)
                errs.Add("A " + this.GetType().Name + " must have at least 1 outgoing path.");
            if (errs.Count > 0)
            {
                err = errs.ToArray();
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
