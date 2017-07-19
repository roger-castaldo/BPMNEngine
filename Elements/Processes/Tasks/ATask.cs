using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    internal abstract class ATask : AFlowNode
    {
        public ATask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public new string[] Outgoing
        {
            get
            {
                string[] tmp = base.Outgoing;
                if (tmp != null)
                {
                    List<string> ret = new List<string>();
                    foreach (string str in tmp)
                    {
                        bool add = true;
                        IElement afn = this.Definition.LocateElement(str);
                        string destID = null;
                        if (afn is MessageFlow)
                            destID = ((MessageFlow)afn).targetRef;
                        else if (afn is SequenceFlow)
                            destID = ((SequenceFlow)afn).targetRef;
                        if (destID != null)
                        {
                            IElement ice = this.Definition.LocateElement(destID);
                            if (ice is IntermediateCatchEvent)
                                add = false;
                        }
                        if (add)
                            ret.Add(str);
                    }
                    return (ret.Count == 0 ? null : ret.ToArray());
                }
                return tmp;
            }
        }

        public string CatchEventPath {
            get{
                if (base.Outgoing != null)
                {
                    foreach (string str in base.Outgoing)
                    {
                        IElement afn = this.Definition.LocateElement(str);
                        string destID = null;
                        if (afn is MessageFlow)
                            destID = ((MessageFlow)afn).targetRef;
                        else if (afn is SequenceFlow)
                            destID = ((SequenceFlow)afn).targetRef;
                        if (destID != null)
                        {
                            IElement ice = this.Definition.LocateElement(destID);
                            if (ice is IntermediateCatchEvent)
                                return afn.id;
                        }
                    }
                }
                return null;
            }
        }
    }
}
