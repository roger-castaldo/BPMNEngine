using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Interfaces.Elements
{
    internal interface IProcess : IParentElement
    {
        IEnumerable<StartEvent> StartEvents { get; }
        bool IsStartValid(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid);
    }
}
