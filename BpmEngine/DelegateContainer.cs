using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    internal class DelegateContainer
    {
        private readonly OnElementEvent _onEventStarted;
        public OnElementEvent OnEventStarted => _onEventStarted;
        private readonly OnElementEvent _onEventCompleted;
        public OnElementEvent OnEventCompleted => _onEventCompleted;
        private readonly OnElementEvent _onEventError;
        public OnElementEvent OnEventError => _onEventError;
        private readonly OnElementEvent _onTaskStarted;
        public OnElementEvent OnTaskStarted => _onTaskStarted;
        private readonly OnElementEvent _onTaskCompleted;
        public OnElementEvent OnTaskCompleted => _onTaskCompleted;
        private readonly OnElementEvent _onTaskError;
        public OnElementEvent OnTaskError => _onTaskError;
        private readonly OnProcessEvent _onProcessStarted;
        public OnProcessEvent OnProcessStarted => _onProcessStarted;
        private readonly OnProcessEvent _onProcessCompleted;
        public OnProcessEvent OnProcessCompleted => _onProcessCompleted;
        private readonly OnProcessErrorEvent _onProcessError;
        public OnProcessErrorEvent OnProcessError => _onProcessError;
        private readonly OnFlowComplete _onSequenceFlowCompleted;
        public OnFlowComplete OnSequenceFlowCompleted => _onSequenceFlowCompleted;
        private readonly OnFlowComplete _onMessageFlowCompleted;
        internal OnFlowComplete OnMessageFlowCompleted => _onMessageFlowCompleted;
        private readonly OnFlowComplete _onAssociationFlowCompleted;
        internal OnFlowComplete OnAssociationFlowCompleted => _onAssociationFlowCompleted;
        private readonly OnElementEvent _onGatewayStarted;
        public OnElementEvent OnGatewayStarted => _onGatewayStarted;
        private readonly OnElementEvent _onGatewayCompleted;
        public OnElementEvent OnGatewayCompleted => _onGatewayCompleted;
        private readonly OnElementEvent _onGatewayError;
        public OnElementEvent OnGatewayError => _onGatewayError;
        private readonly OnElementEvent _onSubProcessStarted;
        public OnElementEvent OnSubProcessStarted => _onSubProcessStarted;
        private readonly OnElementEvent _onSubProcessCompleted;
        public OnElementEvent OnSubProcessCompleted => _onSubProcessCompleted;
        private readonly OnElementEvent _onSubProcessError;
        public OnElementEvent OnSubProcessError => _onSubProcessError;
        private readonly OnElementAborted _onStepAborted;
        public OnElementAborted OnStepAborted => _onStepAborted;
        private readonly OnStateChange _onStateChange;
        public OnStateChange OnStateChange => _onStateChange;
        private static bool _DefaultEventStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private readonly IsEventStartValid _isEventStartValid;
        public IsEventStartValid IsEventStartValid => _isEventStartValid;
        private static bool _DefaultProcessStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private readonly IsProcessStartValid _isProcessStartValid;
        public IsProcessStartValid IsProcessStartValid => _isProcessStartValid;
        private static bool _DefaultFlowValid(IElement flow, IReadonlyVariables variables) { return true; }
        private readonly IsFlowValid _isFlowValid;
        public IsFlowValid IsFlowValid => _isFlowValid;
        private readonly ProcessTask _processBusinessRuleTask;
        public ProcessTask ProcessBusinessRuleTask => _processBusinessRuleTask;
        private readonly StartManualTask _beginManualTask;
        public StartManualTask BeginManualTask => _beginManualTask;
        private readonly ProcessTask _processRecieveTask;
        public ProcessTask ProcessRecieveTask => _processRecieveTask;
        private readonly ProcessTask _processScriptTask;
        public ProcessTask ProcessScriptTask => _processScriptTask;
        private readonly ProcessTask _processSendTask;
        public ProcessTask ProcessSendTask => _processSendTask;
        private readonly ProcessTask _processServiceTask;
        public ProcessTask ProcessServiceTask => _processServiceTask;
        private readonly ProcessTask _processTask;
        public ProcessTask ProcessTask => _processTask;
        private readonly ProcessTask _callActivity;
        public ProcessTask CallActivity => _callActivity;
        private readonly StartUserTask _beginUserTask;
        public StartUserTask BeginUserTask => _beginUserTask;
        private readonly LogLine _logLine;
        public LogLine LogLine => _logLine;
        private readonly LogException _logException;
        public LogException LogException => _logException;

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
            OnFlowComplete onAssociationFlowCompleted,
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
            _onAssociationFlowCompleted=onAssociationFlowCompleted;
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
            OnFlowComplete onAssociationFlowCompleted,
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
                (onAssociationFlowCompleted==null ? _onAssociationFlowCompleted : onAssociationFlowCompleted),
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
