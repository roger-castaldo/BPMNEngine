using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    /// <summary>
    /// This interface is used to define an externall accessible task that can have extension items to allow for processing beyond the basic BPMN notation.
    /// All emissions from this Task if caught by a brounday event that is flagged to cancelActivity will cause this task to abort and no longer be usable.  
    /// In doing so it will not submit the variable changes into the process.
    /// </summary>
    public interface ITask : IStepElement
    {
        /// <summary>
        /// Called to issue a signal from the task (this should be caught somewhere within the process by a Signal Recieving Element with a matching signal defined)
        /// </summary>
        /// <param name="signal">The signal to emit into the process</param>
        void Signal(string signal);
        /// <summary>
        /// Called to issue an escalation from the task (this should be caught somewhere within the process by an Escalation Reciving Element)
        /// </summary>
        void Escalate();
        /// <summary>
        /// Called to issue a message from the task (this should be caught somewhere within the process by a Message Recieving Element with a matching message defined)
        /// </summary>
        /// <param name="message">The message to emit into the process</param>
        void EmitMessage(string message);
        /// <summary>
        /// Called to issue an exception fromn the task (this should be caught somewhere within the process by an Exception Recieving Element with a matching exception definition)
        /// </summary>
        /// <param name="error"></param>
        void EmitError(Exception error);
        /// <summary>
        /// The variables container for this task which allows you to both obtain and modify process variables.
        /// </summary>
        IVariables Variables { get; }
    }
}
