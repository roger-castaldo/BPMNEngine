using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.Elements
{
    internal interface IProcess : IParentElement
    {
        ImmutableArray<StartEvent> StartEvents { get; }
        ValueTask<bool> IsStartValidAsync(IReadonlyVariables variables, IsProcessStartValid isProcessStartValid);
    }
}
