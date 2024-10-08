﻿namespace BPMNEngine.Interfaces.Tasks
{
    /// <summary>
    /// This interface is used to define an externally accessible User Task.
    /// </summary>
    public interface IUserTask : IManualTask
    {
        /// <summary>
        /// The User ID of the user that completed the task.  This should be set before calling MarkComplete() if there is a desire
        /// to log the user id of the individual completing the task.
        /// </summary>
        string UserID { get; set; }
    }
}
