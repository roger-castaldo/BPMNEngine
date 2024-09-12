<a name='assembly'></a>
# BPMNEngine

## Contents

- [ActiveStepsException](#T-BPMNEngine-ActiveStepsException 'BPMNEngine.ActiveStepsException')
- [BasicEvents](#T-BPMNEngine-DelegateContainers-Events-BasicEvents 'BPMNEngine.DelegateContainers.Events.BasicEvents')
  - [Completed](#P-BPMNEngine-DelegateContainers-Events-BasicEvents-Completed 'BPMNEngine.DelegateContainers.Events.BasicEvents.Completed')
  - [Error](#P-BPMNEngine-DelegateContainers-Events-BasicEvents-Error 'BPMNEngine.DelegateContainers.Events.BasicEvents.Error')
  - [Started](#P-BPMNEngine-DelegateContainers-Events-BasicEvents-Started 'BPMNEngine.DelegateContainers.Events.BasicEvents.Started')
- [BusinessProcess](#T-BPMNEngine-BusinessProcess 'BPMNEngine.BusinessProcess')
  - [#ctor(doc,constants,events,validations,tasks,logging)](#M-BPMNEngine-BusinessProcess-#ctor-System-Xml-XmlDocument,System-Collections-Generic-IEnumerable{BPMNEngine-SProcessRuntimeConstant},BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging- 'BPMNEngine.BusinessProcess.#ctor(System.Xml.XmlDocument,System.Collections.Generic.IEnumerable{BPMNEngine.SProcessRuntimeConstant},BPMNEngine.DelegateContainers.ProcessEvents,BPMNEngine.DelegateContainers.StepValidations,BPMNEngine.DelegateContainers.ProcessTasks,BPMNEngine.DelegateContainers.ProcessLogging)')
  - [Document](#P-BPMNEngine-BusinessProcess-Document 'BPMNEngine.BusinessProcess.Document')
  - [Item](#P-BPMNEngine-BusinessProcess-Item-System-String- 'BPMNEngine.BusinessProcess.Item(System.String)')
  - [BeginProcess(pars,events,validations,tasks,logging,stateLogLevel)](#M-BPMNEngine-BusinessProcess-BeginProcess-System-Collections-Generic-Dictionary{System-String,System-Object},BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel- 'BPMNEngine.BusinessProcess.BeginProcess(System.Collections.Generic.Dictionary{System.String,System.Object},BPMNEngine.DelegateContainers.ProcessEvents,BPMNEngine.DelegateContainers.StepValidations,BPMNEngine.DelegateContainers.ProcessTasks,BPMNEngine.DelegateContainers.ProcessLogging,Microsoft.Extensions.Logging.LogLevel)')
  - [Diagram(type)](#M-BPMNEngine-BusinessProcess-Diagram-Microsoft-Maui-Graphics-ImageFormat- 'BPMNEngine.BusinessProcess.Diagram(Microsoft.Maui.Graphics.ImageFormat)')
  - [Dispose()](#M-BPMNEngine-BusinessProcess-Dispose 'BPMNEngine.BusinessProcess.Dispose')
  - [Equals(obj)](#M-BPMNEngine-BusinessProcess-Equals-System-Object- 'BPMNEngine.BusinessProcess.Equals(System.Object)')
  - [ExtractProcessLog(doc)](#M-BPMNEngine-BusinessProcess-ExtractProcessLog-System-Xml-XmlDocument- 'BPMNEngine.BusinessProcess.ExtractProcessLog(System.Xml.XmlDocument)')
  - [ExtractProcessLog(reader)](#M-BPMNEngine-BusinessProcess-ExtractProcessLog-System-Text-Json-Utf8JsonReader- 'BPMNEngine.BusinessProcess.ExtractProcessLog(System.Text.Json.Utf8JsonReader)')
  - [ExtractProcessSteps(doc)](#M-BPMNEngine-BusinessProcess-ExtractProcessSteps-System-Xml-XmlDocument- 'BPMNEngine.BusinessProcess.ExtractProcessSteps(System.Xml.XmlDocument)')
  - [ExtractProcessSteps(reader)](#M-BPMNEngine-BusinessProcess-ExtractProcessSteps-System-Text-Json-Utf8JsonReader- 'BPMNEngine.BusinessProcess.ExtractProcessSteps(System.Text.Json.Utf8JsonReader)')
  - [ExtractProcessVariablesFromStateDocument(doc)](#M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Xml-XmlDocument- 'BPMNEngine.BusinessProcess.ExtractProcessVariablesFromStateDocument(System.Xml.XmlDocument)')
  - [ExtractProcessVariablesFromStateDocument(doc,stepIndex)](#M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Xml-XmlDocument,System-Int32- 'BPMNEngine.BusinessProcess.ExtractProcessVariablesFromStateDocument(System.Xml.XmlDocument,System.Int32)')
  - [ExtractProcessVariablesFromStateDocument(reader)](#M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Text-Json-Utf8JsonReader- 'BPMNEngine.BusinessProcess.ExtractProcessVariablesFromStateDocument(System.Text.Json.Utf8JsonReader)')
  - [ExtractProcessVariablesFromStateDocument(reader,stepIndex)](#M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Text-Json-Utf8JsonReader,System-Int32- 'BPMNEngine.BusinessProcess.ExtractProcessVariablesFromStateDocument(System.Text.Json.Utf8JsonReader,System.Int32)')
  - [GetHashCode()](#M-BPMNEngine-BusinessProcess-GetHashCode 'BPMNEngine.BusinessProcess.GetHashCode')
  - [LoadState(doc,autoResume,events,validations,tasks,logging,stateLogLevel)](#M-BPMNEngine-BusinessProcess-LoadState-System-Xml-XmlDocument,System-Boolean,BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel- 'BPMNEngine.BusinessProcess.LoadState(System.Xml.XmlDocument,System.Boolean,BPMNEngine.DelegateContainers.ProcessEvents,BPMNEngine.DelegateContainers.StepValidations,BPMNEngine.DelegateContainers.ProcessTasks,BPMNEngine.DelegateContainers.ProcessLogging,Microsoft.Extensions.Logging.LogLevel)')
  - [LoadState(reader,autoResume,events,validations,tasks,logging,stateLogLevel)](#M-BPMNEngine-BusinessProcess-LoadState-System-Text-Json-Utf8JsonReader,System-Boolean,BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel- 'BPMNEngine.BusinessProcess.LoadState(System.Text.Json.Utf8JsonReader,System.Boolean,BPMNEngine.DelegateContainers.ProcessEvents,BPMNEngine.DelegateContainers.StepValidations,BPMNEngine.DelegateContainers.ProcessTasks,BPMNEngine.DelegateContainers.ProcessLogging,Microsoft.Extensions.Logging.LogLevel)')
- [DateString](#T-BPMNEngine-DateString 'BPMNEngine.DateString')
  - [#ctor(value)](#M-BPMNEngine-DateString-#ctor-System-String- 'BPMNEngine.DateString.#ctor(System.String)')
  - [GetTime(variables)](#M-BPMNEngine-DateString-GetTime-BPMNEngine-Interfaces-Variables-IReadonlyVariables- 'BPMNEngine.DateString.GetTime(BPMNEngine.Interfaces.Variables.IReadonlyVariables)')
- [DiagramException](#T-BPMNEngine-DiagramException 'BPMNEngine.DiagramException')
- [ElementProcessEvents](#T-BPMNEngine-DelegateContainers-Events-ElementProcessEvents 'BPMNEngine.DelegateContainers.Events.ElementProcessEvents')
  - [Completed](#P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Completed 'BPMNEngine.DelegateContainers.Events.ElementProcessEvents.Completed')
  - [Error](#P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Error 'BPMNEngine.DelegateContainers.Events.ElementProcessEvents.Error')
  - [Started](#P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Started 'BPMNEngine.DelegateContainers.Events.ElementProcessEvents.Started')
- [FlowEvents](#T-BPMNEngine-DelegateContainers-Events-FlowEvents 'BPMNEngine.DelegateContainers.Events.FlowEvents')
  - [AssociationFlow](#P-BPMNEngine-DelegateContainers-Events-FlowEvents-AssociationFlow 'BPMNEngine.DelegateContainers.Events.FlowEvents.AssociationFlow')
  - [MessageFlow](#P-BPMNEngine-DelegateContainers-Events-FlowEvents-MessageFlow 'BPMNEngine.DelegateContainers.Events.FlowEvents.MessageFlow')
  - [SequenceFlow](#P-BPMNEngine-DelegateContainers-Events-FlowEvents-SequenceFlow 'BPMNEngine.DelegateContainers.Events.FlowEvents.SequenceFlow')
- [IElement](#T-BPMNEngine-Interfaces-Elements-IElement 'BPMNEngine.Interfaces.Elements.IElement')
  - [ExtensionElement](#P-BPMNEngine-Interfaces-Elements-IElement-ExtensionElement 'BPMNEngine.Interfaces.Elements.IElement.ExtensionElement')
  - [ID](#P-BPMNEngine-Interfaces-Elements-IElement-ID 'BPMNEngine.Interfaces.Elements.IElement.ID')
  - [Item](#P-BPMNEngine-Interfaces-Elements-IElement-Item-System-String- 'BPMNEngine.Interfaces.Elements.IElement.Item(System.String)')
  - [SubNodes](#P-BPMNEngine-Interfaces-Elements-IElement-SubNodes 'BPMNEngine.Interfaces.Elements.IElement.SubNodes')
- [IFlowElement](#T-BPMNEngine-Interfaces-Elements-IFlowElement 'BPMNEngine.Interfaces.Elements.IFlowElement')
  - [SourceRef](#P-BPMNEngine-Interfaces-Elements-IFlowElement-SourceRef 'BPMNEngine.Interfaces.Elements.IFlowElement.SourceRef')
  - [TargetRef](#P-BPMNEngine-Interfaces-Elements-IFlowElement-TargetRef 'BPMNEngine.Interfaces.Elements.IFlowElement.TargetRef')
- [IManualTask](#T-BPMNEngine-Interfaces-Tasks-IManualTask 'BPMNEngine.Interfaces.Tasks.IManualTask')
  - [MarkComplete()](#M-BPMNEngine-Interfaces-Tasks-IManualTask-MarkComplete 'BPMNEngine.Interfaces.Tasks.IManualTask.MarkComplete')
- [IParentElement](#T-BPMNEngine-Interfaces-Elements-IParentElement 'BPMNEngine.Interfaces.Elements.IParentElement')
  - [Children](#P-BPMNEngine-Interfaces-Elements-IParentElement-Children 'BPMNEngine.Interfaces.Elements.IParentElement.Children')
- [IProcessInstance](#T-BPMNEngine-Interfaces-IProcessInstance 'BPMNEngine.Interfaces.IProcessInstance')
  - [CurrentState](#P-BPMNEngine-Interfaces-IProcessInstance-CurrentState 'BPMNEngine.Interfaces.IProcessInstance.CurrentState')
  - [CurrentVariables](#P-BPMNEngine-Interfaces-IProcessInstance-CurrentVariables 'BPMNEngine.Interfaces.IProcessInstance.CurrentVariables')
  - [Document](#P-BPMNEngine-Interfaces-IProcessInstance-Document 'BPMNEngine.Interfaces.IProcessInstance.Document')
  - [Item](#P-BPMNEngine-Interfaces-IProcessInstance-Item-System-String- 'BPMNEngine.Interfaces.IProcessInstance.Item(System.String)')
  - [Keys](#P-BPMNEngine-Interfaces-IProcessInstance-Keys 'BPMNEngine.Interfaces.IProcessInstance.Keys')
  - [StateLogLevel](#P-BPMNEngine-Interfaces-IProcessInstance-StateLogLevel 'BPMNEngine.Interfaces.IProcessInstance.StateLogLevel')
  - [Animate(outputVariables)](#M-BPMNEngine-Interfaces-IProcessInstance-Animate-System-Boolean- 'BPMNEngine.Interfaces.IProcessInstance.Animate(System.Boolean)')
  - [Diagram(outputVariables,type)](#M-BPMNEngine-Interfaces-IProcessInstance-Diagram-System-Boolean,Microsoft-Maui-Graphics-ImageFormat- 'BPMNEngine.Interfaces.IProcessInstance.Diagram(System.Boolean,Microsoft.Maui.Graphics.ImageFormat)')
  - [GetManualTask(taskID)](#M-BPMNEngine-Interfaces-IProcessInstance-GetManualTask-System-String- 'BPMNEngine.Interfaces.IProcessInstance.GetManualTask(System.String)')
  - [GetUserTask(taskID)](#M-BPMNEngine-Interfaces-IProcessInstance-GetUserTask-System-String- 'BPMNEngine.Interfaces.IProcessInstance.GetUserTask(System.String)')
  - [Resume()](#M-BPMNEngine-Interfaces-IProcessInstance-Resume 'BPMNEngine.Interfaces.IProcessInstance.Resume')
  - [Suspend()](#M-BPMNEngine-Interfaces-IProcessInstance-Suspend 'BPMNEngine.Interfaces.IProcessInstance.Suspend')
  - [WaitForCompletion()](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion 'BPMNEngine.Interfaces.IProcessInstance.WaitForCompletion')
  - [WaitForCompletion(millisecondsTimeout)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion-System-Int32- 'BPMNEngine.Interfaces.IProcessInstance.WaitForCompletion(System.Int32)')
  - [WaitForCompletion(timeout)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion-System-TimeSpan- 'BPMNEngine.Interfaces.IProcessInstance.WaitForCompletion(System.TimeSpan)')
  - [WaitForManualTask(taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-String,BPMNEngine-Interfaces-Tasks-IManualTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForManualTask(System.String,BPMNEngine.Interfaces.Tasks.IManualTask@)')
  - [WaitForManualTask(millisecondsTimeout,taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-Int32,System-String,BPMNEngine-Interfaces-Tasks-IManualTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForManualTask(System.Int32,System.String,BPMNEngine.Interfaces.Tasks.IManualTask@)')
  - [WaitForManualTask(timeout,taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-TimeSpan,System-String,BPMNEngine-Interfaces-Tasks-IManualTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForManualTask(System.TimeSpan,System.String,BPMNEngine.Interfaces.Tasks.IManualTask@)')
  - [WaitForUserTask(taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-String,BPMNEngine-Interfaces-Tasks-IUserTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForUserTask(System.String,BPMNEngine.Interfaces.Tasks.IUserTask@)')
  - [WaitForUserTask(millisecondsTimeout,taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-Int32,System-String,BPMNEngine-Interfaces-Tasks-IUserTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForUserTask(System.Int32,System.String,BPMNEngine.Interfaces.Tasks.IUserTask@)')
  - [WaitForUserTask(timeout,taskID,task)](#M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-TimeSpan,System-String,BPMNEngine-Interfaces-Tasks-IUserTask@- 'BPMNEngine.Interfaces.IProcessInstance.WaitForUserTask(System.TimeSpan,System.String,BPMNEngine.Interfaces.Tasks.IUserTask@)')
- [IReadonlyVariables](#T-BPMNEngine-Interfaces-Variables-IReadonlyVariables 'BPMNEngine.Interfaces.Variables.IReadonlyVariables')
  - [Error](#P-BPMNEngine-Interfaces-Variables-IReadonlyVariables-Error 'BPMNEngine.Interfaces.Variables.IReadonlyVariables.Error')
- [ISequenceFlow](#T-BPMNEngine-Interfaces-Elements-ISequenceFlow 'BPMNEngine.Interfaces.Elements.ISequenceFlow')
  - [ConditionExpression](#P-BPMNEngine-Interfaces-Elements-ISequenceFlow-ConditionExpression 'BPMNEngine.Interfaces.Elements.ISequenceFlow.ConditionExpression')
- [IState](#T-BPMNEngine-Interfaces-State-IState 'BPMNEngine.Interfaces.State.IState')
  - [ActiveElements](#P-BPMNEngine-Interfaces-State-IState-ActiveElements 'BPMNEngine.Interfaces.State.IState.ActiveElements')
  - [AsJSONDocument](#P-BPMNEngine-Interfaces-State-IState-AsJSONDocument 'BPMNEngine.Interfaces.State.IState.AsJSONDocument')
  - [AsXMLDocument](#P-BPMNEngine-Interfaces-State-IState-AsXMLDocument 'BPMNEngine.Interfaces.State.IState.AsXMLDocument')
  - [Item](#P-BPMNEngine-Interfaces-State-IState-Item-System-String- 'BPMNEngine.Interfaces.State.IState.Item(System.String)')
  - [Keys](#P-BPMNEngine-Interfaces-State-IState-Keys 'BPMNEngine.Interfaces.State.IState.Keys')
  - [Log](#P-BPMNEngine-Interfaces-State-IState-Log 'BPMNEngine.Interfaces.State.IState.Log')
  - [Steps](#P-BPMNEngine-Interfaces-State-IState-Steps 'BPMNEngine.Interfaces.State.IState.Steps')
  - [Variables](#P-BPMNEngine-Interfaces-State-IState-Variables 'BPMNEngine.Interfaces.State.IState.Variables')
- [IStateStep](#T-BPMNEngine-Interfaces-State-IStateStep 'BPMNEngine.Interfaces.State.IStateStep')
  - [CompletedBy](#P-BPMNEngine-Interfaces-State-IStateStep-CompletedBy 'BPMNEngine.Interfaces.State.IStateStep.CompletedBy')
  - [ElementID](#P-BPMNEngine-Interfaces-State-IStateStep-ElementID 'BPMNEngine.Interfaces.State.IStateStep.ElementID')
  - [EndTime](#P-BPMNEngine-Interfaces-State-IStateStep-EndTime 'BPMNEngine.Interfaces.State.IStateStep.EndTime')
  - [IncomingID](#P-BPMNEngine-Interfaces-State-IStateStep-IncomingID 'BPMNEngine.Interfaces.State.IStateStep.IncomingID')
  - [OutgoingID](#P-BPMNEngine-Interfaces-State-IStateStep-OutgoingID 'BPMNEngine.Interfaces.State.IStateStep.OutgoingID')
  - [StartTime](#P-BPMNEngine-Interfaces-State-IStateStep-StartTime 'BPMNEngine.Interfaces.State.IStateStep.StartTime')
  - [Status](#P-BPMNEngine-Interfaces-State-IStateStep-Status 'BPMNEngine.Interfaces.State.IStateStep.Status')
- [IStepElement](#T-BPMNEngine-Interfaces-Elements-IStepElement 'BPMNEngine.Interfaces.Elements.IStepElement')
  - [Lane](#P-BPMNEngine-Interfaces-Elements-IStepElement-Lane 'BPMNEngine.Interfaces.Elements.IStepElement.Lane')
  - [Process](#P-BPMNEngine-Interfaces-Elements-IStepElement-Process 'BPMNEngine.Interfaces.Elements.IStepElement.Process')
  - [SubProcess](#P-BPMNEngine-Interfaces-Elements-IStepElement-SubProcess 'BPMNEngine.Interfaces.Elements.IStepElement.SubProcess')
- [ITask](#T-BPMNEngine-Interfaces-Tasks-ITask 'BPMNEngine.Interfaces.Tasks.ITask')
  - [Variables](#P-BPMNEngine-Interfaces-Tasks-ITask-Variables 'BPMNEngine.Interfaces.Tasks.ITask.Variables')
  - [Debug(message)](#M-BPMNEngine-Interfaces-Tasks-ITask-Debug-System-String- 'BPMNEngine.Interfaces.Tasks.ITask.Debug(System.String)')
  - [Debug(message,pars)](#M-BPMNEngine-Interfaces-Tasks-ITask-Debug-System-String,System-Object[]- 'BPMNEngine.Interfaces.Tasks.ITask.Debug(System.String,System.Object[])')
  - [EmitError(error,isAborted)](#M-BPMNEngine-Interfaces-Tasks-ITask-EmitError-System-Exception,System-Boolean@- 'BPMNEngine.Interfaces.Tasks.ITask.EmitError(System.Exception,System.Boolean@)')
  - [EmitMessage(message,isAborted)](#M-BPMNEngine-Interfaces-Tasks-ITask-EmitMessage-System-String,System-Boolean@- 'BPMNEngine.Interfaces.Tasks.ITask.EmitMessage(System.String,System.Boolean@)')
  - [Error(message)](#M-BPMNEngine-Interfaces-Tasks-ITask-Error-System-String- 'BPMNEngine.Interfaces.Tasks.ITask.Error(System.String)')
  - [Error(message,pars)](#M-BPMNEngine-Interfaces-Tasks-ITask-Error-System-String,System-Object[]- 'BPMNEngine.Interfaces.Tasks.ITask.Error(System.String,System.Object[])')
  - [Escalate(isAborted)](#M-BPMNEngine-Interfaces-Tasks-ITask-Escalate-System-Boolean@- 'BPMNEngine.Interfaces.Tasks.ITask.Escalate(System.Boolean@)')
  - [Exception(exception)](#M-BPMNEngine-Interfaces-Tasks-ITask-Exception-System-Exception- 'BPMNEngine.Interfaces.Tasks.ITask.Exception(System.Exception)')
  - [Fatal(message)](#M-BPMNEngine-Interfaces-Tasks-ITask-Fatal-System-String- 'BPMNEngine.Interfaces.Tasks.ITask.Fatal(System.String)')
  - [Fatal(message,pars)](#M-BPMNEngine-Interfaces-Tasks-ITask-Fatal-System-String,System-Object[]- 'BPMNEngine.Interfaces.Tasks.ITask.Fatal(System.String,System.Object[])')
  - [Info(message)](#M-BPMNEngine-Interfaces-Tasks-ITask-Info-System-String- 'BPMNEngine.Interfaces.Tasks.ITask.Info(System.String)')
  - [Info(message,pars)](#M-BPMNEngine-Interfaces-Tasks-ITask-Info-System-String,System-Object[]- 'BPMNEngine.Interfaces.Tasks.ITask.Info(System.String,System.Object[])')
  - [Signal(signal,isAborted)](#M-BPMNEngine-Interfaces-Tasks-ITask-Signal-System-String,System-Boolean@- 'BPMNEngine.Interfaces.Tasks.ITask.Signal(System.String,System.Boolean@)')
- [IUserTask](#T-BPMNEngine-Interfaces-Tasks-IUserTask 'BPMNEngine.Interfaces.Tasks.IUserTask')
  - [UserID](#P-BPMNEngine-Interfaces-Tasks-IUserTask-UserID 'BPMNEngine.Interfaces.Tasks.IUserTask.UserID')
- [IVariables](#T-BPMNEngine-Interfaces-Variables-IVariables 'BPMNEngine.Interfaces.Variables.IVariables')
  - [Item](#P-BPMNEngine-Interfaces-Variables-IVariables-Item-System-String- 'BPMNEngine.Interfaces.Variables.IVariables.Item(System.String)')
- [IVariablesContainer](#T-BPMNEngine-Interfaces-Variables-IVariablesContainer 'BPMNEngine.Interfaces.Variables.IVariablesContainer')
  - [FullKeys](#P-BPMNEngine-Interfaces-Variables-IVariablesContainer-FullKeys 'BPMNEngine.Interfaces.Variables.IVariablesContainer.FullKeys')
  - [Item](#P-BPMNEngine-Interfaces-Variables-IVariablesContainer-Item-System-String- 'BPMNEngine.Interfaces.Variables.IVariablesContainer.Item(System.String)')
  - [Keys](#P-BPMNEngine-Interfaces-Variables-IVariablesContainer-Keys 'BPMNEngine.Interfaces.Variables.IVariablesContainer.Keys')
- [IntermediateProcessExcepion](#T-BPMNEngine-IntermediateProcessExcepion 'BPMNEngine.IntermediateProcessExcepion')
  - [ProcessMessage](#P-BPMNEngine-IntermediateProcessExcepion-ProcessMessage 'BPMNEngine.IntermediateProcessExcepion.ProcessMessage')
- [InvalidAttributeValueException](#T-BPMNEngine-InvalidAttributeValueException 'BPMNEngine.InvalidAttributeValueException')
- [InvalidElementException](#T-BPMNEngine-InvalidElementException 'BPMNEngine.InvalidElementException')
- [InvalidProcessDefinitionException](#T-BPMNEngine-InvalidProcessDefinitionException 'BPMNEngine.InvalidProcessDefinitionException')
  - [ProcessExceptions](#P-BPMNEngine-InvalidProcessDefinitionException-ProcessExceptions 'BPMNEngine.InvalidProcessDefinitionException.ProcessExceptions')
- [IsEventStartValid](#T-BPMNEngine-IsEventStartValid 'BPMNEngine.IsEventStartValid')
- [IsFlowValid](#T-BPMNEngine-IsFlowValid 'BPMNEngine.IsFlowValid')
- [IsProcessStartValid](#T-BPMNEngine-IsProcessStartValid 'BPMNEngine.IsProcessStartValid')
- [JintAssemblyMissingException](#T-BPMNEngine-JintAssemblyMissingException 'BPMNEngine.JintAssemblyMissingException')
  - [#ctor()](#M-BPMNEngine-JintAssemblyMissingException-#ctor 'BPMNEngine.JintAssemblyMissingException.#ctor')
- [LogException](#T-BPMNEngine-LogException 'BPMNEngine.LogException')
- [LogLine](#T-BPMNEngine-LogLine 'BPMNEngine.LogLine')
- [MissingAttributeException](#T-BPMNEngine-MissingAttributeException 'BPMNEngine.MissingAttributeException')
- [MultipleOutgoingPathsException](#T-BPMNEngine-MultipleOutgoingPathsException 'BPMNEngine.MultipleOutgoingPathsException')
- [NotSuspendedException](#T-BPMNEngine-NotSuspendedException 'BPMNEngine.NotSuspendedException')
- [OnElementAborted](#T-BPMNEngine-OnElementAborted 'BPMNEngine.OnElementAborted')
- [OnElementEvent](#T-BPMNEngine-OnElementEvent 'BPMNEngine.OnElementEvent')
- [OnFlowComplete](#T-BPMNEngine-OnFlowComplete 'BPMNEngine.OnFlowComplete')
- [OnProcessErrorEvent](#T-BPMNEngine-OnProcessErrorEvent 'BPMNEngine.OnProcessErrorEvent')
- [OnProcessEvent](#T-BPMNEngine-OnProcessEvent 'BPMNEngine.OnProcessEvent')
- [OnStateChange](#T-BPMNEngine-OnStateChange 'BPMNEngine.OnStateChange')
- [ProcessEvents](#T-BPMNEngine-DelegateContainers-ProcessEvents 'BPMNEngine.DelegateContainers.ProcessEvents')
  - [Events](#P-BPMNEngine-DelegateContainers-ProcessEvents-Events 'BPMNEngine.DelegateContainers.ProcessEvents.Events')
  - [Flows](#P-BPMNEngine-DelegateContainers-ProcessEvents-Flows 'BPMNEngine.DelegateContainers.ProcessEvents.Flows')
  - [Gateways](#P-BPMNEngine-DelegateContainers-ProcessEvents-Gateways 'BPMNEngine.DelegateContainers.ProcessEvents.Gateways')
  - [OnStateChange](#P-BPMNEngine-DelegateContainers-ProcessEvents-OnStateChange 'BPMNEngine.DelegateContainers.ProcessEvents.OnStateChange')
  - [OnStepAborted](#P-BPMNEngine-DelegateContainers-ProcessEvents-OnStepAborted 'BPMNEngine.DelegateContainers.ProcessEvents.OnStepAborted')
  - [Processes](#P-BPMNEngine-DelegateContainers-ProcessEvents-Processes 'BPMNEngine.DelegateContainers.ProcessEvents.Processes')
  - [SubProcesses](#P-BPMNEngine-DelegateContainers-ProcessEvents-SubProcesses 'BPMNEngine.DelegateContainers.ProcessEvents.SubProcesses')
  - [Tasks](#P-BPMNEngine-DelegateContainers-ProcessEvents-Tasks 'BPMNEngine.DelegateContainers.ProcessEvents.Tasks')
- [ProcessLogging](#T-BPMNEngine-DelegateContainers-ProcessLogging 'BPMNEngine.DelegateContainers.ProcessLogging')
  - [LogException](#P-BPMNEngine-DelegateContainers-ProcessLogging-LogException 'BPMNEngine.DelegateContainers.ProcessLogging.LogException')
  - [LogLine](#P-BPMNEngine-DelegateContainers-ProcessLogging-LogLine 'BPMNEngine.DelegateContainers.ProcessLogging.LogLine')
- [ProcessTask](#T-BPMNEngine-ProcessTask 'BPMNEngine.ProcessTask')
- [ProcessTasks](#T-BPMNEngine-DelegateContainers-ProcessTasks 'BPMNEngine.DelegateContainers.ProcessTasks')
  - [BeginManualTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-BeginManualTask 'BPMNEngine.DelegateContainers.ProcessTasks.BeginManualTask')
  - [BeginUserTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-BeginUserTask 'BPMNEngine.DelegateContainers.ProcessTasks.BeginUserTask')
  - [CallActivity](#P-BPMNEngine-DelegateContainers-ProcessTasks-CallActivity 'BPMNEngine.DelegateContainers.ProcessTasks.CallActivity')
  - [ProcessBusinessRuleTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessBusinessRuleTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessBusinessRuleTask')
  - [ProcessReceiveTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessReceiveTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessReceiveTask')
  - [ProcessScriptTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessScriptTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessScriptTask')
  - [ProcessSendTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessSendTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessSendTask')
  - [ProcessServiceTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessServiceTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessServiceTask')
  - [ProcessTask](#P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessTask 'BPMNEngine.DelegateContainers.ProcessTasks.ProcessTask')
- [SFile](#T-BPMNEngine-SFile 'BPMNEngine.SFile')
  - [Content](#P-BPMNEngine-SFile-Content 'BPMNEngine.SFile.Content')
  - [ContentType](#P-BPMNEngine-SFile-ContentType 'BPMNEngine.SFile.ContentType')
  - [Extension](#P-BPMNEngine-SFile-Extension 'BPMNEngine.SFile.Extension')
  - [Name](#P-BPMNEngine-SFile-Name 'BPMNEngine.SFile.Name')
  - [Equals(obj)](#M-BPMNEngine-SFile-Equals-System-Object- 'BPMNEngine.SFile.Equals(System.Object)')
  - [op_Equality(left,right)](#M-BPMNEngine-SFile-op_Equality-BPMNEngine-SFile,BPMNEngine-SFile- 'BPMNEngine.SFile.op_Equality(BPMNEngine.SFile,BPMNEngine.SFile)')
  - [op_Inequality(left,right)](#M-BPMNEngine-SFile-op_Inequality-BPMNEngine-SFile,BPMNEngine-SFile- 'BPMNEngine.SFile.op_Inequality(BPMNEngine.SFile,BPMNEngine.SFile)')
- [SProcessRuntimeConstant](#T-BPMNEngine-SProcessRuntimeConstant 'BPMNEngine.SProcessRuntimeConstant')
  - [Name](#P-BPMNEngine-SProcessRuntimeConstant-Name 'BPMNEngine.SProcessRuntimeConstant.Name')
  - [Value](#P-BPMNEngine-SProcessRuntimeConstant-Value 'BPMNEngine.SProcessRuntimeConstant.Value')
- [StartManualTask](#T-BPMNEngine-StartManualTask 'BPMNEngine.StartManualTask')
- [StartUserTask](#T-BPMNEngine-StartUserTask 'BPMNEngine.StartUserTask')
- [StepStatuses](#T-BPMNEngine-StepStatuses 'BPMNEngine.StepStatuses')
  - [Aborted](#F-BPMNEngine-StepStatuses-Aborted 'BPMNEngine.StepStatuses.Aborted')
  - [Failed](#F-BPMNEngine-StepStatuses-Failed 'BPMNEngine.StepStatuses.Failed')
  - [NotRun](#F-BPMNEngine-StepStatuses-NotRun 'BPMNEngine.StepStatuses.NotRun')
  - [Started](#F-BPMNEngine-StepStatuses-Started 'BPMNEngine.StepStatuses.Started')
  - [Succeeded](#F-BPMNEngine-StepStatuses-Succeeded 'BPMNEngine.StepStatuses.Succeeded')
  - [Suspended](#F-BPMNEngine-StepStatuses-Suspended 'BPMNEngine.StepStatuses.Suspended')
  - [Waiting](#F-BPMNEngine-StepStatuses-Waiting 'BPMNEngine.StepStatuses.Waiting')
  - [WaitingStart](#F-BPMNEngine-StepStatuses-WaitingStart 'BPMNEngine.StepStatuses.WaitingStart')
- [StepValidations](#T-BPMNEngine-DelegateContainers-StepValidations 'BPMNEngine.DelegateContainers.StepValidations')
  - [IsEventStartValid](#P-BPMNEngine-DelegateContainers-StepValidations-IsEventStartValid 'BPMNEngine.DelegateContainers.StepValidations.IsEventStartValid')
  - [IsFlowValid](#P-BPMNEngine-DelegateContainers-StepValidations-IsFlowValid 'BPMNEngine.DelegateContainers.StepValidations.IsFlowValid')
  - [IsProcessStartValid](#P-BPMNEngine-DelegateContainers-StepValidations-IsProcessStartValid 'BPMNEngine.DelegateContainers.StepValidations.IsProcessStartValid')

<a name='T-BPMNEngine-ActiveStepsException'></a>
## ActiveStepsException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when a Process Instance is being disposed but still has active steps

<a name='T-BPMNEngine-DelegateContainers-Events-BasicEvents'></a>
## BasicEvents `type`

##### Namespace

BPMNEngine.DelegateContainers.Events

##### Summary

Base class used to define the properties (event types) for a given
element types events

<a name='P-BPMNEngine-DelegateContainers-Events-BasicEvents-Completed'></a>
### Completed `property`

##### Summary

This delegate is called when a particular element completes

```
public void OnEventCompleted(IStepElement Event, IReadonlyVariables variables){
    Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
    foreach (string key in variables.FullKeys){
        Console.WriteLine("\t{0}:{1}",key,variables[key]);
    }
}
```

<a name='P-BPMNEngine-DelegateContainers-Events-BasicEvents-Error'></a>
### Error `property`

##### Summary

This delegate is called when a particular element has an error

```
    public void OnEventError(IStepElement Event, IReadonlyVariables variables){
        Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

<a name='P-BPMNEngine-DelegateContainers-Events-BasicEvents-Started'></a>
### Started `property`

##### Summary

This is the delegate called when a particular element starts

```
public void OnEventStarted(IStepElement Event, IReadonlyVariables variables);{
    Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
    foreach (string key in variables.FullKeys){
        Console.WriteLine("\t{0}:{1}",key,variables[key]);
    }
}
```

<a name='T-BPMNEngine-BusinessProcess'></a>
## BusinessProcess `type`

##### Namespace

BPMNEngine

##### Summary

This class is the primary class for the library.  It implements a Business Process by constructing the object using a BPMN 2.0 compliant definition.
This is followed by assigning delegates for handling the specific process events and then starting the process.  A process can also be suspended and 
the suspended state loaded and resumed.  It can also be cloned, including the current state and delegates in order to have more than once instance 
of the given process executing.

<a name='M-BPMNEngine-BusinessProcess-#ctor-System-Xml-XmlDocument,System-Collections-Generic-IEnumerable{BPMNEngine-SProcessRuntimeConstant},BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging-'></a>
### #ctor(doc,constants,events,validations,tasks,logging) `constructor`

##### Summary

Creates a new instance of the BusinessProcess passing it the definition, StateLogLevel, runtime constants and LogLine delegate

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The Xml Document containing the BPMN 2.0 definition |
| constants | [System.Collections.Generic.IEnumerable{BPMNEngine.SProcessRuntimeConstant}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.IEnumerable 'System.Collections.Generic.IEnumerable{BPMNEngine.SProcessRuntimeConstant}') | An array of runtime constants that are set for this particular instance of the process |
| events | [BPMNEngine.DelegateContainers.ProcessEvents](#T-BPMNEngine-DelegateContainers-ProcessEvents 'BPMNEngine.DelegateContainers.ProcessEvents') | The Process Events delegates container |
| validations | [BPMNEngine.DelegateContainers.StepValidations](#T-BPMNEngine-DelegateContainers-StepValidations 'BPMNEngine.DelegateContainers.StepValidations') | The Process Validations delegates container |
| tasks | [BPMNEngine.DelegateContainers.ProcessTasks](#T-BPMNEngine-DelegateContainers-ProcessTasks 'BPMNEngine.DelegateContainers.ProcessTasks') | The Process Tasks delegates container |
| logging | [BPMNEngine.DelegateContainers.ProcessLogging](#T-BPMNEngine-DelegateContainers-ProcessLogging 'BPMNEngine.DelegateContainers.ProcessLogging') | The Process Logging delegates container |

<a name='P-BPMNEngine-BusinessProcess-Document'></a>
### Document `property`

##### Summary

The XML Document that was supplied to the constructor containing the BPMN 2.0 definition

<a name='P-BPMNEngine-BusinessProcess-Item-System-String-'></a>
### Item `property`

##### Summary

This is used to access the values of the process runtime and definition constants

##### Returns

The value of the variable

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the variable |

<a name='M-BPMNEngine-BusinessProcess-BeginProcess-System-Collections-Generic-Dictionary{System-String,System-Object},BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel-'></a>
### BeginProcess(pars,events,validations,tasks,logging,stateLogLevel) `method`

##### Summary

Called to start and instance of the defined BusinessProcess

##### Returns

a process instance if the process was successfully started

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| pars | [System.Collections.Generic.Dictionary{System.String,System.Object}](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Collections.Generic.Dictionary 'System.Collections.Generic.Dictionary{System.String,System.Object}') | The variables to start the process with |
| events | [BPMNEngine.DelegateContainers.ProcessEvents](#T-BPMNEngine-DelegateContainers-ProcessEvents 'BPMNEngine.DelegateContainers.ProcessEvents') | The Process Events delegates container |
| validations | [BPMNEngine.DelegateContainers.StepValidations](#T-BPMNEngine-DelegateContainers-StepValidations 'BPMNEngine.DelegateContainers.StepValidations') | The Process Validations delegates container |
| tasks | [BPMNEngine.DelegateContainers.ProcessTasks](#T-BPMNEngine-DelegateContainers-ProcessTasks 'BPMNEngine.DelegateContainers.ProcessTasks') | The Process Tasks delegates container |
| logging | [BPMNEngine.DelegateContainers.ProcessLogging](#T-BPMNEngine-DelegateContainers-ProcessLogging 'BPMNEngine.DelegateContainers.ProcessLogging') | The Process Logging delegates container |
| stateLogLevel | [Microsoft.Extensions.Logging.LogLevel](#T-Microsoft-Extensions-Logging-LogLevel 'Microsoft.Extensions.Logging.LogLevel') | Used to set the logging level for the process state document |

<a name='M-BPMNEngine-BusinessProcess-Diagram-Microsoft-Maui-Graphics-ImageFormat-'></a>
### Diagram(type) `method`

##### Summary

Called to render a PNG image of the process

##### Returns

A Bitmap containing a rendered image of the process

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| type | [Microsoft.Maui.Graphics.ImageFormat](#T-Microsoft-Maui-Graphics-ImageFormat 'Microsoft.Maui.Graphics.ImageFormat') | The output image format to generate, this being jpeg,png or bmp |

<a name='M-BPMNEngine-BusinessProcess-Dispose'></a>
### Dispose() `method`

##### Summary

Called to Dispose of the given process instance.

##### Parameters

This method has no parameters.

<a name='M-BPMNEngine-BusinessProcess-Equals-System-Object-'></a>
### Equals(obj) `method`

##### Summary

Compares a given process instance to this instance to see if they are the same.

##### Returns

true if they are the same, false if they are not.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| obj | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | The Business Process instance to compare this one to. |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessLog-System-Xml-XmlDocument-'></a>
### ExtractProcessLog(doc) `method`

##### Summary

A Utility call used to extract the log from a Business Process State Document

##### Returns

The log from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The State XML Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessLog-System-Text-Json-Utf8JsonReader-'></a>
### ExtractProcessLog(reader) `method`

##### Summary

A Utility call used to extract the log from a Business Process State Document

##### Returns

The log from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [System.Text.Json.Utf8JsonReader](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader 'System.Text.Json.Utf8JsonReader') | The State Json Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessSteps-System-Xml-XmlDocument-'></a>
### ExtractProcessSteps(doc) `method`

##### Summary

A Utility call used to extract the steps from a Business Process State Document

##### Returns

The steps from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The State XML Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessSteps-System-Text-Json-Utf8JsonReader-'></a>
### ExtractProcessSteps(reader) `method`

##### Summary

A Utility call used to extract the steps from a Business Process State Document

##### Returns

The steps from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [System.Text.Json.Utf8JsonReader](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader 'System.Text.Json.Utf8JsonReader') | The State Json Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Xml-XmlDocument-'></a>
### ExtractProcessVariablesFromStateDocument(doc) `method`

##### Summary

A Utility call used to extract the variable values from a Business Process State Document.

##### Returns

The variables extracted from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The State XML Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Xml-XmlDocument,System-Int32-'></a>
### ExtractProcessVariablesFromStateDocument(doc,stepIndex) `method`

##### Summary

A Utility call used to extract the variable values from a Business Process State Document at a given step index.

##### Returns

The variables extracted from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The State XML Document file to extract the values from |
| stepIndex | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The step index to extract the values at |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Text-Json-Utf8JsonReader-'></a>
### ExtractProcessVariablesFromStateDocument(reader) `method`

##### Summary

A Utility call used to extract the variable values from a Business Process State Document.

##### Returns

The variables extracted from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [System.Text.Json.Utf8JsonReader](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader 'System.Text.Json.Utf8JsonReader') | The State Json Document file to extract the values from |

<a name='M-BPMNEngine-BusinessProcess-ExtractProcessVariablesFromStateDocument-System-Text-Json-Utf8JsonReader,System-Int32-'></a>
### ExtractProcessVariablesFromStateDocument(reader,stepIndex) `method`

##### Summary

A Utility call used to extract the variable values from a Business Process State Document at a given step index.

##### Returns

The variables extracted from the Process State Document

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [System.Text.Json.Utf8JsonReader](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader 'System.Text.Json.Utf8JsonReader') | The State Json Document file to extract the values from |
| stepIndex | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The step index to extract the values at |

<a name='M-BPMNEngine-BusinessProcess-GetHashCode'></a>
### GetHashCode() `method`

##### Summary

Returns the HashCode of the Business Process instance.

##### Returns



##### Parameters

This method has no parameters.

<a name='M-BPMNEngine-BusinessProcess-LoadState-System-Xml-XmlDocument,System-Boolean,BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel-'></a>
### LoadState(doc,autoResume,events,validations,tasks,logging,stateLogLevel) `method`

##### Summary

Called to load a Process Instance from a stored State Document

##### Returns

an instance of IProcessInstance if successful or null it failed

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| doc | [System.Xml.XmlDocument](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Xml.XmlDocument 'System.Xml.XmlDocument') | The process state document |
| autoResume | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | set true if the process was suspended and needs to resume once loaded |
| events | [BPMNEngine.DelegateContainers.ProcessEvents](#T-BPMNEngine-DelegateContainers-ProcessEvents 'BPMNEngine.DelegateContainers.ProcessEvents') | The Process Events delegates container |
| validations | [BPMNEngine.DelegateContainers.StepValidations](#T-BPMNEngine-DelegateContainers-StepValidations 'BPMNEngine.DelegateContainers.StepValidations') | The Process Validations delegates container |
| tasks | [BPMNEngine.DelegateContainers.ProcessTasks](#T-BPMNEngine-DelegateContainers-ProcessTasks 'BPMNEngine.DelegateContainers.ProcessTasks') | The Process Tasks delegates container |
| logging | [BPMNEngine.DelegateContainers.ProcessLogging](#T-BPMNEngine-DelegateContainers-ProcessLogging 'BPMNEngine.DelegateContainers.ProcessLogging') | The Process Logging delegates container |
| stateLogLevel | [Microsoft.Extensions.Logging.LogLevel](#T-Microsoft-Extensions-Logging-LogLevel 'Microsoft.Extensions.Logging.LogLevel') | Used to set the logging level for the process state document |

<a name='M-BPMNEngine-BusinessProcess-LoadState-System-Text-Json-Utf8JsonReader,System-Boolean,BPMNEngine-DelegateContainers-ProcessEvents,BPMNEngine-DelegateContainers-StepValidations,BPMNEngine-DelegateContainers-ProcessTasks,BPMNEngine-DelegateContainers-ProcessLogging,Microsoft-Extensions-Logging-LogLevel-'></a>
### LoadState(reader,autoResume,events,validations,tasks,logging,stateLogLevel) `method`

##### Summary

Called to load a Process Instance from a stored State Document

##### Returns

an instance of IProcessInstance if successful or null it failed

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| reader | [System.Text.Json.Utf8JsonReader](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader 'System.Text.Json.Utf8JsonReader') | The json based process state |
| autoResume | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | set true if the process was suspended and needs to resume once loaded |
| events | [BPMNEngine.DelegateContainers.ProcessEvents](#T-BPMNEngine-DelegateContainers-ProcessEvents 'BPMNEngine.DelegateContainers.ProcessEvents') | The Process Events delegates container |
| validations | [BPMNEngine.DelegateContainers.StepValidations](#T-BPMNEngine-DelegateContainers-StepValidations 'BPMNEngine.DelegateContainers.StepValidations') | The Process Validations delegates container |
| tasks | [BPMNEngine.DelegateContainers.ProcessTasks](#T-BPMNEngine-DelegateContainers-ProcessTasks 'BPMNEngine.DelegateContainers.ProcessTasks') | The Process Tasks delegates container |
| logging | [BPMNEngine.DelegateContainers.ProcessLogging](#T-BPMNEngine-DelegateContainers-ProcessLogging 'BPMNEngine.DelegateContainers.ProcessLogging') | The Process Logging delegates container |
| stateLogLevel | [Microsoft.Extensions.Logging.LogLevel](#T-Microsoft-Extensions-Logging-LogLevel 'Microsoft.Extensions.Logging.LogLevel') | Used to set the logging level for the process state document |

<a name='T-BPMNEngine-DateString'></a>
## DateString `type`

##### Namespace

BPMNEngine

##### Summary

This class is used to convert a date string into a datetime value, uses the similar 
concepts as the strtotime found in php

<a name='M-BPMNEngine-DateString-#ctor-System-String-'></a>
### #ctor(value) `constructor`

##### Summary

creates an instance

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| value | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | the date string that is meant to be converted |

<a name='M-BPMNEngine-DateString-GetTime-BPMNEngine-Interfaces-Variables-IReadonlyVariables-'></a>
### GetTime(variables) `method`

##### Summary

Converts the date string into an actual datetime object

##### Returns

A DateTime build from the string

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| variables | [BPMNEngine.Interfaces.Variables.IReadonlyVariables](#T-BPMNEngine-Interfaces-Variables-IReadonlyVariables 'BPMNEngine.Interfaces.Variables.IReadonlyVariables') | The process variables currently avaialbe (this is used when variables exist in the string e.g. ${variablename}) |

##### Exceptions

| Name | Description |
| ---- | ----------- |
| [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') | Occurs when the string is determined unparsable |

<a name='T-BPMNEngine-DiagramException'></a>
## DiagramException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when an error occurs generating a Process Diagram Image

<a name='T-BPMNEngine-DelegateContainers-Events-ElementProcessEvents'></a>
## ElementProcessEvents `type`

##### Namespace

BPMNEngine.DelegateContainers.Events

##### Summary

Class used to define the properties (event types) for a Process

<a name='P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Completed'></a>
### Completed `property`

##### Summary

This delegate is called when a particular element completes

```
public void OnEventCompleted(IStepElement Event, IReadonlyVariables variables){
    Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
    foreach (string key in variables.FullKeys){
        Console.WriteLine("\t{0}:{1}",key,variables[key]);
    }
}
```

<a name='P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Error'></a>
### Error `property`

##### Summary

This delegate is called when a particular element has an error

```
    public void OnEventError(IStepElement process,IStepElement Event, IReadonlyVariables variables){
        Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

<a name='P-BPMNEngine-DelegateContainers-Events-ElementProcessEvents-Started'></a>
### Started `property`

##### Summary

This is the delegate called when a particular element starts

```
public void OnEventStarted(IStepElement Event, IReadonlyVariables variables);{
    Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
    foreach (string key in variables.FullKeys){
        Console.WriteLine("\t{0}:{1}",key,variables[key]);
    }
}
```

<a name='T-BPMNEngine-DelegateContainers-Events-FlowEvents'></a>
## FlowEvents `type`

##### Namespace

BPMNEngine.DelegateContainers.Events

##### Summary

Class used to define all Flow Events that can complete

<a name='P-BPMNEngine-DelegateContainers-Events-FlowEvents-AssociationFlow'></a>
### AssociationFlow `property`

##### Summary

Called when an Association Flow completes

```
public void onAssociationFlowCompleted(IFlowElement flow, IReadonlyVariables variables){
        Console.WriteLine("Association Flow {0} has been completed with the following variables:",flow.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

<a name='P-BPMNEngine-DelegateContainers-Events-FlowEvents-MessageFlow'></a>
### MessageFlow `property`

##### Summary

Called when a Message Flow completes

<a name='P-BPMNEngine-DelegateContainers-Events-FlowEvents-SequenceFlow'></a>
### SequenceFlow `property`

##### Summary

Called when a Sequence Flow completes

```
public void OnSequenceFlowCompleted(IFlowElement flow, IReadonlyVariables variables){
        Console.WriteLine("Sequence Flow {0} has been completed with the following variables:",flow.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

<a name='T-BPMNEngine-Interfaces-Elements-IElement'></a>
## IElement `type`

##### Namespace

BPMNEngine.Interfaces.Elements

##### Summary

This interface is the parent interface for ALL process elements (which are XML nodes)

<a name='P-BPMNEngine-Interfaces-Elements-IElement-ExtensionElement'></a>
### ExtensionElement `property`

##### Summary

The extensions element if it exists.  This element is what is used in BPMN 2.0 to house additional components outside of the definition that 
woudl allow you to extend the definition beyond the business process diagraming and into more of a realm for building it.  Such as Script and Condition 
elements that this library implements.

<a name='P-BPMNEngine-Interfaces-Elements-IElement-ID'></a>
### ID `property`

##### Summary

The unique ID of the element from the process

<a name='P-BPMNEngine-Interfaces-Elements-IElement-Item-System-String-'></a>
### Item `property`

##### Summary

This is called to get an attribute value from the process element

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| attributeName | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the attribute |

<a name='P-BPMNEngine-Interfaces-Elements-IElement-SubNodes'></a>
### SubNodes `property`

##### Summary

The child XMLNodes from the process element

<a name='T-BPMNEngine-Interfaces-Elements-IFlowElement'></a>
## IFlowElement `type`

##### Namespace

BPMNEngine.Interfaces.Elements

##### Summary

An interface for a flow element which can be message, sequence, or in some cases association

<a name='P-BPMNEngine-Interfaces-Elements-IFlowElement-SourceRef'></a>
### SourceRef `property`

##### Summary

The id for the source element

<a name='P-BPMNEngine-Interfaces-Elements-IFlowElement-TargetRef'></a>
### TargetRef `property`

##### Summary

the id for the destination element

<a name='T-BPMNEngine-Interfaces-Tasks-IManualTask'></a>
## IManualTask `type`

##### Namespace

BPMNEngine.Interfaces.Tasks

##### Summary

This interface is used to define an externally accessible manual task

<a name='M-BPMNEngine-Interfaces-Tasks-IManualTask-MarkComplete'></a>
### MarkComplete() `method`

##### Summary

Called to mark that the manual task has been completed

##### Parameters

This method has no parameters.

<a name='T-BPMNEngine-Interfaces-Elements-IParentElement'></a>
## IParentElement `type`

##### Namespace

BPMNEngine.Interfaces.Elements

##### Summary

This interface is the interface for all process elements with children

<a name='P-BPMNEngine-Interfaces-Elements-IParentElement-Children'></a>
### Children `property`

##### Summary

The child elements of the given process element

<a name='T-BPMNEngine-Interfaces-IProcessInstance'></a>
## IProcessInstance `type`

##### Namespace

BPMNEngine.Interfaces

##### Summary

This interface defines a running instance of a Business Process and will have its own Unique ID.  It contains its own state 
defining the state of this instance.

<a name='P-BPMNEngine-Interfaces-IProcessInstance-CurrentState'></a>
### CurrentState `property`

##### Summary

The Process State of this instance

<a name='P-BPMNEngine-Interfaces-IProcessInstance-CurrentVariables'></a>
### CurrentVariables `property`

##### Summary

Used to get the current variable values for this process instance

<a name='P-BPMNEngine-Interfaces-IProcessInstance-Document'></a>
### Document `property`

##### Summary

The XML Document that was supplied to the constructor containing the BPMN 2.0 definition

<a name='P-BPMNEngine-Interfaces-IProcessInstance-Item-System-String-'></a>
### Item `property`

##### Summary

This is used to access the values of the process runtime and definition constants

##### Returns

The value of the variable

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the variable |

<a name='P-BPMNEngine-Interfaces-IProcessInstance-Keys'></a>
### Keys `property`

##### Summary

Called to obtain the names of all process runtime and definition constants

<a name='P-BPMNEngine-Interfaces-IProcessInstance-StateLogLevel'></a>
### StateLogLevel `property`

##### Summary

The log level to use inside the state document for logging

<a name='M-BPMNEngine-Interfaces-IProcessInstance-Animate-System-Boolean-'></a>
### Animate(outputVariables) `method`

##### Summary

Called to render an animated version of the process (output in GIF format).  This will animate each step of the process using the current state.

##### Returns

a binary array of data containing the animated GIF

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| outputVariables | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Set true to output the variables into the diagram |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-Diagram-System-Boolean,Microsoft-Maui-Graphics-ImageFormat-'></a>
### Diagram(outputVariables,type) `method`

##### Summary

Called to render a PNG image of the process at its current state

##### Returns

A Bitmap containing a rendered image of the process at its current state

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| outputVariables | [System.Boolean](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean 'System.Boolean') | Set true to include outputting variables into the image |
| type | [Microsoft.Maui.Graphics.ImageFormat](#T-Microsoft-Maui-Graphics-ImageFormat 'Microsoft.Maui.Graphics.ImageFormat') | The image format to encode the diagram in |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-GetManualTask-System-String-'></a>
### GetManualTask(taskID) `method`

##### Summary

Used to get an Active Manual Task by supplying the task ID

##### Returns

The User task specified from the business process. If it cannot be found or the Task is not waiting it will return null.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to load |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-GetUserTask-System-String-'></a>
### GetUserTask(taskID) `method`

##### Summary

Used to get an Active User Task by supplying the task ID

##### Returns

The User task specified from the business process. If it cannot be found or the Task is not waiting it will return null.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to load |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-Resume'></a>
### Resume() `method`

##### Summary

Called to Resume a suspended process.  Will fail if the process is not currently suspended.

##### Parameters

This method has no parameters.

<a name='M-BPMNEngine-Interfaces-IProcessInstance-Suspend'></a>
### Suspend() `method`

##### Summary

Called to suspend this instance

##### Parameters

This method has no parameters.

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion'></a>
### WaitForCompletion() `method`

##### Summary

Used to lock a Thread into waiting for the process to complete

##### Returns

the result of calling WaitOne on the Process Complete manual reset event

##### Parameters

This method has no parameters.

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion-System-Int32-'></a>
### WaitForCompletion(millisecondsTimeout) `method`

##### Summary

Used to lock a Thread into waiting for the process to complete including a timeout

##### Returns

the result of calling WaitOne(millisecondsTimeout) on the Process Complete manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| millisecondsTimeout | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The timeout for the process to complete |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForCompletion-System-TimeSpan-'></a>
### WaitForCompletion(timeout) `method`

##### Summary

Used to lock a Thread into waiting for the process to complete including a timeout

##### Returns

the result of calling WaitOne(timeout) on the Process Complete manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| timeout | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The timeout for the process to complete |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-String,BPMNEngine-Interfaces-Tasks-IManualTask@-'></a>
### WaitForManualTask(taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a Manual task to be ready

##### Returns

the result of calling WaitOne on the Manual Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IManualTask@](#T-BPMNEngine-Interfaces-Tasks-IManualTask@ 'BPMNEngine.Interfaces.Tasks.IManualTask@') | The Manual task specified if the task was successfully started |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-Int32,System-String,BPMNEngine-Interfaces-Tasks-IManualTask@-'></a>
### WaitForManualTask(millisecondsTimeout,taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a Manual task to be ready

##### Returns

the result of calling WaitOne(millisecondsTimeout) on the Manual Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| millisecondsTimeout | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The timeout for the Manual task to start |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IManualTask@](#T-BPMNEngine-Interfaces-Tasks-IManualTask@ 'BPMNEngine.Interfaces.Tasks.IManualTask@') | The Manual task specified if the task was successfully started |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForManualTask-System-TimeSpan,System-String,BPMNEngine-Interfaces-Tasks-IManualTask@-'></a>
### WaitForManualTask(timeout,taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a Manual task to be ready

##### Returns

the result of calling WaitOne(timeout) on the Manual Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| timeout | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The timeout for the Manual task to start |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IManualTask@](#T-BPMNEngine-Interfaces-Tasks-IManualTask@ 'BPMNEngine.Interfaces.Tasks.IManualTask@') | The Manual task specified if the task was successfully started |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-String,BPMNEngine-Interfaces-Tasks-IUserTask@-'></a>
### WaitForUserTask(taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a user task to be ready

##### Returns

the result of calling WaitOne on the User Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IUserTask@](#T-BPMNEngine-Interfaces-Tasks-IUserTask@ 'BPMNEngine.Interfaces.Tasks.IUserTask@') | The User task specified if the task was successfully started |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-Int32,System-String,BPMNEngine-Interfaces-Tasks-IUserTask@-'></a>
### WaitForUserTask(millisecondsTimeout,taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a user task to be ready

##### Returns

the result of calling WaitOne(millisecondsTimeout) on the User Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| millisecondsTimeout | [System.Int32](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Int32 'System.Int32') | The timeout for the user task to start |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IUserTask@](#T-BPMNEngine-Interfaces-Tasks-IUserTask@ 'BPMNEngine.Interfaces.Tasks.IUserTask@') | The User task specified if the task was successfully started |

<a name='M-BPMNEngine-Interfaces-IProcessInstance-WaitForUserTask-System-TimeSpan,System-String,BPMNEngine-Interfaces-Tasks-IUserTask@-'></a>
### WaitForUserTask(timeout,taskID,task) `method`

##### Summary

Used to lock a Thread into waiting for a user task to be ready

##### Returns

the result of calling WaitOne(timeout) on the User Task manual reset event

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| timeout | [System.TimeSpan](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.TimeSpan 'System.TimeSpan') | The timeout for the user task to start |
| taskID | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The id of the task to wait for |
| task | [BPMNEngine.Interfaces.Tasks.IUserTask@](#T-BPMNEngine-Interfaces-Tasks-IUserTask@ 'BPMNEngine.Interfaces.Tasks.IUserTask@') | The User task specified if the task was successfully started |

<a name='T-BPMNEngine-Interfaces-Variables-IReadonlyVariables'></a>
## IReadonlyVariables `type`

##### Namespace

BPMNEngine.Interfaces.Variables

##### Summary

This interface defines a Read Only version of the process variables container.  These are using in event delegates as the process variables
cannot be changed by events.

<a name='P-BPMNEngine-Interfaces-Variables-IReadonlyVariables-Error'></a>
### Error `property`

##### Summary

The error that occured, assuming this was passed to an error event delgate this will have a value

<a name='T-BPMNEngine-Interfaces-Elements-ISequenceFlow'></a>
## ISequenceFlow `type`

##### Namespace

BPMNEngine.Interfaces.Elements

##### Summary

This interface is the extended interface for a sequence flow to provide additional properties that are beyond an IElement

<a name='P-BPMNEngine-Interfaces-Elements-ISequenceFlow-ConditionExpression'></a>
### ConditionExpression `property`

##### Summary

The Condition Expression that was attached to the sequence flow, this may be an attribute or a sub element

<a name='T-BPMNEngine-Interfaces-State-IState'></a>
## IState `type`

##### Namespace

BPMNEngine.Interfaces.State

##### Summary

Houses the current state of a process, this will have current variables (including the Keys to know all variables contained)
as well as the ability to output a string version (XML/JSON) of the state

<a name='P-BPMNEngine-Interfaces-State-IState-ActiveElements'></a>
### ActiveElements `property`

##### Summary

Called to obtain a list of all elements that are active (Started or Waiting)

<a name='P-BPMNEngine-Interfaces-State-IState-AsJSONDocument'></a>
### AsJSONDocument `property`

##### Summary

Called to convert the state into a loadable json document

<a name='P-BPMNEngine-Interfaces-State-IState-AsXMLDocument'></a>
### AsXMLDocument `property`

##### Summary

Called to convert the state into a loadable xml document

<a name='P-BPMNEngine-Interfaces-State-IState-Item-System-String-'></a>
### Item `property`

##### Summary

Called to get the value of a process variable

##### Returns

The value of the variable or null if not found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the process variable |

<a name='P-BPMNEngine-Interfaces-State-IState-Keys'></a>
### Keys `property`

##### Summary

Called to get a list of all process variable names available

<a name='P-BPMNEngine-Interfaces-State-IState-Log'></a>
### Log `property`

##### Summary

Called to obtain a copy of the current log content found within the state

<a name='P-BPMNEngine-Interfaces-State-IState-Steps'></a>
### Steps `property`

##### Summary

Called to obtain a readonly list of the step state information

<a name='P-BPMNEngine-Interfaces-State-IState-Variables'></a>
### Variables `property`

##### Summary

Called to obtain a readonly dictionary of the current variables in the state

<a name='T-BPMNEngine-Interfaces-State-IStateStep'></a>
## IStateStep `type`

##### Namespace

BPMNEngine.Interfaces.State

##### Summary

Houses the step information from a state to indicate statuses and timestamps for given elements
during the execution of the procesas

<a name='P-BPMNEngine-Interfaces-State-IStateStep-CompletedBy'></a>
### CompletedBy `property`

##### Summary

When a user task is completed and the CompletedBy is set, it is housed here

<a name='P-BPMNEngine-Interfaces-State-IStateStep-ElementID'></a>
### ElementID `property`

##### Summary

The ID of the element for this step

<a name='P-BPMNEngine-Interfaces-State-IStateStep-EndTime'></a>
### EndTime `property`

##### Summary

When the element is completed this will had a value or used to house the suspension timestamp

<a name='P-BPMNEngine-Interfaces-State-IStateStep-IncomingID'></a>
### IncomingID `property`

##### Summary

The ID of the element that led to this step

<a name='P-BPMNEngine-Interfaces-State-IStateStep-OutgoingID'></a>
### OutgoingID `property`

##### Summary

The list of outgoing elements to be executed next from the completion of this element

<a name='P-BPMNEngine-Interfaces-State-IStateStep-StartTime'></a>
### StartTime `property`

##### Summary

The timestamp for the start of the step

<a name='P-BPMNEngine-Interfaces-State-IStateStep-Status'></a>
### Status `property`

##### Summary

The status at the point of logging

<a name='T-BPMNEngine-Interfaces-Elements-IStepElement'></a>
## IStepElement `type`

##### Namespace

BPMNEngine.Interfaces.Elements

##### Summary

This interface implements Step Elements in a process.  These are elements that are containg both within a Process and a Lane and 
have properties to access those objects.

<a name='P-BPMNEngine-Interfaces-Elements-IStepElement-Lane'></a>
### Lane `property`

##### Summary

The Lane within the process containing this element

<a name='P-BPMNEngine-Interfaces-Elements-IStepElement-Process'></a>
### Process `property`

##### Summary

The process containing this element

<a name='P-BPMNEngine-Interfaces-Elements-IStepElement-SubProcess'></a>
### SubProcess `property`

##### Summary

The SubProcess containing this element, if the element is within a subprocess

<a name='T-BPMNEngine-Interfaces-Tasks-ITask'></a>
## ITask `type`

##### Namespace

BPMNEngine.Interfaces.Tasks

##### Summary

This interface is used to define an externall accessible task that can have extension items to allow for processing beyond the basic BPMN notation.
All emissions from this Task if caught by a brounday event that is flagged to cancelActivity will cause this task to abort and no longer be usable.  
In doing so it will not submit the variable changes into the process.

<a name='P-BPMNEngine-Interfaces-Tasks-ITask-Variables'></a>
### Variables `property`

##### Summary

The variables container for this task which allows you to both obtain and modify process variables.

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Debug-System-String-'></a>
### Debug(message) `method`

##### Summary

Called to log a debug level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Debug-System-String,System-Object[]-'></a>
### Debug(message,pars) `method`

##### Summary

Called to log a debug level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string formatted message |
| pars | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The arguments to be fed into a string format call agains the message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-EmitError-System-Exception,System-Boolean@-'></a>
### EmitError(error,isAborted) `method`

##### Summary

Called to issue an exception fromn the task (this should be caught somewhere within the process by an Exception Recieving Element with a matching exception definition)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| error | [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') |  |
| isAborted | [System.Boolean@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean@ 'System.Boolean@') | returns true if emitting this signal causes the task to abort |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-EmitMessage-System-String,System-Boolean@-'></a>
### EmitMessage(message,isAborted) `method`

##### Summary

Called to issue a message from the task (this should be caught somewhere within the process by a Message Recieving Element with a matching message defined)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The message to emit into the process |
| isAborted | [System.Boolean@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean@ 'System.Boolean@') | returns true if emitting this signal causes the task to abort |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Error-System-String-'></a>
### Error(message) `method`

##### Summary

Called to log an error level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Error-System-String,System-Object[]-'></a>
### Error(message,pars) `method`

##### Summary

Called to log an error level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string formatted message |
| pars | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The arguments to be fed into a string format call agains the message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Escalate-System-Boolean@-'></a>
### Escalate(isAborted) `method`

##### Summary

Called to issue an escalation from the task (this should be caught somewhere within the process by an Escalation Reciving Element)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| isAborted | [System.Boolean@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean@ 'System.Boolean@') | returns true if emitting this signal causes the task to abort |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Exception-System-Exception-'></a>
### Exception(exception) `method`

##### Summary

Called to log an exception

##### Returns

The exception that was passed as an arguement to allow for throwing

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| exception | [System.Exception](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Exception 'System.Exception') | The Exception that occured |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Fatal-System-String-'></a>
### Fatal(message) `method`

##### Summary

Called to log a fatal level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Fatal-System-String,System-Object[]-'></a>
### Fatal(message,pars) `method`

##### Summary

Called to log a fatal level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string formatted message |
| pars | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The arguments to be fed into a string format call agains the message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Info-System-String-'></a>
### Info(message) `method`

##### Summary

Called to log an info level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Info-System-String,System-Object[]-'></a>
### Info(message,pars) `method`

##### Summary

Called to log an info level message

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| message | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The string formatted message |
| pars | [System.Object[]](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object[] 'System.Object[]') | The arguments to be fed into a string format call agains the message |

<a name='M-BPMNEngine-Interfaces-Tasks-ITask-Signal-System-String,System-Boolean@-'></a>
### Signal(signal,isAborted) `method`

##### Summary

Called to issue a signal from the task (this should be caught somewhere within the process by a Signal Recieving Element with a matching signal defined)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| signal | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The signal to emit into the process |
| isAborted | [System.Boolean@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Boolean@ 'System.Boolean@') | returns true if emitting this signal causes the task to abort |

<a name='T-BPMNEngine-Interfaces-Tasks-IUserTask'></a>
## IUserTask `type`

##### Namespace

BPMNEngine.Interfaces.Tasks

##### Summary

This interface is used to define an externally accessible User Task.

<a name='P-BPMNEngine-Interfaces-Tasks-IUserTask-UserID'></a>
### UserID `property`

##### Summary

The User ID of the user that completed the task.  This should be set before calling MarkComplete() if there is a desire
to log the user id of the individual completing the task.

<a name='T-BPMNEngine-Interfaces-Variables-IVariables'></a>
## IVariables `type`

##### Namespace

BPMNEngine.Interfaces.Variables

##### Summary

This interface defines a container to house the process variables and allows for editing of those variables.

<a name='P-BPMNEngine-Interfaces-Variables-IVariables-Item-System-String-'></a>
### Item `property`

##### Summary

Called to get or set the value of a process variable

##### Returns

The value of the process variable or null if not found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the process variable |

<a name='T-BPMNEngine-Interfaces-Variables-IVariablesContainer'></a>
## IVariablesContainer `type`

##### Namespace

BPMNEngine.Interfaces.Variables

##### Summary

This interface defines the base container to house the process variables

<a name='P-BPMNEngine-Interfaces-Variables-IVariablesContainer-FullKeys'></a>
### FullKeys `property`

##### Summary

Called to get a list of all process variable names available, including process definition constants and runtime constants

<a name='P-BPMNEngine-Interfaces-Variables-IVariablesContainer-Item-System-String-'></a>
### Item `property`

##### Summary

Called to get the value of a process variable

##### Returns

The value of the variable or null if not found

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| name | [System.String](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.String 'System.String') | The name of the process variable |

<a name='P-BPMNEngine-Interfaces-Variables-IVariablesContainer-Keys'></a>
### Keys `property`

##### Summary

Called to get a list of all process variable names available

<a name='T-BPMNEngine-IntermediateProcessExcepion'></a>
## IntermediateProcessExcepion `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown by an Intermediate Throw Event when an Error Message is defined

<a name='P-BPMNEngine-IntermediateProcessExcepion-ProcessMessage'></a>
### ProcessMessage `property`

##### Summary

Houses the error message string defined inside the ErrorMessage definition from within the process document

<a name='T-BPMNEngine-InvalidAttributeValueException'></a>
## InvalidAttributeValueException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when an attribute value is not valid on an Element found within the definition

<a name='T-BPMNEngine-InvalidElementException'></a>
## InvalidElementException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when an Element found within the definition is not valid

<a name='T-BPMNEngine-InvalidProcessDefinitionException'></a>
## InvalidProcessDefinitionException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception gets thrown on the loading of a Process Definition inside a BusinessProcess class when the definition is found to be invalid.

<a name='P-BPMNEngine-InvalidProcessDefinitionException-ProcessExceptions'></a>
### ProcessExceptions `property`

##### Summary

The Exception(s) thrown during the validation process.

<a name='T-BPMNEngine-IsEventStartValid'></a>
## IsEventStartValid `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when determining if an Event Start is valid (i.e. can this event start)

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| Event | [T:BPMNEngine.IsEventStartValid](#T-T-BPMNEngine-IsEventStartValid 'T:BPMNEngine.IsEventStartValid') | The Start Event that is being checked |

##### Example

XML:
<bpmn:startEvent id="StartEvent_0ikjhwl">
 <bpmn:extensionElements>
   <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
 </bpmn:extensionElements>
 <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
</bpmn:startEvent>

```
public bool _EventStartValid(IStepElement Event, IVariables variables){
    if (Event.ExtensionElement != null){
        foreach (XmlNode xn in Event.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="DateRange"){
                    return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
                }
            }
        }
    }
    return true;
}
```

##### Remarks

This delegate is useful when adding additional elements through the extension element that are custom to your code.  It will be called with the given starting element that can be checked against additional components to decide if the start event is valid for a process.
If valid, return true to initiate the containing process.

<a name='T-BPMNEngine-IsFlowValid'></a>
## IsFlowValid `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when determining if a Flow is a valid path

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| flow | [T:BPMNEngine.IsFlowValid](#T-T-BPMNEngine-IsFlowValid 'T:BPMNEngine.IsFlowValid') | The process Flow that is being checked |

##### Example

XML:
<bpmn:outgoing>SequenceFlow_1jma3bu
 <bpmn:extensionElements>
   <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
 </bpmn:extensionElements>
</bpmn:outgoing>

```
public bool _FlowValid(ISequenceFlow flow, IVariables variables){
    if (flow.ExtensionElement != null){
        foreach (XmlNode xn in flow.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="DateRange"){
                    return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
                }
            }
        }
    }
    return true;
}
```

##### Remarks

This delegate is useful when adding additional elements through the extension element that are custom to your code.It will be called with the given flow element that can be checked against additional components to decide if the flow is a valid next step in the process.
If valid, return true to allow the system to continue along the supplied flow.

<a name='T-BPMNEngine-IsProcessStartValid'></a>
## IsProcessStartValid `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when determining if a Process is valid to Start

##### Returns



##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| process | [T:BPMNEngine.IsProcessStartValid](#T-T-BPMNEngine-IsProcessStartValid 'T:BPMNEngine.IsProcessStartValid') | The Process that is being checked |

##### Example

XML:
<bpmn:process id="Process_1" isExecutable="false">
 ...
 <bpmn:extensionElements>
   <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
 </bpmn:extensionElements>
</bpmn:process>

```
public bool _ProcessStartValid(IElement process, IVariables variables){
    if (process.ExtensionElement != null){
        foreach (XmlNode xn in process.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="DateRange"){
                    return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
                }
            }
        }
    }
    return true;
}
```

##### Remarks

This delegate is useful when adding additional elements through the extension element that are custom to your code.It will be called with the given process element that can be checked against additional components to decide if the start is valid for a process.
If valid, return true to allow the system to continue to locate a valid start event within that process.

<a name='T-BPMNEngine-JintAssemblyMissingException'></a>
## JintAssemblyMissingException `type`

##### Namespace

BPMNEngine

##### Summary

Thrown when attempting to use Javascript without the Jint Assembly

<a name='M-BPMNEngine-JintAssemblyMissingException-#ctor'></a>
### #ctor() `constructor`

##### Summary

Thrown when attempting to use Javascript without the Jint Assembly

##### Parameters

This constructor has no parameters.

<a name='T-BPMNEngine-LogException'></a>
## LogException `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to be called when a Log Exception is made by a process.  This can be used to log exceptions externally, to a file, database, or logging engine implemented outside of the library.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| callingElement | [T:BPMNEngine.LogException](#T-T-BPMNEngine-LogException 'T:BPMNEngine.LogException') | The Process Element Calling the Log Exception (may be null) |

<a name='T-BPMNEngine-LogLine'></a>
## LogLine `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to be called when a Log Line Entry is made by a process.  This can be used to log items externally, to a file, database, or logging engine implemented outside of the library.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| callingElement | [T:BPMNEngine.LogLine](#T-T-BPMNEngine-LogLine 'T:BPMNEngine.LogLine') | The Process Element Calling the Log Line (may be null) |

<a name='T-BPMNEngine-MissingAttributeException'></a>
## MissingAttributeException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when a required attribute is missing from an Element found within the definition

<a name='T-BPMNEngine-MultipleOutgoingPathsException'></a>
## MultipleOutgoingPathsException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when an Exclusive Gateway evalutes to more than 1 outgoing path

<a name='T-BPMNEngine-NotSuspendedException'></a>
## NotSuspendedException `type`

##### Namespace

BPMNEngine

##### Summary

This Exception is thrown when a Business Process is told to Resume but is not Suspended

<a name='T-BPMNEngine-OnElementAborted'></a>
## OnElementAborted `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when a process element has been aborted.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| element | [T:BPMNEngine.OnElementAborted](#T-T-BPMNEngine-OnElementAborted 'T:BPMNEngine.OnElementAborted') | The process element that is being aborted. |

##### Example

```
    public void _OnElementAborted(IElement element,IElement source,IReadonlyVariables variables){
        Console.WriteLine("Element {0} inside process {1} has been aborted by {2} with the following variables:",element.id,element.Process.id,source.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

##### Remarks

As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.

<a name='T-BPMNEngine-OnElementEvent'></a>
## OnElementEvent `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when a process element has been started, completed or errored.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| element | [T:BPMNEngine.OnElementEvent](#T-T-BPMNEngine-OnElementEvent 'T:BPMNEngine.OnElementEvent') | The process element that is starting, has completed or has errored. |

##### Example

```
    public void _OnElementStarted(IStepElement element,IReadonlyVariables variables){
        Console.WriteLine("Element {0} inside process {1} has been started with the following variables:",element.id,element.Process.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

##### Remarks

As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.

<a name='T-BPMNEngine-OnFlowComplete'></a>
## OnFlowComplete `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when a process flow has been completed.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| element | [T:BPMNEngine.OnFlowComplete](#T-T-BPMNEngine-OnFlowComplete 'T:BPMNEngine.OnFlowComplete') | The process flow that has been completed. |

##### Example

```
    public void _OnFlowCompleted(IElement element,IReadonlyVariables variables){
        Console.WriteLine("Flow {0} inside process {1} has been started with the following variables:",element.id,element.Process.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

##### Remarks

As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.

<a name='T-BPMNEngine-OnProcessErrorEvent'></a>
## OnProcessErrorEvent `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when a Process has an Error

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| process | [T:BPMNEngine.OnProcessErrorEvent](#T-T-BPMNEngine-OnProcessErrorEvent 'T:BPMNEngine.OnProcessErrorEvent') | The process that the error is contained in |

##### Example

```
    public void _ProcessError(IElement process,IElement sourceElement, IReadonlyVariables variables){
        Console.WriteLine("Element {0} inside process {1} had the error {2} occur with the following variables:",new object[]{sourceElement.id,process.id,variables.Error.Message});
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

##### Remarks

As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.

<a name='T-BPMNEngine-OnProcessEvent'></a>
## OnProcessEvent `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when a Process has been started or completed.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| process | [T:BPMNEngine.OnProcessEvent](#T-T-BPMNEngine-OnProcessEvent 'T:BPMNEngine.OnProcessEvent') | The Process being started or completed |

##### Example

```
    public void _ProcessStarted(IElement process,IReadonlyVariables variables){
        Console.WriteLine("Process {0} has been started with the following variables:",process.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

##### Remarks

As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.

<a name='T-BPMNEngine-OnStateChange'></a>
## OnStateChange `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to get triggered when the Process State changes.  The state may not be usable externally without understanding its structure, however, capturing these events allows for the storage of a process state externally to be brought back in on a process restart.

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| currentState | [T:BPMNEngine.OnStateChange](#T-T-BPMNEngine-OnStateChange 'T:BPMNEngine.OnStateChange') | The current state of the process |

##### Example

```
    public void _StateChange(XmlDocument stateDocument){
        Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
    }
```

<a name='T-BPMNEngine-DelegateContainers-ProcessEvents'></a>
## ProcessEvents `type`

##### Namespace

BPMNEngine.DelegateContainers

##### Summary

This class is used to house all the event based delegates for a business process. 
This can be defined at either the BusinessProcess constructor level for defining it 
against all instances or at the BeginProcess level to defining it against a 
specific instance

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-Events'></a>
### Events `property`

##### Summary

Houses the delegates for Events related to a Business Process Event item

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-Flows'></a>
### Flows `property`

##### Summary

Houses the delegates for Events related to a Business Process flow item

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-Gateways'></a>
### Gateways `property`

##### Summary

Houses the delegates for Events related to a Business Process Gateway item

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-OnStateChange'></a>
### OnStateChange `property`

##### Summary

A delegate called when the Business Process Instance state document has changed

```
public void OnStateChange(XmlDocument stateDocument){
        Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
    }
```

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-OnStepAborted'></a>
### OnStepAborted `property`

##### Summary

A delegate called when an element is aborted within the Business Process

```
public void OnStepAborted(IElement element, IElement source, IReadonlyVariables variables){
        Console.WriteLine("Element {0} inside process {1} has been aborted by {2} with the following variables:",element.id,element.Process.id,source.id);
        foreach (string key in variables.FullKeys){
            Console.WriteLine("\t{0}:{1}",key,variables[key]);
        }
    }
```

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-Processes'></a>
### Processes `property`

##### Summary

Houses the delegates for Events related to a Business Process Process item

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-SubProcesses'></a>
### SubProcesses `property`

##### Summary

Houses the delegates for Events related to a Business Process SubProcess item

<a name='P-BPMNEngine-DelegateContainers-ProcessEvents-Tasks'></a>
### Tasks `property`

##### Summary

Houses the delegates for Events related to a Business Process Task item

<a name='T-BPMNEngine-DelegateContainers-ProcessLogging'></a>
## ProcessLogging `type`

##### Namespace

BPMNEngine.DelegateContainers

##### Summary

This class is used to house all the Logging delegates for a business process. 
This can be defined at either the BusinessProcess constructor level for defining it 
against all instances or at the BeginProcess level to defining it against a 
specific instance

<a name='P-BPMNEngine-DelegateContainers-ProcessLogging-LogException'></a>
### LogException `property`

##### Summary

A delegate called to append a logged exception from the process

<a name='P-BPMNEngine-DelegateContainers-ProcessLogging-LogLine'></a>
### LogLine `property`

##### Summary

A delegate called to append a log line entry from the process

<a name='T-BPMNEngine-ProcessTask'></a>
## ProcessTask `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to process a Process Task (This can be a Business Rule, Script, Send, Service and Task)

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| task | [T:BPMNEngine.ProcessTask](#T-T-BPMNEngine-ProcessTask 'T:BPMNEngine.ProcessTask') | The Task being processed |

##### Example

XML:
<bpmn:businessRuleTask id="BusinessRule_0ikjhwl">
 <bpmn:extensionElements>
   <Analysis outputVariable="averageCost" inputs="Cost" formula="Average"/>
   <Analysis outputVariable="totalQuantity" inputs="Quantity" formula="Sum"/>
 </bpmn:extensionElements>
 <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
</bpmn:startEvent>

```
public void _ProcessBusinessRuleTask(ITask task)
    if (task.ExtensionElement != null){
        foreach (XmlNode xn in task.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="Analysis"){
                    switch(xn.Attributes["formula"].Value){
                        case "Average":
                            decimal avgSum=0;
                            decimal avgCount=0;
                            foreach (Hashtable item in task.Variables["Items"]){
                                if (item.ContainsKey(xn.Attributes["inputs"].Value)){
                                    avgSum+=(decimal)item[xn.Attributes["inputs"].Value];
                                    avgCount++;
                                }
                            }
                            task.Variables[xn.Attriubtes["outputVariable"].Value] = avgSum/avgCount;
                            break;
                        case "Sum":
                            decimal sum=0;
                            foreach (Hashtable item in task.Variables["Items"]){
                                if (item.ContainsKey(xn.Attributes["inputs"].Value)){
                                    sum+=(decimal)item[xn.Attributes["inputs"].Value];
                                }
                            }
                            task.Variables[xn.Attriubtes["outputVariable"].Value] = sum;
                            break;
                    }
                }
            }
        }
    }
}
```

##### Remarks

Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.

<a name='T-BPMNEngine-DelegateContainers-ProcessTasks'></a>
## ProcessTasks `type`

##### Namespace

BPMNEngine.DelegateContainers

##### Summary

This class is used to house all the Tasks delegates for a business process. 
This can be defined at either the BusinessProcess constructor level for defining it 
against all instances or at the BeginProcess level to defining it against a 
specific instance

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-BeginManualTask'></a>
### BeginManualTask `property`

##### Summary

A delegate called to start a Manual Task

```
<![CDATA[
            XML:
            <bpmn:manualTask id="ManualTask_0ikjhwl">
             <bpmn:extensionElements>
               <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
             </bpmn:extensionElements>
             <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
            </bpmn:startEvent>
            ]]>
```

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-BeginUserTask'></a>
### BeginUserTask `property`

##### Summary

A delegate called to start a User Task

```
<![CDATA[
            XML:
            <bpmn:userTask id="UserTask_0ikjhwl">
             <bpmn:extensionElements>
               <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
             </bpmn:extensionElements>
             <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
            </bpmn:startEvent>
            ]]>
```

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-CallActivity'></a>
### CallActivity `property`

##### Summary

A delegate called to execute a CallActivity

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessBusinessRuleTask'></a>
### ProcessBusinessRuleTask `property`

##### Summary

A delegate called to execute a Business Rule Task

```
<![CDATA[
            XML:
            <bpmn:businessRuleTask id="BusinessRule_0ikjhwl">
             <bpmn:extensionElements>
               <Analysis outputVariable="averageCost" inputs="Cost" formula="Average"/>
               <Analysis outputVariable="totalQuantity" inputs="Quantity" formula="Sum"/>
             </bpmn:extensionElements>
             <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
            </bpmn:startEvent>
            ]]>
```

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessReceiveTask'></a>
### ProcessReceiveTask `property`

##### Summary

A delegate called to execute a Receive Task

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessScriptTask'></a>
### ProcessScriptTask `property`

##### Summary

A delegate called to execute a Script Task.  This is called after any internal script extension elements have been processed.

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessSendTask'></a>
### ProcessSendTask `property`

##### Summary

A delegate called to exeucte a Send Task.

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessServiceTask'></a>
### ProcessServiceTask `property`

##### Summary

A delegate called to execute a Service Task

<a name='P-BPMNEngine-DelegateContainers-ProcessTasks-ProcessTask'></a>
### ProcessTask `property`

##### Summary

A delegate called to execute a Task

<a name='T-BPMNEngine-SFile'></a>
## SFile `type`

##### Namespace

BPMNEngine

##### Summary

This structure is used to house a File associated within a process instance.  It is used to both store, encode, decode and retreive File variables inside the process state.

<a name='P-BPMNEngine-SFile-Content'></a>
### Content `property`

##### Summary

The binary content of the File.

<a name='P-BPMNEngine-SFile-ContentType'></a>
### ContentType `property`

##### Summary

The content type tag for the File.  e.g. text/html

<a name='P-BPMNEngine-SFile-Extension'></a>
### Extension `property`

##### Summary

The extension of the File.

<a name='P-BPMNEngine-SFile-Name'></a>
### Name `property`

##### Summary

The name of the File.

<a name='M-BPMNEngine-SFile-Equals-System-Object-'></a>
### Equals(obj) `method`

##### Summary

Compares the object to this

##### Returns

true if obj is an sFile and is equal

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| obj | [System.Object](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Object 'System.Object') | the object to compare |

<a name='M-BPMNEngine-SFile-op_Equality-BPMNEngine-SFile,BPMNEngine-SFile-'></a>
### op_Equality(left,right) `method`

##### Summary

Compares left and right files

##### Returns

true if are equal

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| left | [BPMNEngine.SFile](#T-BPMNEngine-SFile 'BPMNEngine.SFile') | left file for comparison |
| right | [BPMNEngine.SFile](#T-BPMNEngine-SFile 'BPMNEngine.SFile') | right file for comparison |

<a name='M-BPMNEngine-SFile-op_Inequality-BPMNEngine-SFile,BPMNEngine-SFile-'></a>
### op_Inequality(left,right) `method`

##### Summary

Compares left and right files

##### Returns

true if are not equal

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| left | [BPMNEngine.SFile](#T-BPMNEngine-SFile 'BPMNEngine.SFile') | left file for comparison |
| right | [BPMNEngine.SFile](#T-BPMNEngine-SFile 'BPMNEngine.SFile') | right file for comparison |

<a name='T-BPMNEngine-SProcessRuntimeConstant'></a>
## SProcessRuntimeConstant `type`

##### Namespace

BPMNEngine

##### Summary

This structure is used to specify a Process Runtime Constant.  These Constants are used as a Dynamic Constant, so a read only variable within the process that can be unique to the instance running, only a constant to that specific process instance.

<a name='P-BPMNEngine-SProcessRuntimeConstant-Name'></a>
### Name `property`

##### Summary

The Name of the variable.

<a name='P-BPMNEngine-SProcessRuntimeConstant-Value'></a>
### Value `property`

##### Summary

The Value of the variable.

<a name='T-BPMNEngine-StartManualTask'></a>
## StartManualTask `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to start a Manual Task

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| task | [T:BPMNEngine.StartManualTask](#T-T-BPMNEngine-StartManualTask 'T:BPMNEngine.StartManualTask') | The Task being started |

##### Example

XML:
<bpmn:manualTask id="ManualTask_0ikjhwl">
 <bpmn:extensionElements>
   <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
 </bpmn:extensionElements>
 <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
</bpmn:startEvent>

```
public void _ProcessManualTask(IManualTask task)
    if (task.ExtensionElement != null){
        foreach (XmlNode xn in task.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="Question"){
                    Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
                    task.Variables[xn.Attributes["answer_property"].Value] = Console.ReadLine();
                }
            }
        }
    }
    task.MarkComplete();
}
```

##### Remarks

Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.

<a name='T-BPMNEngine-StartUserTask'></a>
## StartUserTask `type`

##### Namespace

BPMNEngine

##### Summary

This delegate is implemented to start a User Task

##### Parameters

| Name | Type | Description |
| ---- | ---- | ----------- |
| task | [T:BPMNEngine.StartUserTask](#T-T-BPMNEngine-StartUserTask 'T:BPMNEngine.StartUserTask') | The Task being started |

##### Example

XML:
<bpmn:userTask id="UserTask_0ikjhwl">
 <bpmn:extensionElements>
   <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
 </bpmn:extensionElements>
 <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
</bpmn:startEvent>

```
public void _ProcessUserTask(IUserTask task)
    if (task.ExtensionElement != null){
        foreach (XmlNode xn in task.ExtensionElement.SubNodes){
            if (xn.NodeType == XmlNodeType.Element)
            {
                if (xn.Name=="Question"){
                    Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
                    task.Variables[xn.Attributes["answer_property"].Value] = Console.ReadLine();
                    Console.WriteLine("Who Are You?");
                    task.UserID = Console.ReadLine();
                }
            }
        }
    }
    task.MarkComplete();
}
```

##### Remarks

Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.

<a name='T-BPMNEngine-StepStatuses'></a>
## StepStatuses `type`

##### Namespace

BPMNEngine

##### Summary

Enumeration of the statuses used for each step of a process

<a name='F-BPMNEngine-StepStatuses-Aborted'></a>
### Aborted `constants`

##### Summary

Aborted after being started or Suspended or Waiting on Start

<a name='F-BPMNEngine-StepStatuses-Failed'></a>
### Failed `constants`

##### Summary

Failed during execution

<a name='F-BPMNEngine-StepStatuses-NotRun'></a>
### NotRun `constants`

##### Summary

Step not run (typically used for drawing only and not stored in the state)

<a name='F-BPMNEngine-StepStatuses-Started'></a>
### Started `constants`

##### Summary

Started the satep

<a name='F-BPMNEngine-StepStatuses-Succeeded'></a>
### Succeeded `constants`

##### Summary

Completed successfully

<a name='F-BPMNEngine-StepStatuses-Suspended'></a>
### Suspended `constants`

##### Summary

Suspended until a timeout has passed

<a name='F-BPMNEngine-StepStatuses-Waiting'></a>
### Waiting `constants`

##### Summary

Waiting for input to complete execution

<a name='F-BPMNEngine-StepStatuses-WaitingStart'></a>
### WaitingStart `constants`

##### Summary

Delayed start until a timeout has passed

<a name='T-BPMNEngine-DelegateContainers-StepValidations'></a>
## StepValidations `type`

##### Namespace

BPMNEngine.DelegateContainers

##### Summary

This class is used to house all the validation delegates for a business process. 
This can be defined at either the BusinessProcess constructor level for defining it 
against all instances or at the BeginProcess level to defining it against a 
specific instance

<a name='P-BPMNEngine-DelegateContainers-StepValidations-IsEventStartValid'></a>
### IsEventStartValid `property`

##### Summary

A delegate called to validate if an event can start

```
<![CDATA[
            XML:
            <bpmn:startEvent id="StartEvent_0ikjhwl">
             <bpmn:extensionElements>
               <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
             </bpmn:extensionElements>
             <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
            </bpmn:startEvent>
            ]]>
```

<a name='P-BPMNEngine-DelegateContainers-StepValidations-IsFlowValid'></a>
### IsFlowValid `property`

##### Summary

A delegate called to validate if a flow is valid to run

```
<![CDATA[
            XML:
            <bpmn:outgoing>SequenceFlow_1jma3bu
             <bpmn:extensionElements>
               <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
             </bpmn:extensionElements>
            </bpmn:outgoing>
            ]]>
```

<a name='P-BPMNEngine-DelegateContainers-StepValidations-IsProcessStartValid'></a>
### IsProcessStartValid `property`

##### Summary

A delegate called to validate if a process is valid to start

```
<![CDATA[
            XML:
            <bpmn:process id="Process_1" isExecutable="false">
             ...
             <bpmn:extensionElements>
               <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
             </bpmn:extensionElements>
            </bpmn:process>
            ]]>
```
