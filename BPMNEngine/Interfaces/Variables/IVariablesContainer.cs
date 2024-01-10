namespace BPMNEngine.Interfaces.Variables
{
    /// <summary>
    /// This interface defines the base container to house the process variables
    /// </summary>
    public interface IVariablesContainer
    {
        /// <summary>
        /// Called to get the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the variable or null if not found</returns>
        object this[string name] { get; }
        /// <summary>
        /// Called to get a list of all process variable names available
        /// </summary>
        IEnumerable<string> Keys { get; }
        /// <summary>
        /// Called to get a list of all process variable names available, including process definition constants and runtime constants
        /// </summary>
        IEnumerable<string> FullKeys { get; }
    }
}
