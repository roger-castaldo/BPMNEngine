using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    internal interface IProcess : IParentElement
    {
        StartEvent[] StartEvents { get; }
        bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid);
    }
}
