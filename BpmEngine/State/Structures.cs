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
        private readonly string _id;
        public string Id => _id;

        private readonly int _stepIndex;
        public int StepIndex => _stepIndex;

        private readonly DateTime _endTime;
        public DateTime EndTime => _endTime;

        public sStepSuspension(XmlElement elem)
        {
            _id = elem.Attributes["id"].Value;
            _stepIndex = int.Parse(elem.Attributes["stepIndex"].Value);
            _endTime = DateTime.ParseExact(elem.Attributes["endTime"].Value, Constants.DATETIME_FORMAT, CultureInfo.InvariantCulture);
        }

    }

    internal struct sSuspendedStep
    {
        private readonly string _incomingID;
        public string IncomingID => _incomingID;

        private readonly string _elementID;
        public string ElementID => _elementID;

        public sSuspendedStep(string incomingID, string elementID)
        {
            _incomingID = incomingID;
            _elementID = elementID;
        }
    }

    internal struct sDelayedStartEvent
    {
        private readonly string _incomingID;
        public string IncomingID => _incomingID;

        private readonly string _elementID;
        public string ElementID => _elementID;

        private readonly DateTime _startTime;
        public TimeSpan Delay => _startTime.Subtract(DateTime.Now);

        public sDelayedStartEvent(string incomingID,string elementID,DateTime startTime)
        {
            _incomingID=incomingID;
            _elementID=elementID;
            _startTime=startTime;
        }
    }
}
