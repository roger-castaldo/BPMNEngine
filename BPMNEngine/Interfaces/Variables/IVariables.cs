namespace BPMNEngine.Interfaces.Variables
{
    /// <summary>
    /// This interface defines a container to house the process variables and allows for editing of those variables.
    /// </summary>
    public interface IVariables : IVariablesContainer, IDisposable
    {
        /// <summary>
        /// Called to get or set the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the process variable or null if not found</returns>
        new object this[string name] { get; set; }
    }
}
