using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.State
{
    /// <summary>
    /// Houses the step information from a state to indicate statuses and timestamps for given elements
    /// during the execution of the procesas
    /// </summary>
    public interface IStateStep
    {
        /// <summary>
        /// The ID of the element for this step
        /// </summary>
        string ElementID { get; }
        /// <summary>
        /// The status at the point of logging
        /// </summary>
        StepStatuses Status { get; }
        /// <summary>
        /// The timestamp for the start of the step
        /// </summary>
        DateTime StartTime { get; }
        /// <summary>
        /// The ID of the element that led to this step
        /// </summary>
        string IncomingID { get; }
        /// <summary>
        /// When the element is completed this will had a value or used to house the suspension timestamp
        /// </summary>
        DateTime? EndTime { get; }
        /// <summary>
        /// When a user task is completed and the CompletedBy is set, it is housed here
        /// </summary>
        string CompletedBy { get; }
        /// <summary>
        /// The list of outgoing elements to be executed next from the completion of this element
        /// </summary>
        IImmutableList<string> OutgoingID { get;  }
    }
}
