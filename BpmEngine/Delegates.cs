using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace BpmEngine
{
    #region Ons
    /// <summary>
    /// This delegate is implemented to get triggered when a process element has been started, completed or errored.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="element">The process element that is starting, has completed or has errored.</param>
    /// <param name="variables">The process variables being provided to the event when it started,completed or errored.</param>
    /// <example>
    /// <code>
    ///     public void _OnElementStarted(IStepElement element,IReadonlyVariables variables){
    ///         Console.WriteLine("Element {0} inside process {1} has been started with the following variables:",element.id,element.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </code>
    /// </example>
    public delegate void OnElementEvent(IStepElement element, IReadonlyVariables variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process element has been aborted.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="element">The process element that is being aborted.</param>
    /// <param name="source">The process element that is causing the abort.</param>
    /// <param name="variables">The process variables being provided to the event it is being aborted.</param>
    /// <example>
    /// <code>
    ///     public void _OnElementAborted(IElement element,IElement source,IReadonlyVariables variables){
    ///         Console.WriteLine("Element {0} inside process {1} has been aborted by {2} with the following variables:",element.id,element.Process.id,source.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </code>
    /// </example>
    public delegate void OnElementAborted(IElement element,IElement source, IReadonlyVariables variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a Process has been started or completed.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="process">The Process being started or completed</param>
    /// <param name="variables">The process variables at the the time of the Process Start or Completion</param>
    /// <example>
    /// <code>
    ///     public void _ProcessStarted(IElement process,IReadonlyVariables variables){
    ///         Console.WriteLine("Process {0} has been started with the following variables:",process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </code>
    /// </example>
    public delegate void OnProcessEvent(IElement process, IReadonlyVariables variables);

    /// <summary>
    /// This delegate is implemented to get triggered when a process flow has been completed.
    /// </summary>
    /// <remarks>
    /// As it is an event driven delegate, the process will continue on without waiting for the delegate to finish.
    /// </remarks>
    /// <param name="element">The process flow that has been completed.</param>
    /// <param name="variables">The process variables being provided to the event when it completed.</param>
    /// <example>
    /// <code>
    ///     public void _OnFlowCompleted(IElement element,IReadonlyVariables variables){
    ///         Console.WriteLine("Flow {0} inside process {1} has been started with the following variables:",element.id,element.Process.id);
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </code>
    /// </example>
    public delegate void OnFlowComplete(IFlowElement element, IReadonlyVariables variables);

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
    /// <code>
    ///     public void _ProcessError(IElement process,IElement sourceElement, IReadonlyVariables variables){
    ///         Console.WriteLine("Element {0} inside process {1} had the error {2} occur with the following variables:",new object[]{sourceElement.id,process.id,variables.Error.Message});
    ///         foreach (string key in variables.FullKeys){
    ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
    ///         }
    ///     }
    /// </code>
    /// </example>
    public delegate void OnProcessErrorEvent(IElement process,IElement sourceElement, IReadonlyVariables variables);

    /// <summary>
    /// This delegate is implemented to get triggered when the Process State changes.  The state may not be usable externally without understanding its structure, however, capturing these events allows for the storage of a process state externally to be brought back in on a process restart.
    /// </summary>
    /// <param name="stateDocument">The XML Document containing the Process State</param>
    /// <example>
    /// <code>
    ///     public void _StateChange(XmlDocument stateDocument){
    ///         Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
    ///     }
    /// </code>
    /// </example>
    public delegate void OnStateChange(IState currentState);
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
    /// <code>
    /// public bool _EventStartValid(IStepElement Event, IVariables variables){
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
    /// </code>
    /// </example>
    public delegate bool IsEventStartValid(IStepElement Event, IReadonlyVariables variables);

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
    /// <code>
    /// public bool _ProcessStartValid(IElement process, IVariables variables){
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
    /// </code>
    /// </example>
    public delegate bool IsProcessStartValid(IElement process, IReadonlyVariables variables);

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
    /// <code>
    /// public bool _FlowValid(ISequenceFlow flow, IVariables variables){
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
    /// </code>
    /// </example>
    public delegate bool IsFlowValid(ISequenceFlow flow, IReadonlyVariables variables);
    #endregion

    #region Tasks

    /// <summary>
    /// This delegate is implemented to process a Process Task (This can be a Business Rule, Script, Send, Service and Task)
    /// </summary>
    /// <remarks>
    /// Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.
    /// </remarks>
    /// <param name="task">The Task being processed</param>
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
    /// <code>
    /// public void _ProcessBusinessRuleTask(ITask task)
    ///     if (task.ExtensionElement != null){
    ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="Analysis"){
    ///                     switch(xn.Attributes["formula"].Value){
    ///                         case "Average":
    ///                             decimal avgSum=0;
    ///                             decimal avgCount=0;
    ///                             foreach (Hashtable item in task.Variables["Items"]){
    ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
    ///                                     avgSum+=(decimal)item[xn.Attributes["inputs"].Value];
    ///                                     avgCount++;
    ///                                 }
    ///                             }
    ///                             task.Variables[xn.Attriubtes["outputVariable"].Value] = avgSum/avgCount;
    ///                             break;
    ///                         case "Sum":
    ///                             decimal sum=0;
    ///                             foreach (Hashtable item in task.Variables["Items"]){
    ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
    ///                                     sum+=(decimal)item[xn.Attributes["inputs"].Value];
    ///                                 }
    ///                             }
    ///                             task.Variables[xn.Attriubtes["outputVariable"].Value] = sum;
    ///                             break;
    ///                     }
    ///                 }
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public delegate void ProcessTask(ITask task);

    /// <summary>
    /// This delegate is implemented to start a Manual Task 
    /// </summary>
    /// <remarks>
    /// Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.
    /// </remarks>
    /// <param name="task">The Task being started</param>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:manualTask id="ManualTask_0ikjhwl">
    ///  <bpmn:extensionElements>
    ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
    ///  </bpmn:extensionElements>
    ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
    /// </bpmn:startEvent>
    /// ]]>
    /// <code>
    /// public void _ProcessManualTask(IManualTask task)
    ///     if (task.ExtensionElement != null){
    ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="Question"){
    ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
    ///                     task.Variables[xn.Attributes["answer_property"].Value] = Console.ReadLine();
    ///                 }
    ///             }
    ///         }
    ///     }
    ///     task.MarkComplete();
    /// }
    /// </code>
    /// </example>
    public delegate void StartManualTask(IManualTask task);
    /// <summary>
    /// This delegate is implemented to start a User Task 
    /// </summary>
    /// <remarks>
    /// Use of the bpmn:extension element to add additional components to the Task is recommended in order to implement your own piece of functionality used in handling of a Task.
    /// </remarks>
    /// <param name="task">The Task being started</param>
    /// <example>
    /// <![CDATA[
    /// XML:
    /// <bpmn:userTask id="UserTask_0ikjhwl">
    ///  <bpmn:extensionElements>
    ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
    ///  </bpmn:extensionElements>
    ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
    /// </bpmn:startEvent>
    /// ]]>
    /// <code>
    /// public void _ProcessUserTask(IUserTask task)
    ///     if (task.ExtensionElement != null){
    ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
    ///             if (xn.NodeType == XmlNodeType.Element)
    ///             {
    ///                 if (xn.Name=="Question"){
    ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
    ///                     task.Variables[xn.Attributes["answer_property"].Value] = Console.ReadLine();
    ///                     Console.WriteLine("Who Are You?");
    ///                     task.UserID = Console.ReadLine();
    ///                 }
    ///             }
    ///         }
    ///     }
    ///     task.MarkComplete();
    /// }
    /// </code>
    /// </example>
    public delegate void StartUserTask(IUserTask task);
    #endregion

    #region internals
    internal delegate void ProcessStepComplete(string sourceID,string outgoingID);
    internal delegate void ProcessStepError(IElement step,Exception ex);
    #endregion

    #region Logging

    /// <summary>
    /// This delegate is implemented to be called when a Log Line Entry is made by a process.  This can be used to log items externally, to a file, database, or logging engine implemented outside of the library.
    /// </summary>
    /// <param name="callingElement">The Process Element Calling the Log Line (may be null)</param>
    /// <param name="assembly">The AssemblyName for the source of the line</param>
    /// <param name="fileName">The source file name for the log entry</param>
    /// <param name="lineNumber">The source line number for the log entry</param>
    /// <param name="level">The log level for the entry</param>
    /// <param name="timestamp">The timestamp of when the log entry occured</param>
    /// <param name="message">The log entry</param>
    public delegate void LogLine(IElement callingElement,AssemblyName assembly, string fileName, int lineNumber, LogLevels level, DateTime timestamp, string message);

    /// <summary>
    /// This delegate is implemented to be called when a Log Exception is made by a process.  This can be used to log exceptions externally, to a file, database, or logging engine implemented outside of the library.
    /// </summary>
    /// <param name="callingElement">The Process Element Calling the Log Exception (may be null)</param>
    /// <param name="assembly">The AssemblyName for the source of the exception</param>
    /// <param name="fileName">The source file name for the exception</param>
    /// <param name="lineNumber">The source line number for the exception</param>
    /// <param name="timestamp">The timestamp of when the exception occured</param>
    /// <param name="exception">The exception that occured</param>
    public delegate void LogException(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception);
    #endregion
}
