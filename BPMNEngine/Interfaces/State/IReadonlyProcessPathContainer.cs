namespace BPMNEngine.Interfaces.State
{
    internal interface IReadonlyProcessPathContainer : IReadOnlyStateContainer
    {
        IEnumerable<string> ActiveSteps { get; }
        IEnumerable<IStateStep> Steps { get; }
    }
}
