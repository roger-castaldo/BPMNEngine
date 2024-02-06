using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.State
{
    internal interface IReadOnlyStateVariablesContainer : IReadOnlyStateContainer
    {
        object this[string name] { get; }
        IImmutableList<string> Keys { get; }
        IImmutableDictionary<string, object> AsExtract { get; }
    }
}
