using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal class DelegateContainer
    {
        private OnElementEvent _onEventStarted;
        public OnElementEvent OnEventStarted { get { return _onEventStarted; } }
        private OnElementEvent _onEventCompleted;
        public OnElementEvent OnEventCompleted { get { return _onEventCompleted; } }
        private OnElementEvent _onEventError;
        public OnElementEvent OnEventError { get { return _onEventError; } }
        private OnElementEvent _onTaskStarted;
        public OnElementEvent OnTaskStarted { get { return _onTaskStarted; } }
        private OnElementEvent _onTaskCompleted;
        public OnElementEvent OnTaskCompleted { get { return _onTaskCompleted; } }
        private OnElementEvent _onTaskError;
        public OnElementEvent OnTaskError { get { return _onTaskError; } }
        private OnProcessEvent _onProcessStarted;
        public OnProcessEvent OnProcessStarted { get { return _onProcessStarted; } }
        private OnProcessEvent _onProcessCompleted;
        public OnProcessEvent OnProcessCompleted { get { return _onProcessCompleted; } }
        private OnProcessErrorEvent _onProcessError;
        public OnProcessErrorEvent OnProcessError { get { return _onProcessError; } }
        private OnFlowComplete _onSequenceFlowCompleted;
        public OnFlowComplete OnSequenceFlowCompleted { get { return _onSequenceFlowCompleted; } }
        private OnFlowComplete _onMessageFlowCompleted;
        internal OnFlowComplete OnMessageFlowCompleted { get { return _onMessageFlowCompleted; } }
        private OnElementEvent _onGatewayStarted;
        public OnElementEvent OnGatewayStarted { get { return _onGatewayStarted; } }
        private OnElementEvent _onGatewayCompleted;
        public OnElementEvent OnGatewayCompleted { get { return _onGatewayCompleted; } }
        private OnElementEvent _onGatewayError;
        public OnElementEvent OnGatewayError { get { return _onGatewayError; } }
        private OnElementEvent _onSubProcessStarted;
        public OnElementEvent OnSubProcessStarted { get { return _onSubProcessStarted; } }
        private OnElementEvent _onSubProcessCompleted;
        public OnElementEvent OnSubProcessCompleted { get { return _onSubProcessCompleted; } }
        private OnElementEvent _onSubProcessError;
        public OnElementEvent OnSubProcessError { get { return _onSubProcessError; } }
        private OnElementAborted _onStepAborted;
        public OnElementAborted OnStepAborted { get { return _onStepAborted; } }
        private OnStateChange _onStateChange;
        public OnStateChange OnStateChange { get { return _onStateChange; } }
        private static bool _DefaultEventStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private IsEventStartValid _isEventStartValid;
        public IsEventStartValid IsEventStartValid { get { return _isEventStartValid; } }
        private static bool _DefaultProcessStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private IsProcessStartValid _isProcessStartValid;
        public IsProcessStartValid IsProcessStartValid { get { return _isProcessStartValid; } }
        private static bool _DefaultFlowValid(IElement flow, IReadonlyVariables variables) { return true; }
        private IsFlowValid _isFlowValid;
        public IsFlowValid IsFlowValid { get { return _isFlowValid; } }
        private ProcessTask _processBusinessRuleTask;
        public ProcessTask ProcessBusinessRuleTask { get { return _processBusinessRuleTask; } }
        private StartManualTask _beginManualTask;
        public StartManualTask BeginManualTask { get { return _beginManualTask; } }
        private ProcessTask _processRecieveTask;
        public ProcessTask ProcessRecieveTask { get { return _processRecieveTask;} }
        private ProcessTask _processScriptTask;
        public ProcessTask ProcessScriptTask { get { return _processScriptTask; } }
        private ProcessTask _processSendTask;
        public ProcessTask ProcessSendTask { get { return _processSendTask;} }
        private ProcessTask _processServiceTask;
        public ProcessTask ProcessServiceTask { get { return _processServiceTask;} }
        private ProcessTask _processTask;
        public ProcessTask ProcessTask { get { return _processTask; } }
        private ProcessTask _callActivity;
        public ProcessTask CallActivity { get { return _callActivity; } }
        private StartUserTask _beginUserTask;
        public StartUserTask BeginUserTask { get { return _beginUserTask; } }
        private LogLine _logLine;
        public LogLine LogLine { get { return _logLine; } }
        private LogException _logException;
        public LogException LogException { get { return _logException; } } 

        public DelegateContainer(LogLine logLine,
            LogException logException,
            OnElementEvent onEventStarted,
            OnElementEvent onEventCompleted,
            OnElementEvent onEventError,
            OnElementEvent onTaskStarted,
            OnElementEvent onTaskCompleted,
            OnElementEvent onTaskError,
            OnProcessEvent onProcessStarted,
            OnProcessEvent onProcessCompleted,
            OnProcessErrorEvent onProcessError,
            OnFlowComplete onSequenceFlowCompleted,
            OnFlowComplete onMessageFlowCompleted,
            OnElementEvent onGatewayStarted,
            OnElementEvent onGatewayCompleted,
            OnElementEvent onGatewayError,
            OnElementEvent onSubProcessStarted,
            OnElementEvent onSubProcessCompleted,
            OnElementEvent onSubProcessError,
            OnStateChange onStateChange,
            OnElementAborted onElementAborted,
            IsEventStartValid isEventStartValid,
            IsProcessStartValid isProcessStartValid,
            IsFlowValid isFlowValid,
            ProcessTask processBusinessRuleTask,
            StartManualTask beginManualTask,
            ProcessTask processRecieveTask,
            ProcessTask processScriptTask,
            ProcessTask processSendTask,
            ProcessTask processServiceTask,
            ProcessTask processTask,
            ProcessTask callActivity,
            StartUserTask beginUserTask)
        {
            _logLine = logLine;
            _logException = logException;
            _onEventStarted=onEventStarted;
            _onEventCompleted=onEventCompleted;
            _onEventError=onEventError;
            _onTaskStarted=onTaskStarted;
            _onTaskCompleted=onTaskCompleted;
            _onTaskError=onTaskError;
            _onProcessStarted=onProcessCompleted;
            _onProcessCompleted=onProcessCompleted;
            _onProcessError = onProcessError;
            _onSequenceFlowCompleted=onSequenceFlowCompleted;
            _onMessageFlowCompleted=onMessageFlowCompleted;
            _onGatewayStarted=onGatewayStarted;
            _onGatewayCompleted=onGatewayCompleted;
            _onGatewayError=onGatewayError;
            _onSubProcessCompleted=onSubProcessCompleted;
            _onSubProcessStarted=onSubProcessStarted;
            _onSubProcessError=onSubProcessError;
            _onStateChange=onStateChange;
            _onStepAborted = onElementAborted;
            _isEventStartValid = (isEventStartValid==null ? new IsEventStartValid(_DefaultEventStartValid) : isEventStartValid);
            _isProcessStartValid = (isProcessStartValid==null ? new IsProcessStartValid(_DefaultProcessStartValid) : isProcessStartValid);
            _isFlowValid = (isFlowValid==null ? new IsFlowValid(_DefaultFlowValid) : isFlowValid);
            _processBusinessRuleTask=processBusinessRuleTask;
            _beginManualTask=beginManualTask;
            _processRecieveTask=processRecieveTask;
            _processScriptTask=processScriptTask;
            _processSendTask=processSendTask;
            _processServiceTask=processServiceTask;
            _processTask=processTask;
            _callActivity=callActivity;
            _beginUserTask=beginUserTask;
        }

        public DelegateContainer Merge(LogLine logLine,
            LogException logException,
            OnElementEvent onEventStarted,
            OnElementEvent onEventCompleted,
            OnElementEvent onEventError,
            OnElementEvent onTaskStarted,
            OnElementEvent onTaskCompleted,
            OnElementEvent onTaskError,
            OnProcessEvent onProcessStarted,
            OnProcessEvent onProcessCompleted,
            OnProcessErrorEvent onProcessError,
            OnFlowComplete onSequenceFlowCompleted,
            OnFlowComplete onMessageFlowCompleted,
            OnElementEvent onGatewayStarted,
            OnElementEvent onGatewayCompleted,
            OnElementEvent onGatewayError,
            OnElementEvent onSubProcessStarted,
            OnElementEvent onSubProcessCompleted,
            OnElementEvent onSubProcessError,
            OnStateChange onStateChange,
            OnElementAborted onElementAborted,
            IsEventStartValid isEventStartValid,
            IsProcessStartValid isProcessStartValid,
            IsFlowValid isFlowValid,
            ProcessTask processBusinessRuleTask,
            StartManualTask beginManualTask,
            ProcessTask processRecieveTask,
            ProcessTask processScriptTask,
            ProcessTask processSendTask,
            ProcessTask processServiceTask,
            ProcessTask processTask,
            ProcessTask callActivity,
            StartUserTask beginUserTask)
        {
            return new DelegateContainer(
                (logLine==null ? _logLine : logLine),
                (logException==null ? _logException : logException),
                (onEventStarted==null ? _onEventStarted : onEventStarted),
                (onEventCompleted==null ? _onEventCompleted : onEventCompleted),
                (onEventError ==null ? _onEventError : onEventError),
                (onTaskStarted==null ? _onTaskStarted : onTaskStarted),
                (onTaskCompleted==null ? _onTaskCompleted : onTaskCompleted),
                (onTaskError==null ? _onTaskError : onTaskError),
                (onProcessStarted==null ? _onProcessStarted : onProcessStarted),
                (onProcessCompleted==null ? _onProcessCompleted : onProcessCompleted),
                (onProcessError==null ? _onProcessError : onProcessError),
                (onSequenceFlowCompleted==null ? _onSequenceFlowCompleted : onSequenceFlowCompleted),
                (onMessageFlowCompleted==null ? _onMessageFlowCompleted : onMessageFlowCompleted),
                (onGatewayStarted==null ? _onGatewayStarted : onGatewayStarted),
                (onGatewayCompleted==null ? _onGatewayCompleted : onGatewayCompleted),
                (onGatewayError==null ? _onGatewayError : onGatewayError),
                (onSubProcessStarted==null ? _onSubProcessStarted : onSubProcessStarted),
                (onSubProcessCompleted==null ? _onSubProcessCompleted : onSubProcessCompleted),
                (onSubProcessError==null ? _onSubProcessError : onSubProcessError),
                (onStateChange==null ? _onStateChange : onStateChange),
                (onElementAborted==null ? _onStepAborted : onElementAborted),
                (isEventStartValid==null ? _isEventStartValid : isEventStartValid),
                (isProcessStartValid==null ? _isProcessStartValid : isProcessStartValid),
                (isFlowValid==null ? _isFlowValid : isFlowValid),
                (processBusinessRuleTask==null ? _processBusinessRuleTask : processBusinessRuleTask),
                (beginManualTask==null ? _beginManualTask : beginManualTask),
                (processRecieveTask==null ? _processRecieveTask : processRecieveTask),
                (processScriptTask==null ? _processScriptTask : processScriptTask),
                (processSendTask==null ? _processSendTask : processSendTask),
                (processServiceTask==null ? _processServiceTask : processServiceTask),
                (processTask==null ? _processTask : processTask),
                (callActivity==null ? _callActivity : callActivity),
                (beginUserTask==null ? _beginUserTask : beginUserTask)
                );
        }
    }
}
