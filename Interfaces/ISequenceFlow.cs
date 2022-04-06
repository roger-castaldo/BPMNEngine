﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    /// <summary>
    /// This interface is the extended interface for a sequence flow to provide additional properties that are beyond an IElement
    /// </summary>
    public interface ISequenceFlow : IElement 
    {
        /// <summary>
        /// The Condition Expression that was attached to the sequence flow, this may be an attribute or a sub element
        /// </summary>
        string conditionExpression { get; }
    }
}