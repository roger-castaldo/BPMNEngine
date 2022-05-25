using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [RequiredAttribute("id")]
    internal abstract class AFlowNode : AParentElement, IStepElement
    {
        public string name { get { return this["name"]; } }

        public IElement Process
        {
            get
            {
                IElement elem = Parent;
                while (elem != null)
                {
                    if (elem is Process)
                        return elem;
                    else if (elem is AElement)
                        elem = ((AElement)elem).Parent;
                }
                return null;
            }
        }

        public IElement SubProcess
        {
            get
            {
                IElement elem = Parent;
                while (elem != null)
                {
                    if (elem is SubProcess)
                        return elem;
                    else if (elem is AElement)
                        elem = ((AElement)elem).Parent;
                }
                return null;
            }
        }

        public IElement Lane
        {
            get
            {
                Process p = (Process)Process;
                if (p != null)
                {
                    foreach (IElement ie in p.Children)
                    {
                        if (ie is LaneSet)
                        {
                            foreach (IElement ln in ((LaneSet)ie).Children)
                            {
                                if (ln is Lane)
                                {
                                    if (new List<string>(((Lane)ln).Nodes).Contains(id))
                                        return ln;
                                }
                            }
                        }
                    }
                }
                return null;
            }
        }

        public string[] Incoming
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (IElement elem in this.Children)
                {
                    if (elem is IncomingFlow)
                        ret.Add(((IncomingFlow)elem).Value);
                }
                foreach (MessageFlow msgFlow in Definition.MessageFlows)
                {
                    if (msgFlow.targetRef == this.id)
                        ret.Add(msgFlow.id);
                }
                return (ret.Count == 0 ? null : ret.ToArray());
            }
        }

        public string[] Outgoing
        {
            get
            {
                List<string> ret = new List<string>();
                foreach (IElement elem in this.Children)
                {
                    if (elem is OutgoingFlow)
                        ret.Add(((OutgoingFlow)elem).Value);
                }
                foreach (MessageFlow msgFlow in Definition.MessageFlows)
                {
                    if (msgFlow.sourceRef == this.id)
                        ret.Add(msgFlow.id);
                }
                return (ret.Count == 0 ? null : ret.ToArray());
            }
        }

        public AFlowNode(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
        }
    }
}
