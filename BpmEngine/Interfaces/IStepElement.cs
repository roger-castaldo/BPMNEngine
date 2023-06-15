using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces
{
    /// <summary>
    /// This interface implements Step Elements in a process.  These are elements that are containg both within a Process and a Lane and 
    /// have properties to access those objects.
    /// </summary>
    public interface IStepElement : IElement
    {
        /// <summary>
        /// The process containing this element
        /// </summary>
        IElement Process { get; }

        /// <summary>
        /// The SubProcess containing this element, if the element is within a subprocess
        /// </summary>
        IElement SubProcess { get; }

        /// <summary>
        /// The Lane within the process containing this element
        /// </summary>
        IElement Lane { get; }
    }
}
