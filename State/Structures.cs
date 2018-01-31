using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.State
{


    internal struct sStepSuspension
    {
        private string _id;
        public string id { get { return _id; } }

        private int _stepIndex;
        public int StepIndex { get { return _stepIndex; } }

        private DateTime _endTime;
        public DateTime EndTime { get { return _endTime; } }

        public sStepSuspension(string id, int stepIndex, TimeSpan span)
        {
            _id = id;
            _stepIndex = stepIndex;
            _endTime = DateTime.Now.Add(span);
        }

        public sStepSuspension(XmlElement elem)
        {
            _id = elem.Attributes["id"].Value;
            _stepIndex = int.Parse(elem.Attributes["stepIndex"].Value);
            _endTime = DateTime.ParseExact(elem.Attributes["endTime"].Value, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
        }

    }

    internal struct sSuspendedStep
    {
        private string _incomingID;
        public string IncomingID { get { return _incomingID; } }

        private string _elementID;
        public string ElementID { get { return _elementID; } }

        public sSuspendedStep(string incomingID, string elementID)
        {
            _incomingID = incomingID;
            _elementID = elementID;
        }
    }
}
