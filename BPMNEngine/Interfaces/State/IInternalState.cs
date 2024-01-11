namespace BPMNEngine.Interfaces.State
{
    internal interface IInternalState : IState
    {
        Dictionary<string, object> Variables { get; }
        IEnumerable<IStateStep> Steps {get;}
        string Log { get; }
    }
}
