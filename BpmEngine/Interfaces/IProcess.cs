using BpmEngine.Elements.Processes.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Interfaces
{
    internal interface IProcess : IParentElement
    {
        IEnumerable<StartEvent> StartEvents { get; }
        bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid);
    }
}
