using BPMNEngine.Elements.Processes.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces
{
    internal interface IProcess : IParentElement
    {
        IEnumerable<StartEvent> StartEvents { get; }
        bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid);
    }
}
