using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{
    internal class SuspendedStepContainer : AStateContainer
    {

        protected override string _ContainerName
        {
            get
            {
                return "SuspendedSteps";
            }
        }
        
        public SuspendedStepContainer(ProcessState state)
            : base(state)
        {
        }

        public IEnumerable<sStepSuspension> Steps {
            get
            {
                return ChildNodes.Cast<XmlNode>()
                    .Where(node=>node.NodeType== XmlNodeType.Element)
                    .Select(elem => new sStepSuspension((XmlElement)elem));
            }
        }

        internal void SuspendStep(string elementID, int stepIndex, TimeSpan span)
        {
            XmlElement elem = _ProduceElement("sStepSuspension");
            _SetAttribute(elem, "id", elementID);
            _SetAttribute(elem, "stepIndex", stepIndex.ToString());
            _SetAttribute(elem, "endTime", DateTime.Now.Add(span).ToString(Constants.DATETIME_FORMAT));
            _AppendElement(elem);
        }
    }
}
