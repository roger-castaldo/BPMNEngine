using System;
using System.Collections.Generic;
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

        public sStepSuspension[] Steps {
            get
            {
                List<sStepSuspension> ret = new List<sStepSuspension>();
                foreach (XmlElement elem in ChildNodes)
                    ret.Add(new sStepSuspension(elem));
                return ret.ToArray();
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
