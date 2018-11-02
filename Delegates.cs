using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    #region Ons
    /// <summary>
    /// This delegate is implemented to get triggered when an Event is started
    /// </summary>
    /// <param name="Event">The process Event starting</param>
    /// <param name="variables">The process variables at the time of the Event Start</param>
    public delegate void OnEventStarted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when an Event is completed
    /// </summary>
    /// <param name="Event">The process Event completing</param>
    /// <param name="variables">The process variables at the time of the Event Complete</param>
    public delegate void OnEventCompleted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when an Event has an Error
    /// </summary>
    /// <param name="Event">The process Event having an error</param>
    /// <param name="variables">The process variables at the time of the Event Error</param>
    public delegate void OnEventError(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task is started
    /// </summary>
    /// <param name="task">The process Task being started</param>
    /// <param name="variables">The process variables at the time of the Task Start</param>
    public delegate void OnTaskStarted(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task is completed
    /// </summary>
    /// <param name="task">The process Task that completed</param>
    /// <param name="variables">The process variables at the time of the Task Complete</param>
    public delegate void OnTaskCompleted(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task has an Error
    /// </summary>
    /// <param name="task">The process Task having an error</param>
    /// <param name="variables">The process variables at the time of the Task Error</param>
    public delegate void OnTaskError(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process is started
    /// </summary>
    /// <param name="process">The Process being started</param>
    /// <param name="variables">The process variables at the the time of the Process Start</param>
    public delegate void OnProcessStarted(IElement process, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process is completed
    /// </summary>
    /// <param name="process">The Process that completed</param>
    /// <param name="variables">The process variables at the time of the Process Complete</param>
    public delegate void OnProcessCompleted(IElement process, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process has an Error
    /// </summary>
    /// <param name="process">The process that the error is contained in</param>
    /// <param name="sourceElement">The Process Element that is the source of the error</param>
    /// <param name="variables">The process variables at the time of the Error</param>
    public delegate void OnProcessError(IElement process,IElement sourceElement, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Sequence Flow completes
    /// </summary>
    /// <param name="flow">The process Sequence Flow completed</param>
    /// <param name="variables">The process varialbes at the time of the Sequence Flow Complete</param>
    public delegate void OnSequenceFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Message Flow completed
    /// </summary>
    /// <param name="flow">The process Message Flow that completed</param>
    /// <param name="variables">The process variables at the time of the Message Flow Complete</param>
    public delegate void OnMessageFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway is starting
    /// </summary>
    /// <param name="gateway">The process Gateway that was started</param>
    /// <param name="variables">The process variables at the time of the Gateway Start</param>
    public delegate void OnGatewayStarted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway is completed
    /// </summary>
    /// <param name="gateway">The process Gateway that completed</param>
    /// <param name="variables">The process variables at the time of the Gateway Completing</param>
    public delegate void OnGatewayCompleted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway has an Error
    /// </summary>
    /// <param name="gateway">The process Gateway that had an error</param>
    /// <param name="variables">The process variables at the time of the Gateway Error</param>
    public delegate void OnGatewayError(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess is starting
    /// </summary>
    /// <param name="SubProcess">The SubProcess that started</param>
    /// <param name="variables">The process variables at the time of the SubProcess start</param>
    public delegate void OnSubProcessStarted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess is completed
    /// </summary>
    /// <param name="SubProcess">The SubProcess that completed</param>
    /// <param name="variables">The process variables at the time of the SubProcess completing</param>
    public delegate void OnSubProcessCompleted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess has an error
    /// </summary>
    /// <param name="SubProcess">The SubProcess that had the error</param>
    /// <param name="variables">The process variables at the time of the SubProcess having an error</param>
    public delegate void OnSubProcessError(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when the Process State changes
    /// </summary>
    /// <param name="stateDocument">The XML Document containing the Process State</param>
    public delegate void OnStateChange(XmlDocument stateDocument);
    internal delegate void processStateChanged();
    #endregion

    #region Validations

    /// <summary>
    /// This delegate is implemented to get triggered when determining if an Event Start is valid (i.e. can this event start)
    /// </summary>
    /// <param name="Event">The process Event that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    public delegate bool IsEventStartValid(IStepElement Event, ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when determining if a Process is valid to Start
    /// </summary>
    /// <param name="process">The Process that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    public delegate bool IsProcessStartValid(IElement process, ProcessVariablesContainer variables);
    #endregion

    #region Conditions

    /// <summary>
    /// This delegate is implemented to get triggered when determining if a Flow is a valid path
    /// </summary>
    /// <param name="flow">The process Flow that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    public delegate bool IsFlowValid(IElement flow, ProcessVariablesContainer variables);
    #endregion

    #region Tasks

    /// <summary>
    /// This delegate is implemented to process a Business Rule Task
    /// </summary>
    /// <param name="task">The Business Rule Task being processed</param>
    /// <param name="variables">The process variables</param>
    public delegate void ProcessBusinessRuleTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is called when a Manual Task has been completed
    /// </summary>
    /// <param name="taskID">The ID of the manual task being completed</param>
    /// <param name="newVariables">The process variables to supply back to the process</param>
    public delegate void CompleteManualTask(string taskID, ProcessVariablesContainer newVariables);

    /// <summary>
    /// This delegate is called when a Manual Task has been errored
    /// </summary>
    /// <param name="taskID">The ID of the manual task that had an error</param>
    /// <param name="ex">The exception thrown</param>
    public delegate void ErrorManualTask(string taskID, Exception ex);

    /// <summary>
    /// This delegate is implemented to be called when a Manual Task needs to be executed
    /// </summary>
    /// <param name="task">The Manual Task to execute</param>
    /// <param name="variables">The process variables for the task</param>
    /// <param name="completeCallback">The delegate to call when the manual task is completed</param>
    /// <param name="errorCallBack">The delegate to call when the manual task has an error</param>
    public delegate void BeginManualTask(IStepElement task, ProcessVariablesContainer variables, CompleteManualTask completeCallback, ErrorManualTask errorCallBack);

    /// <summary>
    /// This delegate is implemented to be called when a Process Recieve Task needs to be executed
    /// </summary>
    /// <param name="task">The Process Recieve Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessRecieveTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Script Task needs to be executed
    /// </summary>
    /// <param name="task">The Process Script Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessScriptTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Send Task needs to be executed
    /// </summary>
    /// <param name="task">The Process Send Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessSendTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Service Task needs to be executed
    /// </summary>
    /// <param name="task">The Process Service Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessServiceTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Task needs to be executed
    /// </summary>
    /// <param name="task">The Process Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is called when a User Task has been completed 
    /// </summary>
    /// <param name="taskID">The ID of the task completed</param>
    /// <param name="newVariables">The process variables to submit into the process</param>
    /// <param name="completedByID">The completed by id (optional, use null when not needed, otherwise used to indicate a user id for the completion in the state document)</param>
    public delegate void CompleteUserTask(string taskID, ProcessVariablesContainer newVariables,string completedByID);

    /// <summary>
    /// This delegate is called when a User Task has an error
    /// </summary>
    /// <param name="taskID">The ID of the task errored</param>
    /// <param name="ex">The error exception</param>
    public delegate void ErrorUserTask(string taskID, Exception ex);

    /// <summary>
    /// This delegate is implemented to be called when a Process User Task needs to be executed
    /// </summary>
    /// <param name="task">The User Task to be executed</param>
    /// <param name="variables">The process variables for the Task</param>
    /// <param name="completeCallback">The delegate callback to be called when the User Task is completed</param>
    /// <param name="errorCallBack">The delegate callback to be called when the User Task has an error</param>
    public delegate void BeginUserTask(IStepElement task, ProcessVariablesContainer variables, CompleteUserTask completeCallback, ErrorUserTask errorCallBack);
    #endregion

    #region internals
    internal delegate void ProcessStepComplete(string sourceID,string outgoingID);
    internal delegate void ProcessStepError(IElement step,Exception ex);
    #endregion

    #region Logging

    /// <summary>
    /// This delegate is implemented to be called when a Log Line Entry is made by a process
    /// </summary>
    /// <param name="assembly">The AssemblyName for the source of the line</param>
    /// <param name="fileName">The source file name for the log entry</param>
    /// <param name="lineNumber">The source line number for the log entry</param>
    /// <param name="level">The log level for the entry</param>
    /// <param name="timestamp">The timestamp of when the log entry occured</param>
    /// <param name="message">The log entry</param>
    public delegate void LogLine(AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message);

    /// <summary>
    /// This delegate is implemented to be called when a Log Exception is made by a process
    /// </summary>
    /// <param name="assembly">The AssemblyName for the source of the exception</param>
    /// <param name="fileName">The source file name for the exception</param>
    /// <param name="lineNumber">The source line number for the exception</param>
    /// <param name="timestamp">The timestamp of when the exception occured</param>
    /// <param name="exception">The exception that occured</param>
    public delegate void LogException(AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception);
    #endregion
}
