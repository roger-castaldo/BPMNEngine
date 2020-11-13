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
    /// This delegate is implemented to get triggered when an Event is started inside process.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="Event">The process Event that is starting.</param>
    /// <param name="variables">The process variables being provided to the event when it start.</param>
    /// <example>
    ///     public void _EventStarted(IStepElement Event,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnEventStarted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when an Event is completed inside a process.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="Event">The process Event that has completed</param>
    /// <param name="variables">The process variables being provided after the event has completed</param>
    /// <example>
    ///     public void _EventCompleted(IStepElement Event,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnEventCompleted(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when an Event has an Error
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.
    /// </remarks>
    /// <param name="Event">The process Event that is having an error</param>
    /// <param name="variables">The process variables at the time the error occured inside the Event</param>
    /// <example>
    ///     public void _EventError(IStepElement Event,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnEventError(IStepElement Event, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task is started
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="task">The process Task being started</param>
    /// <param name="variables">The process variables at the time of the Task Start</param>
    /// <example>
    ///     public void _TaskStarted(IStepElement tasl,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Task {0} inside process {1} has been started with the following variables:",task.id,task.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnTaskStarted(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task is completed
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="task">The process Task that completed</param>
    /// <param name="variables">The process variables at the time of the Task Complete</param>
    /// <example>
    ///     public void _TaskCompleted(IStepElement task,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Task {0} inside process {1} has completed with the following variables:",task.id,task.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnTaskCompleted(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Task has an Error
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.
    /// </remarks>
    /// <param name="task">The process Task having an error</param>
    /// <param name="variables">The process variables at the time of the Task Error</param>
    /// <example>
    ///     public void _TaskError(IStepElement task,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Task {0} inside process {1} had the error {2} occur with the following variables:",new object[]{task.id,Event.task.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnTaskError(IStepElement task, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process is started
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="process">The Process being started</param>
    /// <param name="variables">The process variables at the the time of the Process Start</param>
    /// <example>
    ///     public void _ProcessStarted(IElement process,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Process {0} has been started with the following variables:",process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnProcessStarted(IElement process, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process is completed
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="process">The Process that completed</param>
    /// <param name="variables">The process variables at the time of the Process Complete</param>
    /// <example>
    ///     public void _ProcessCompleted(IElement process,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Process {0} has completed with the following variables:",process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnProcessCompleted(IElement process, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process has an Error
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.
    /// </remarks>
    /// <param name="process">The process that the error is contained in</param>
    /// <param name="sourceElement">The Process Element that is the source of the error</param>
    /// <param name="variables">The process variables at the time of the Error</param>
    /// <example>
    ///     public void _ProcessError(IElement process,IElement sourceElement, ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Element {0} inside process {1} had the error {2} occur with the following variables:",new object[]{sourceElement.id,process.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnProcessError(IElement process,IElement sourceElement, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Sequence Flow completes
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="flow">The process Sequence Flow completed</param>
    /// <param name="variables">The process varialbes at the time of the Sequence Flow Complete</param>
    /// <example>
    ///     public void _SequenceFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Sequence Flow {0} has been completed with the following variables:",flow.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnSequenceFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Message Flow completed
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="flow">The process Message Flow that completed</param>
    /// <param name="variables">The process variables at the time of the Message Flow Complete</param>
    /// <example>
    ///     public void _MessageFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Message Flow {0} has been completed with the following variables:",flow.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnMessageFlowCompleted(IElement flow, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway is starting
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="gateway">The process Gateway that was started</param>
    /// <param name="variables">The process variables at the time of the Gateway Start</param>
    /// <example>
    ///     public void _GatewayStarted(IStepElement gateway,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Gateway {0} inside process {1} has been started with the following variables:",gateway.id,gateway.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnGatewayStarted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway is completed
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="gateway">The process Gateway that completed</param>
    /// <param name="variables">The process variables at the time of the Gateway Completing</param>
    /// <example>
    ///     public void _GatewayCompleted(IStepElement gateway,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Gateway {0} inside process {1} has completed with the following variables:",gateway.id,gateway.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnGatewayCompleted(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process Gateway has an Error
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.
    /// </remarks>
    /// <param name="gateway">The process Gateway that had an error</param>
    /// <param name="variables">The process variables at the time of the Gateway Error</param>
    /// <example>
    ///     public void _GatewayError(IStepElement gateway,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Gateway {0} inside process {1} had the error {2} occur with the following variables:",new object[]{gateway.id,gateway.Process.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnGatewayError(IStepElement gateway, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess is starting
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="SubProcess">The SubProcess that started</param>
    /// <param name="variables">The process variables at the time of the SubProcess start</param>
    /// <example>
    ///     public void _SubProcessStarted(IStepElement SubProcess,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Sub Process {0} inside process {1} has been started with the following variables:",SubProcess.id,SubProcess.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnSubProcessStarted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess is completed
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="SubProcess">The SubProcess that completed</param>
    /// <param name="variables">The process variables at the time of the SubProcess completing</param>
    /// /// <example>
    ///     public void _SubProcessCompleted(IStepElement SubProcess,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Sub Process {0} inside process {1} has completed with the following variables:",SubProcess.id,SubProcess.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnSubProcessCompleted(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a SubProcess has an error
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.  As well as the variables container will have Error set to the Exception that occured.
    /// </remarks>
    /// <param name="SubProcess">The SubProcess that had the error</param>
    /// <param name="variables">The process variables at the time of the SubProcess having an error</param>
    /// <example>
    ///     public void _SubProcessError(IStepElement SubProcess,ReadOnlyProcessVariablesContainer variables){
    ///         Console.WriteLine("Sub Process {0} inside process {1} had the error {2} occur with the following variables:",new object[]{SubProcess.id,SubProcess.Process.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </example>
    public delegate void OnSubProcessError(IStepElement SubProcess, ReadOnlyProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when the Process State changes.  The state may not be usable externally without understanding its structure, however, capturing these events allows for the storage of a process state externally to be brought back in on a process restart.
    /// </summary>
    /// <param name="stateDocument">The XML Document containing the Process State</param>
    /// <example>
    ///     public void _StateChange(XmlDocument stateDocument){
    ///         Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
    ///     }
    /// </example>
    public delegate void OnStateChange(XmlDocument stateDocument);
    internal delegate void processStateChanged();
    #endregion

    #region Conditions

    /// <summary>
    /// This delegate is implemented to get triggered when determining if an Event Start is valid (i.e. can this event start)
    /// </summary>
    /// <remarks>
    /// This delegate is useful when adding additional elements through the extension element that are custom to your code.  It will be called with the given starting element that can be checked against additional components to decide if the start event is valid for a process.
    /// If valid, return true to initiate the containing process.
    /// </remarks>
    /// <param name="Event">The Start Event that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:startEvent id="StartEvent_0ikjhwl">
    ///  <bpmn:extensionElements>
    ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
    ///  </bpmn:extensionElements>
    ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
    /// </bpmn:startEvent>
    /// ]]>
    /// 
    /// public bool _EventStartValid(IStepElement Event, ProcessVariablesContainer variables){
    ///     if (Event.ExtensionElement != null){
    ///         foreach (XmlNode xn in Event.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="DateRange"){
    ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
    ///                 }
    ///             }
    ///         }
    ///     }
    ///     return true;
    /// }
    /// 
    /// </example>
    public delegate bool IsEventStartValid(IStepElement Event, ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when determining if a Process is valid to Start
    /// </summary>
    /// <remarks>
    /// This delegate is useful when adding additional elements through the extension element that are custom to your code.It will be called with the given process element that can be checked against additional components to decide if the start is valid for a process.
    /// If valid, return true to allow the system to continue to locate a valid start event within that process.
    /// </remarks>
    /// <param name="process">The Process that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:process id="Process_1" isExecutable="false">
    ///  ...
    ///  <bpmn:extensionElements>
    ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
    ///  </bpmn:extensionElements>
    /// </bpmn:process>
    /// ]]>
    /// 
    /// public bool _ProcessStartValid(IElement process, ProcessVariablesContainer variables){
    ///     if (process.ExtensionElement != null){
    ///         foreach (XmlNode xn in process.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="DateRange"){
    ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
    ///                 }
    ///             }
    ///         }
    ///     }
    ///     return true;
    /// }
    /// 
    /// </example>
    public delegate bool IsProcessStartValid(IElement process, ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to get triggered when determining if a Flow is a valid path
    /// </summary>
    /// <remarks>
    /// This delegate is useful when adding additional elements through the extension element that are custom to your code.It will be called with the given flow element that can be checked against additional components to decide if the flow is a valid next step in the process.
    /// If valid, return true to allow the system to continue along the supplied flow.
    /// </remarks>
    /// <param name="flow">The process Flow that is being checked</param>
    /// <param name="variables">The process variables at the time of the check</param>
    /// <returns></returns>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:outgoing>SequenceFlow_1jma3bu
    ///  <bpmn:extensionElements>
    ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
    ///  </bpmn:extensionElements>
    /// </bpmn:outgoing>
    /// ]]>
    /// 
    /// public bool _FlowValid(IElement flow, ProcessVariablesContainer variables){
    ///     if (flow.ExtensionElement != null){
    ///         foreach (XmlNode xn in flow.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="DateRange"){
    ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
    ///                 }
    ///             }
    ///         }
    ///     }
    ///     return true;
    /// }
    /// 
    /// </example>
    public delegate bool IsFlowValid(IElement flow, ProcessVariablesContainer variables);
    #endregion

    #region Tasks

    /// <summary>
    /// This delegate is implemented to process a Business Rule Task
    /// </summary>
    /// <remarks>
    /// Use of the bpmn:extension element to add additional components to the Business Rule Task is recommended in order to implement your own piece of functionality used in handling of a Business Rule Task.
    /// </remarks>
    /// <param name="task">The Business Rule Task being processed</param>
    /// <param name="variables">The process variables</param>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:businessRuleTask id="BusinessRule_0ikjhwl">
    ///  <bpmn:extensionElements>
    ///    <Analysis outputVariable="averageCost" inputs="Cost" formula="Average"/>
    ///    <Analysis outputVariable="totalQuantity" inputs="Quantity" formula="Sum"/>
    ///  </bpmn:extensionElements>
    ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
    /// </bpmn:startEvent>
    /// ]]>
    /// 
    /// public void _ProcessBusinessRuleTask(IStepElement task, ref ProcessVariablesContainer variables)
    ///     if (task.ExtensionElement != null){
    ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="Analysis"){
    ///                     switch(xn.Attributes["formula"].Value){
    ///                         case "Average":
    ///                             decimal avgSum=0;
    ///                             decimal avgCount=0;
    ///                             foreach (Hashtable item in variables["Items"]){
    ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
    ///                                     avgSum+=(decimal)item[xn.Attributes["inputs"].Value];
    ///                                     avgCount++;
    ///                                 }
    ///                             }
    ///                             variables[xn.Attriubtes["outputVariable"].Value] = avgSum/avgCount;
    ///                             break;
    ///                         case "Sum":
    ///                             decimal sum=0;
    ///                             foreach (Hashtable item in variables["Items"]){
    ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
    ///                                     sum+=(decimal)item[xn.Attributes["inputs"].Value];
    ///                                 }
    ///                             }
    ///                             variables[xn.Attriubtes["outputVariable"].Value] = sum;
    ///                             break;
    ///                     }
    ///                 }
    ///             }
    ///         }
    ///     }
    /// }
    /// 
    /// </example>
    public delegate void ProcessBusinessRuleTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is called when a Manual Task has been completed.  This is supplied by the delegate call BeginManualTask.  Alternatively, to complete a manual task, you can also call BusinessProcess.CompleteManualTask.  Either option will complete the manual task with the supplied variables
    /// and allow the process to continue, however using the delegate means you must maintain the delegate reference for completing the task.
    /// </summary>
    /// <param name="taskID">The ID of the manual task being completed</param>
    /// <param name="newVariables">The process variables to supply back to the process</param>
    public delegate void CompleteManualTask(string taskID, ProcessVariablesContainer newVariables);

    /// <summary>
    /// This delegate is called when a Manual Task has been errored.  This is supplied by the delegate call BeginManualTask.  Alternatively, to complete a manual task, you can also call BusinessProcess.ErrorManualTask.  Either option will generate an error on the manual task with the supplied Exception
    /// and allow the process to perform whatever functionality has been setup for an error, however using the delegate means you must maintain the delegate reference for erroring the task.
    /// </summary>
    /// <param name="taskID">The ID of the manual task that had an error</param>
    /// <param name="ex">The exception thrown</param>
    public delegate void ErrorManualTask(string taskID, Exception ex);

    /// <summary>
    /// This delegate is implemented to be called when a Manual Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.  The idea for this delegate is to allow a manual task to be completed outside the system, then using the delegates,
    /// indicate its completion or indicate an error occured.  The process itself will suspend this path until the task has been completed.  As this type of task is defined to require manual intervention of a user, a sample code is not included because this type of task would typically mark 
    /// something somewhere to advise a user of the task and then use the BusinessProcess.CompleteManualTask or BusinessProcess.ErrorManualTask call back in order to indicate what has happened to the task within the process.
    /// </summary>
    /// <param name="task">The Manual Task to execute</param>
    /// <param name="variables">The process variables for the task</param>
    /// <param name="completeCallback">The delegate to call when the manual task is completed</param>
    /// <param name="errorCallBack">The delegate to call when the manual task has an error</param>
    public delegate void BeginManualTask(IStepElement task, ProcessVariablesContainer variables, CompleteManualTask completeCallback, ErrorManualTask errorCallBack);

    /// <summary>
    /// This delegate is implemented to be called when a Process Recieve Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.
    /// </summary>
    /// <param name="task">The Process Recieve Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessRecieveTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Script Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.  This delegate will be called after any internal script extension elements have been processed, those 
    /// being A cSharpScript, Javascript or VBScript item.
    /// </summary>
    /// <param name="task">The Process Script Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessScriptTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Send Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.
    /// </summary>
    /// <param name="task">The Process Send Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessSendTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Service Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.
    /// </summary>
    /// <param name="task">The Process Service Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessServiceTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is implemented to be called when a Process Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.
    /// </summary>
    /// <param name="task">The Process Task to execute</param>
    /// <param name="variables">The process variables for the Task</param>
    public delegate void ProcessTask(IStepElement task, ref ProcessVariablesContainer variables);

    /// <summary>
    /// This delegate is called when a User Task has been completed.  This is supplied by the delegate call BeginUserTask.  Alternatively, to complete a manual task, you can also call BusinessProcess.CompleteUserTask.  Either option will complete the manual task with the supplied variables
    /// and allow the process to continue, however using the delegate means you must maintain the delegate reference for completing the task. 
    /// </summary>
    /// <param name="taskID">The ID of the task completed</param>
    /// <param name="newVariables">The process variables to submit into the process</param>
    /// <param name="completedByID">The completed by id (optional, use null when not needed, otherwise used to indicate a user id for the completion in the state document)</param>
    public delegate void CompleteUserTask(string taskID, ProcessVariablesContainer newVariables,string completedByID);

    /// <summary>
    /// This delegate is called when a User Task has an error.  This is supplied by the delegate call BeginUserTask.  Alternatively, to complete a manual task, you can also call BusinessProcess.ErrorUserTask.  Either option will generate an error on the manual task with the supplied Exception
    /// and allow the process to perform whatever functionality has been setup for an error, however using the delegate means you must maintain the delegate reference for erroring the task.
    /// </summary>
    /// <param name="taskID">The ID of the task errored</param>
    /// <param name="ex">The error exception</param>
    public delegate void ErrorUserTask(string taskID, Exception ex);

    /// <summary>
    /// This delegate is implemented to be called when a Process User Task needs to be executed.  Here you can process additional Extension Elements that have been placed into the task.  The idea for this delegate is to allow a user task to be completed outside the system, then using the delegates,
    /// indicate its completion or indicate an error occured.  The process itself will suspend this path until the task has been completed.  As this type of task is defined to require intervention of a user, a sample code is not included because this type of task would typically mark 
    /// something somewhere to advise a user of the task and then use the BusinessProcess.CompleteUserTask or BusinessProcess.ErrorUserTask call back in order to indicate what has happened to the task within the process.
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
    /// This delegate is implemented to be called when a Log Line Entry is made by a process.  This can be used to log items externally, to a file, database, or logging engine implemented outside of the library.
    /// </summary>
    /// <param name="assembly">The AssemblyName for the source of the line</param>
    /// <param name="fileName">The source file name for the log entry</param>
    /// <param name="lineNumber">The source line number for the log entry</param>
    /// <param name="level">The log level for the entry</param>
    /// <param name="timestamp">The timestamp of when the log entry occured</param>
    /// <param name="message">The log entry</param>
    public delegate void LogLine(AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message);

    /// <summary>
    /// This delegate is implemented to be called when a Log Exception is made by a process.  This can be used to log exceptions externally, to a file, database, or logging engine implemented outside of the library.
    /// </summary>
    /// <param name="assembly">The AssemblyName for the source of the exception</param>
    /// <param name="fileName">The source file name for the exception</param>
    /// <param name="lineNumber">The source line number for the exception</param>
    /// <param name="timestamp">The timestamp of when the exception occured</param>
    /// <param name="exception">The exception that occured</param>
    public delegate void LogException(AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception);
    #endregion
}
