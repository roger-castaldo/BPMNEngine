using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    /// <summary>
    /// This interface defines a Read Only version of the process variables container.  These are using in event delegates as the process variables
    /// cannot be changed by events.
    /// </summary>
    public interface IReadonlyVariables
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
        string[] Keys { get; }
        /// <summary>
        /// Called to get a list of all process variable names available, including process definition constants and runtime constants
        /// </summary>
        string[] FullKeys { get; }
        /// <summary>
        /// The error that occured, assuming this was passed to an error event delgate this will have a value
        /// </summary>
        Exception Error { get; }
    }
}
