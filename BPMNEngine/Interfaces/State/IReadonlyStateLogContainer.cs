namespace BPMNEngine.Interfaces.State
{
    internal interface IReadonlyStateLogContainer : IReadOnlyStateContainer
    {
        string Log { get; }
    }
}
