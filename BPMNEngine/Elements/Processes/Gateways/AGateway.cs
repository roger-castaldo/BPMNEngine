using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Gateways
{
    [ValidParent(typeof(IProcess))]
    internal abstract class AGateway : AFlowNode
    {
        public AGateway(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public abstract IEnumerable<string> EvaulateOutgoingPaths(Definition definition,IsFlowValid isFlowValid, IReadonlyVariables variables);
        public string Default { 
            get {
                return Outgoing.Any()&&Outgoing.Count()==1 ? Outgoing.First() : this["default"];
            } 
        }

        public override bool IsValid(out string[] err)
        {
            List<string> errs = new List<string>();
            if (!Incoming.Any())
                errs.Add("A " + this.GetType().Name + " must have at least 1 incoming path.");
            if (!Outgoing.Any())
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
