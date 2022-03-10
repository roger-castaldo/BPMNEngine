using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Gateways
{
    [ValidParent(typeof(IProcess))]
    internal abstract class AGateway : AFlowNode
    {
        public AGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public abstract string[] EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid, IReadonlyVariables variables);
        public abstract bool IsIncomingFlowComplete(string incomingID, ProcessPath path);

        public bool IsWaiting(ProcessPath path)
        {
            return path.GetStatus(this.id) == StepStatuses.Waiting;
        }

        public string Default { get { return this["default"]; } }

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
