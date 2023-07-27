using BPMNEngine.Attributes;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [ValidParent(typeof(IProcess))]
    internal abstract class ATask : AFlowNode
    {
        public ATask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        private static readonly EventSubTypes[] _blockedSubTypes = new EventSubTypes[]
        {
            EventSubTypes.Signal,
            EventSubTypes.Escalation,
            EventSubTypes.Error,
            EventSubTypes.Message,
            EventSubTypes.Link
        };

        public new IEnumerable<string> Outgoing
        {
            get
            {
                return base.Outgoing.Where(str =>
                {
                    bool add = true;
                    IElement afn = this.Definition.LocateElement(str);
                    string destID = null;
                    if (afn is MessageFlow messageFlow)
                        destID = messageFlow.targetRef;
                    else if (afn is SequenceFlow sequenceFlow)
                        destID = sequenceFlow.targetRef;
                    if (destID != null)
                    {
                        IElement ice = this.Definition.LocateElement(destID);
                        if (ice is IntermediateCatchEvent intermediateCatchEvent)
                            add = !intermediateCatchEvent.SubType.HasValue || !_blockedSubTypes.Contains(intermediateCatchEvent.SubType.Value);
                    }
                    return add;
                });
            }
        }
    }
}
