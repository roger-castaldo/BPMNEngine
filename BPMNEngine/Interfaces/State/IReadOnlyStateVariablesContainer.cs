namespace BPMNEngine.Interfaces.State
{
    internal interface IReadOnlyStateVariablesContainer : IReadOnlyStateContainer
    {
        object this[string name] { get; }
        IEnumerable<string> Keys { get; }
    }
}
