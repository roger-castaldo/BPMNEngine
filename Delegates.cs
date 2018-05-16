using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    #region Ons
    public delegate void OnEventStarted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnEventCompleted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnEventError(IStepElement Event, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnTaskStarted(IStepElement task, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnTaskCompleted(IStepElement task, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnTaskError(IStepElement task, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnProcessStarted(IElement process, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnProcessCompleted(IElement process, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnProcessError(IElement process,IElement sourceElement, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnSequenceFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnMessageFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnGatewayStarted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnGatewayCompleted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnGatewayError(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnSubProcessStarted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnSubProcessCompleted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnSubProcessError(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);
    public delegate void OnStateChange(XmlDocument stateDocument);
    internal delegate void processStateChanged();
    #endregion

    #region Validations
    public delegate bool IsEventStartValid(IStepElement Event, ProcessVariablesContainer variables);
    public delegate bool IsProcessStartValid(IElement process, ProcessVariablesContainer variables);
    #endregion

    #region Conditions
    public delegate bool IsFlowValid(IElement flow, ProcessVariablesContainer variables);
    #endregion

    #region Tasks
    public delegate void ProcessBusinessRuleTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void CompleteManualTask(string taskID, ProcessVariablesContainer newVariables);
    public delegate void ErrorManualTask(string taskID, Exception ex);
    public delegate void BeginManualTask(IStepElement task, ProcessVariablesContainer variables, CompleteManualTask completeCallback, ErrorManualTask errorCallBack);
    public delegate void ProcessRecieveTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void ProcessScriptTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void ProcessSendTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void ProcessServiceTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void ProcessTask(IStepElement task, ref ProcessVariablesContainer variables);
    public delegate void CompleteUserTask(string taskID, ProcessVariablesContainer newVariables,string completedByID);
    public delegate void ErrorUserTask(string taskID, Exception ex);
    public delegate void BeginUserTask(IStepElement task, ProcessVariablesContainer variables, CompleteUserTask completeCallback, ErrorUserTask errorCallBack);
    #endregion

    #region internals
    internal delegate void ProcessStepComplete(string sourceID,string outgoingID);
    internal delegate void ProcessStepError(IElement step,Exception ex);
    #endregion

    #region Logging
    public delegate void LogLine(AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message);
    public delegate void LogException(AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception);
    #endregion
}
