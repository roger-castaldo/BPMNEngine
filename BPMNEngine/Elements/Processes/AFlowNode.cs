using BPMNEngine.Attributes;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace BPMNEngine.Elements.Processes
{
    [RequiredAttribute("id")]
    internal abstract class AFlowNode : AParentElement, IStepElement
    {
        public IElement Process
        {
            get
            {
                if (Parent is Process)
                    return Parent;
                else if (Parent is AElement element)
                    return element.Parent;
                return null;
            }
        }

        public IElement SubProcess
        {
            get
            {
                if (Parent is SubProcess)
                    return Parent;
                else if (Parent is AFlowNode flowNode)
                    return flowNode.SubProcess;
                return null;
            }
        }

        public IElement Lane
        {
            get
            {
                Process p = (Process)Process;
                if (p==null)
                    return null;
                return p.Children
                    .OfType<LaneSet>()
                    .SelectMany(ls => ls.Children)
                    .FirstOrDefault(ln => ln is Lane && ((Lane)ln).Nodes.Contains(id));
            }
        }

        public IEnumerable<string> Incoming
        {
            get
            {
                return new string[] { }
                    .Concat(Children
                        .OfType<IncomingFlow>()
                        .Select(elem => elem.Value)
                    ).Concat(Definition.MessageFlows
                        .Where(msgFlow => msgFlow.targetRef==this.id)
                        .Select(msgFlow => msgFlow.id)
                    );
            }
        }

        public IEnumerable<string> Outgoing
        {
            get
            {
                return new string[] { }
                    .Concat(Children
                        .OfType<OutgoingFlow>()
                        .Select(elem => elem.Value)
                    ).Concat(Definition.MessageFlows
                        .Where(msgFlow => msgFlow.sourceRef==this.id)
                        .Select(msgFlow => msgFlow.id)
                    );
            }
        }

        public AFlowNode(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
        }
    }
}
