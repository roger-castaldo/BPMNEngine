using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Interfaces
{
    /// <summary>
    /// This interface is used to define an externally accessible manual task
    /// </summary>
    public interface IManualTask : ITask
    {
        /// <summary>
        /// Called to mark that the manual task has been completed
        /// </summary>
        void MarkComplete();
    }
}
