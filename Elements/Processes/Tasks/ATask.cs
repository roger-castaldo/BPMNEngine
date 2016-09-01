using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
{
    internal abstract class ATask : AFlowNode
    {
        public ATask(XmlElement elem)
            : base(elem) { }
    }
}
