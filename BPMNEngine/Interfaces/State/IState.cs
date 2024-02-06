using BPMNEngine.Interfaces.Elements;
using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.State
{
    /// <summary>
    /// Houses the current state of a process, this will have current variables (including the Keys to know all variables contained)
    /// as well as the ability to output a string version (XML/JSON) of the state
    /// </summary>
    public interface IState
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
        IImmutableList<string> Keys { get; }
        /// <summary>
        /// Called to convert the state into a loadable xml document
        /// </summary>
        string AsXMLDocument { get; }
        /// <summary>
        /// Called to convert the state into a loadable json document
        /// </summary>
        string AsJSONDocument { get; }
        /// <summary>
        /// Called to obtain a list of all elements that are active (Started or Waiting)
        /// </summary>
        IImmutableList<IElement> ActiveElements { get; }
        /// <summary>
        /// Called to obtain a readonly dictionary of the current variables in the state
        /// </summary>
        IImmutableDictionary<string, object> Variables { get; }
        /// <summary>
        /// Called to obtain a readonly list of the step state information
        /// </summary>
        IImmutableList<IStateStep> Steps { get; }
        /// <summary>
        /// Called to obtain a copy of the current log content found within the state
        /// </summary>
        string Log { get; }
    }
}
