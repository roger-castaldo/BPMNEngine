using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Interfaces
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
        /// <param name="isAborted">returns true if emitting this signal causes the task to abort</param>
        void Signal(string signal,out bool isAborted);
        /// <summary>
        /// Called to issue an escalation from the task (this should be caught somewhere within the process by an Escalation Reciving Element)
        /// </summary>
        /// <param name="isAborted">returns true if emitting this signal causes the task to abort</param>
        void Escalate(out bool isAborted);
        /// <summary>
        /// Called to issue a message from the task (this should be caught somewhere within the process by a Message Recieving Element with a matching message defined)
        /// </summary>
        /// <param name="message">The message to emit into the process</param>
        /// <param name="isAborted">returns true if emitting this signal causes the task to abort</param>
        void EmitMessage(string message, out bool isAborted);
        /// <summary>
        /// Called to issue an exception fromn the task (this should be caught somewhere within the process by an Exception Recieving Element with a matching exception definition)
        /// </summary>
        /// <param name="error"></param>
        /// <param name="isAborted">returns true if emitting this signal causes the task to abort</param>
        void EmitError(Exception error, out bool isAborted);
        /// <summary>
        /// The variables container for this task which allows you to both obtain and modify process variables.
        /// </summary>
        IVariables Variables { get; }
        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Debug(string message);
        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Debug(string message, object[] pars);
        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Info(string message);
        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Info(string message, object[] pars);
        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Error(string message);
        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Error(string message, object[] pars);
        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Fatal(string message);
        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Fatal(string message, object[] pars);
        /// <summary>
        /// Called to log an exception 
        /// </summary>
        /// <param name="exception">The Exception that occured</param>
        /// <returns>The exception that was passed as an arguement to allow for throwing</returns>
        Exception Exception(Exception exception);
    }
}
