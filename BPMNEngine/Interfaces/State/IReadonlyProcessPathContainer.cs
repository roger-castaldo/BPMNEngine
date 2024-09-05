using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.State
{
    internal interface IReadonlyProcessPathContainer : IReadOnlyStateContainer
    {
        IImmutableList<string> ActiveSteps { get; }
        IImmutableList<IStateStep> Steps { get; }
    }
}
