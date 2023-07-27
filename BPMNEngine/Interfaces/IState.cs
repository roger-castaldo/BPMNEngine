using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Interfaces
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
        IEnumerable<string> Keys { get; }
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
        IEnumerable<IElement> ActiveElements { get; }
    }
}
