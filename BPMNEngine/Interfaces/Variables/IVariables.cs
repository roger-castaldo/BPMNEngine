using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces.Variables
{
    /// <summary>
    /// This interface defines a container to house the process variables and allows for editing of those variables.
    /// </summary>
    public interface IVariables
    {
        /// <summary>
        /// Called to get or set the value of a process variable
        /// </summary>
        /// <param name="name">The name of the process variable</param>
        /// <returns>The value of the process variable or null if not found</returns>
        object this[string name] { get; set; }
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
