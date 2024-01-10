namespace BPMNEngine.Interfaces.Variables
{
    /// <summary>
    /// This interface defines a Read Only version of the process variables container.  These are using in event delegates as the process variables
    /// cannot be changed by events.
    /// </summary>
    public interface IReadonlyVariables : IVariablesContainer
    {
        /// <summary>
        /// The error that occured, assuming this was passed to an error event delgate this will have a value
        /// </summary>
        Exception Error { get; }
    }
}
