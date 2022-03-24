using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Drawing;
using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    /// <summary>
    /// This class is the primary class for the library.  It implements a Business Process by constructing the object using a BPMN 2.0 compliant definition.
    /// This is followed by assigning delegates for handling the specific process events and then starting the process.  A process can also be suspended and 
    /// the suspended state loaded and resumed.  It can also be cloned, including the current state and delegates in order to have more than once instance 
    /// of the given process executing.
    /// </summary>
    public sealed class BusinessProcess : IDisposable
    {
        private static readonly TimeSpan _ANIMATION_DELAY = new TimeSpan(0,0,1);
        private const int _DEFAULT_PADDING = 100;
        private const int _VARIABLE_NAME_WIDTH = 200;
        private const int _VARIABLE_VALUE_WIDTH = 300;
        private const int _VARIABLE_IMAGE_WIDTH = _VARIABLE_NAME_WIDTH+_VARIABLE_VALUE_WIDTH;

        private Guid _id;
        private List<object> _components;
        private Dictionary<string, IElement> _elements;
        private AHandlingEvent[] _eventHandlers = null;

        internal IElement GetElement(string id)
        {
            if (_elements.ContainsKey(id))
                return _elements[id];
            return null;
        }
        private IElement[] _Elements
        {
            get
            {
                List<IElement> ret = new List<IElement>();
                foreach (object obj in _components)
                {
                    if (new List<Type>(obj.GetType().GetInterfaces()).Contains(typeof(IElement)))
                        ret.Add((IElement)obj);
                }
                return ret.ToArray();
            }
        }

        private void _RecurAddChildren(IElement parent)
        {
            _elements.Add(parent.id, parent);
            if (parent is IParentElement)
            {
                foreach (IElement elem in ((IParentElement)parent).Children)
                    _RecurAddChildren(elem);
            }
        }

        private XmlDocument _doc;
        /// <summary>
        /// The XML Document that was supplied to the constructor containing the BPMN 2.0 definition
        /// </summary>
        public XmlDocument Document { get { return _doc; } }

        private sProcessRuntimeConstant[] _constants;
        /// <summary>
        /// This is used to access the values of the process runtime and definition constants
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable</returns>
        public object this[string name]
        {
            get
            {
                if (_constants != null)
                {
                    foreach (sProcessRuntimeConstant sprc in _constants)
                    {
                        if (sprc.Name == name)
                            return sprc.Value;
                    }
                }
                if (_Elements != null)
                {
                    foreach (IElement elem in _Elements)
                    {
                        if (elem is Definition)
                        {
                            Definition def = (Definition)elem;
                            if (def.ExtensionElement != null)
                            {
                                foreach (IElement e in ((ExtensionElements)def.ExtensionElement).Children)
                                {
                                    if (e is DefinitionVariable)
                                    {
                                        DefinitionVariable dv = (DefinitionVariable)e;
                                        if (dv.Name == name)
                                            return dv.Value;
                                    }else if (e is DefinitionFile)
                                    {
                                        DefinitionFile df = (DefinitionFile)e;
                                        if (string.Format("{0}.{1}", df.Name, df.Extension) == name || df.Name == name)
                                            return new sFile(df.Name, df.Extension, df.ContentType, df.Content);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                return null;
            }
        }

        
        internal string[] Keys
        {
            get
            {
                List<string> ret = new List<string>();
                if (_constants != null)
                {
                    foreach (sProcessRuntimeConstant sprc in _constants)
                    {
                        if (!ret.Contains(sprc.Name))
                            ret.Add(sprc.Name);
                    }
                }
                if (_Elements != null)
                {
                    foreach (IElement elem in _Elements)
                    {
                        if (elem is Definition)
                        {
                            Definition def = (Definition)elem;
                            if (def.ExtensionElement != null)
                            {
                                foreach (IElement e in ((ExtensionElements)def.ExtensionElement).Children)
                                {
                                    if (e is DefinitionVariable)
                                    {
                                        DefinitionVariable dv = (DefinitionVariable)e;
                                        if (!ret.Contains(dv.Name))
                                            ret.Add(dv.Name);
                                    }
                                    else if (e is DefinitionFile)
                                    {
                                        DefinitionFile df = (DefinitionFile)e;
                                        if (!ret.Contains(string.Format("{0}.{1}", df.Name, df.Extension)))
                                            ret.Add(string.Format("{0}.{1}", df.Name, df.Extension));
                                        if (!ret.Contains(df.Name))
                                            ret.Add(df.Name);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                return ret.ToArray();
            }
        }

        private DelegateContainer _delegates;

        internal ATask GetTask(string taskID)
        {
            IElement elem = GetElement(taskID);
            if (elem is ATask)
                return (ATask)elem;
            return null;
        }

        internal void HandleTaskEmission(ProcessInstance instance, ITask task, object data, EventSubTypes type)
        {
            AHandlingEvent[] events = _GetEventHandlers(type, data, (AFlowNode)GetElement(task.id), new ReadOnlyProcessVariablesContainer(task));
            foreach (AHandlingEvent ahe in events)
            {
                ProcessEvent(instance,task.id, ahe);
            }
        }

        private BusinessProcess() {
            _id = Utility.NextRandomGuid();
        }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, StateLogLevel, runtime constants and LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="stateLogLevel">The log level to use for logging data into the process state</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        /// <param name="logLine">The LogLine delegate called to append a log line entry from the process</param>
        /// <param name="logException">The LogException delegate called to append a logged exception from the process</param>
        /// <param name="onEventStarted">
        /// The OnEventStarted delegate called when an event starts
        /// <code>
        /// public void OnEventStarted(IStepElement Event, IReadonlyVariables variables);{
        ///     Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
        ///     foreach (string key in variables.FullKeys){
        ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///     }
        /// }
        /// </code>
        /// </param>
        /// <param name="onEventCompleted">
        /// The OnEventCompleted delegate called when an event completes
        /// <code>
        /// public void OnEventCompleted(IStepElement Event, IReadonlyVariables variables){
        ///     Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
        ///     foreach (string key in variables.FullKeys){
        ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///     }
        /// }
        /// </code>
        /// </param>
        /// <param name="onEventError">
        /// The OnEventError delegate called when an Event has an error
        /// <code>
        ///     public void OnEventError(IStepElement Event, IReadonlyVariables variables){
        ///         Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onTaskStarted">
        /// The OnTaskStarted delegate called when an Task starts
        /// <code>
        /// public void OnTaskStarted(IStepElement task, IReadonlyVariables variables){
        ///         Console.WriteLine("Task {0} inside process {1} has been started with the following variables:",task.id,task.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onTaskCompleted">
        /// The OnTaskCompleted delegate called when an Task completes
        /// <code>
        /// public void OnTaskCompleted(IStepElement task, IReadonlyVariables variables)
        ///         Console.WriteLine("Task {0} inside process {1} has completed with the following variables:",task.id,task.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onTaskError">
        /// The OnTaskError delegate called when an Task has an error
        /// <code>
        ///     public void OnTaskError(IStepElement task, IReadonlyVariables variables){
        ///         Console.WriteLine("Task {0} inside process {1} had the error {2} occur with the following variables:",new object[]{task.id,Event.task.id,variables.Error.Message});
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onProcessStarted">
        /// The OnProcessStarted delegate called when an Process starts
        /// <code>
        /// public void OnProcessStarted(IElement process, IReadonlyVariables variables){
        ///         Console.WriteLine("Process {0} has been started with the following variables:",process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onProcessCompleted">
        /// The OnProcessCompleted delegate called when an Process completes
        /// <code>
        /// public void OnProcessCompleted(IElement process, IReadonlyVariables variables){
        ///         Console.WriteLine("Process {0} has completed with the following variables:",process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onProcessError">
        /// The OnProcessError delegate called when an Process has an error
        /// <code>
        /// public void OnProcessError(IElement process, IElement sourceElement, IReadonlyVariables variables){
        ///         Console.WriteLine("Element {0} inside process {1} had the error {2} occur with the following variables:",new object[]{sourceElement.id,process.id,variables.Error.Message});
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onSequenceFlowCompleted">
        /// The OnSequenceFlowCompleted delegate called when a sequence flow completes
        /// <code>
        /// public void OnSequenceFlowCompleted(IElement flow, IReadonlyVariables variables){
        ///         Console.WriteLine("Sequence Flow {0} has been completed with the following variables:",flow.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onMessageFlowCompleted">
        /// The OnMessageFlowCompleted delegate called when a message flow completes
        /// <code>
        /// public void OnMessageFlowCompleted(IElement flow, IReadonlyVariables variables){
        ///         Console.WriteLine("Message Flow {0} has been completed with the following variables:",flow.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onGatewayStarted">
        /// The OnGatewayStarted delegate called when an Gateway starts
        /// <code>
        ///     public void OnGatewayStarted(IStepElement gateway, IReadonlyVariables variables){
        ///         Console.WriteLine("Gateway {0} inside process {1} has been started with the following variables:",gateway.id,gateway.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onGatewayCompleted">
        /// The OnGatewayCompleted delegate called when an Gateway completes
        /// <code>
        /// public void OnGatewayCompleted(IStepElement gateway, IReadonlyVariables variables){
        ///         Console.WriteLine("Gateway {0} inside process {1} has completed with the following variables:",gateway.id,gateway.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onGatewayError">
        /// The OnGatewayError delegate called when an Gateway has an error
        /// <code>
        /// public void OnGatewayError(IStepElement gateway, IReadonlyVariables variables){
        ///         Console.WriteLine("Gateway {0} inside process {1} had the error {2} occur with the following variables:",new object[]{gateway.id,gateway.Process.id,variables.Error.Message});
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onSubProcessStarted">
        /// The OnSubProcessStarted delegate called when an SubProcess starts
        /// <code>
        ///  public void OnSubProcessStarted(IStepElement SubProcess, IReadonlyVariables variables){
        ///         Console.WriteLine("Sub Process {0} inside process {1} has been started with the following variables:",SubProcess.id,SubProcess.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onSubProcessCompleted">
        /// The OnSubProcessCompleted delegate called when an SubProcess completes
        /// <code>
        /// public void OnSubProcessCompleted(IStepElement SubProcess, IReadonlyVariables variables){
        ///         Console.WriteLine("Sub Process {0} inside process {1} has completed with the following variables:",SubProcess.id,SubProcess.Process.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onSubProcessError">
        /// The OnSubProcessError delegate called when an SubProcess has an error
        /// <code>
        /// public void OnSubProcessError(IStepElement SubProcess, IReadonlyVariables variables){
        ///         Console.WriteLine("Sub Process {0} inside process {1} had the error {2} occur with the following variables:",new object[]{SubProcess.id,SubProcess.Process.id,variables.Error.Message});
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="onStateChange">
        /// The OnStateChange delegate called when the process state document has changed
        /// <code>
        /// public void OnStateChange(XmlDocument stateDocument){
        ///         Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
        ///     }
        /// </code>
        /// </param>
        /// <param name="onElementAborted">
        /// The OnElementAborted delegate called when an element is aborted within the process
        /// <code>
        /// public void OnStepAborted(IElement element, IElement source, IReadonlyVariables variables){
        ///         Console.WriteLine("Element {0} inside process {1} has been aborted by {2} with the following variables:",element.id,element.Process.id,source.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </param>
        /// <param name="isEventStartValid">
        /// The IsEventStartValid delegate called to validate if an event can start
        /// <code>
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
        /// public bool IsEventStartValid(IStepElement Event, IVariables variables){
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
        /// </param>
        /// <param name="isProcessStartValid">
        /// The IsProcessStartValid delegate called to validate if a process is valid to start
        /// <code>
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
        /// public bool IsProcessStartValid(IElement process, IVariables variables){
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
        /// </param>
        /// <param name="isFlowValid">
        /// The IsFlowValid delegate called to validate if a flow is valid to run
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:outgoing>SequenceFlow_1jma3bu
        ///  <bpmn:extensionElements>
        ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
        ///  </bpmn:extensionElements>
        /// </bpmn:outgoing>
        /// ]]>
        /// 
        /// public bool IsFlowValid(IElement flow, IVariables variables){
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
        /// </param>
        /// <param name="processBusinessRuleTask">
        /// The ProcessBusinessRuleTask delegate called to execute a Business Rule Task
        /// <code>
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
        /// public void ProcessBusinessRuleTask(ITask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Analysis"){
        ///                     switch(xn.Attributes["formula"].Value){
        ///                         case "Average":
        ///                             decimal avgSum=0;
        ///                             decimal avgCount=0;
        ///                             foreach (Hashtable item in task["Items"]){
        ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
        ///                                     avgSum+=(decimal)item[xn.Attributes["inputs"].Value];
        ///                                     avgCount++;
        ///                                 }
        ///                             }
        ///                             task[xn.Attriubtes["outputVariable"].Value] = avgSum/avgCount;
        ///                             break;
        ///                         case "Sum":
        ///                             decimal sum=0;
        ///                             foreach (Hashtable item in task["Items"]){
        ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
        ///                                     sum+=(decimal)item[xn.Attributes["inputs"].Value];
        ///                                 }
        ///                             }
        ///                             task[xn.Attriubtes["outputVariable"].Value] = sum;
        ///                             break;
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </param>
        /// <param name="beginManualTask">
        /// The BeginManualTask delegate called to start a Manual Task
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:manualTask id="ManualTask_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public void BeginManualTask(IManualTask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Question"){
        ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
        ///                     task[xn.Attributes["answer_property"].Value] = Console.ReadLine();
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     task.MarkComplete();
        /// }
        /// </code>
        /// </param>
        /// <param name="processRecieveTask">The ProcessRecieveTask delegate called to execute a Recieve Task</param>
        /// <param name="processScriptTask">The ProcessScriptTask delegate called to execute a Script Task.  This is called after any internal script extension elements have been processed.</param>
        /// <param name="processSendTask">The ProcessSendTask delegate called to exeucte a Send Task.</param>
        /// <param name="processServiceTask">The ProcessServiceTask delegate called to execute a Service Task</param>
        /// <param name="processTask">The ProcessTask delegate called to execute a Task</param>
        /// <param name="beginUserTask">
        /// The BeginUserTask delegate called to start a User Task
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:userTask id="UserTask_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public void BeginUserTask(IUserTask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Question"){
        ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
        ///                     task[xn.Attributes["answer_property"].Value] = Console.ReadLine();
        ///                     Console.WriteLine("Who Are You?");
        ///                     task.UserID = Console.ReadLine();
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     task.MarkComplete();
        /// }
        /// </code>
        /// </param>
        public BusinessProcess(XmlDocument doc, 
            sProcessRuntimeConstant[] constants = null,
            LogLine logLine = null,
            LogException logException = null,
            OnElementEvent onEventStarted = null,
            OnElementEvent onEventCompleted = null,
            OnElementEvent onEventError=null,
            OnElementEvent onTaskStarted=null,
            OnElementEvent onTaskCompleted=null,
            OnElementEvent onTaskError=null,
            OnProcessEvent onProcessStarted=null,
            OnProcessEvent onProcessCompleted=null,
            OnProcessErrorEvent onProcessError=null,
            OnFlowComplete onSequenceFlowCompleted=null,
            OnFlowComplete onMessageFlowCompleted=null,
            OnElementEvent onGatewayStarted=null,
            OnElementEvent onGatewayCompleted=null,
            OnElementEvent onGatewayError=null,
            OnElementEvent onSubProcessStarted=null,
            OnElementEvent onSubProcessCompleted=null,
            OnElementEvent onSubProcessError=null,
            OnStateChange onStateChange=null,
            OnElementAborted onElementAborted=null,
            IsEventStartValid isEventStartValid=null,
            IsProcessStartValid isProcessStartValid=null,
            IsFlowValid isFlowValid=null,
            ProcessTask processBusinessRuleTask=null,
            StartManualTask beginManualTask=null,
            ProcessTask processRecieveTask=null,
            ProcessTask processScriptTask=null,
            ProcessTask processSendTask=null,
            ProcessTask processServiceTask=null,
            ProcessTask processTask=null,
            StartUserTask beginUserTask=null
            )
        {
            _id = Utility.NextRandomGuid();
            _constants = constants;
            _delegates = new DelegateContainer(logLine, logException, onEventStarted, onEventCompleted, onEventError, onTaskStarted, onTaskCompleted, onTaskError, onProcessStarted, onProcessCompleted, onProcessError, onSequenceFlowCompleted, onMessageFlowCompleted,
                onGatewayStarted, onGatewayCompleted, onGatewayError, onSubProcessStarted, onSubProcessCompleted, onSubProcessError, onStateChange, onElementAborted, isEventStartValid, isProcessStartValid, isFlowValid, processBusinessRuleTask,
                beginManualTask, processRecieveTask, processScriptTask, processSendTask, processServiceTask, processTask, beginUserTask);


            List<Exception> exceptions = new List<Exception>();
            _doc = new XmlDocument();
            _doc.LoadXml(doc.OuterXml);
            BpmEngine.ElementTypeCache elementMapCache = new BpmEngine.ElementTypeCache();
            DateTime start = DateTime.Now;
            WriteLogLine((IElement)null,LogLevels.Info,new StackFrame(1,true),DateTime.Now,"Producing new Business Process from XML Document");
            _components = new List<object>();
            _elements = new Dictionary<string, Interfaces.IElement>();
            XmlPrefixMap map = new XmlPrefixMap(this);
            foreach (XmlNode n in doc.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (map.Load((XmlElement)n))
                        elementMapCache.MapIdeals(map);
                    IElement elem = Utility.ConstructElementType((XmlElement)n, ref map, ref elementMapCache, null);
                    if (elem != null)
                    {
                        if (elem is AParentElement)
                            ((AParentElement)elem).LoadChildren(ref map, ref elementMapCache);
                        ((AElement)elem).LoadExtensionElement(ref map, ref elementMapCache);
                        _components.Add(elem);
                    }
                    else
                        _components.Add(n);
                }
                else
                    _components.Add(n);
            }
            foreach (object obj in _components)
            {
                if (obj is IElement)
                    _RecurAddChildren((IElement)obj);
            }
            if (_Elements.Length== 0)
                exceptions.Add(new XmlException("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
            else
            {
                bool found = false;
                foreach (IElement elem in _Elements)
                {
                    if (elem is Definition)
                        found = true;
                }
                if (!found)
                    exceptions.Add(new XmlException("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
            }
            _InitDefinition();
            if (exceptions.Count == 0)
            {
                foreach (IElement elem in _Elements)
                    _ValidateElement((AElement)elem, ref exceptions);
            }
            if (exceptions.Count != 0)
            {
                Exception ex = new InvalidProcessDefinitionException(exceptions);
                WriteLogException((IElement)null,new StackFrame(1, true), DateTime.Now, ex);
                throw ex;
            }
            List<AHandlingEvent> tmp = new List<AHandlingEvent>();
            foreach (string str in _elements.Keys)
            {
                if (_elements[str] is AHandlingEvent)
                    tmp.Add((AHandlingEvent)_elements[str]);
            }
            _eventHandlers = tmp.ToArray();
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Time to load Process Document {0}ms",DateTime.Now.Subtract(start).TotalMilliseconds));
        }

        private void _InitDefinition()
        {
            for (int x = 0; x < _components.Count; x++)
            {
                if (_components[x] is Definition)
                {
                    Definition def = (Definition)_components[x];
                    _components.RemoveAt(x);
                    def.OwningProcess = this;
                    _components.Insert(x, def);
                }
            }
        }

        private void _ValidateElement(AElement elem,ref List<Exception> exceptions)
        {
            WriteLogLine(elem,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Validating element {0}", new object[] { elem.id }));
            foreach (RequiredAttribute ra in Utility.GetCustomAttributesForClass(elem.GetType(),typeof(RequiredAttribute)))
            {
               if (elem[ra.Name]==null)
                    exceptions.Add(new MissingAttributeException(elem.Definition,elem.Element,ra));
            }
            foreach (AttributeRegex ar in Utility.GetCustomAttributesForClass(elem.GetType(), typeof(AttributeRegex)))
            {
                if (!ar.IsValid(elem))
                    exceptions.Add(new InvalidAttributeValueException(elem.Definition,elem.Element, ar));
            }
            string[] err;
            if (!elem.IsValid(out err))
                exceptions.Add(new InvalidElementException(elem.Definition,elem.Element, err));
            if (elem.ExtensionElement != null)
                _ValidateElement((ExtensionElements)elem.ExtensionElement, ref exceptions);
            if (elem is AParentElement)
            {
                foreach (AElement e in ((AParentElement)elem).Children)
                    _ValidateElement(e,ref exceptions);
            }
        }

        /// <summary>
        /// Called to load a Process Instance from a stored State Document
        /// </summary>
        /// <param name="doc">The process state document</param>
        /// <param name="autoResume">set true if the process was suspended and needs to resume once loaded</param>
        /// <param name="logLine">Used to replace existing process delegate specific for this instance</param>
        /// <param name="logException">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSequenceFlowCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onMessageFlowCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onStateChange">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onElementAborted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isEventStartValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isProcessStartValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isFlowValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processBusinessRuleTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="beginManualTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processRecieveTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processScriptTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processSendTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processServiceTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="beginUserTask">Used to replace existing process delegate specific for this instance</param>
        /// <returns>an instance of IProcessInstance if successful or null it failed</returns>
        public IProcessInstance LoadState(XmlDocument doc,
            bool autoResume=false,
            LogLine logLine = null,
            LogException logException = null,
            OnElementEvent onEventStarted = null,
            OnElementEvent onEventCompleted = null,
            OnElementEvent onEventError = null,
            OnElementEvent onTaskStarted = null,
            OnElementEvent onTaskCompleted = null,
            OnElementEvent onTaskError = null,
            OnProcessEvent onProcessStarted = null,
            OnProcessEvent onProcessCompleted = null,
            OnProcessErrorEvent onProcessError = null,
            OnFlowComplete onSequenceFlowCompleted = null,
            OnFlowComplete onMessageFlowCompleted = null,
            OnElementEvent onGatewayStarted = null,
            OnElementEvent onGatewayCompleted = null,
            OnElementEvent onGatewayError = null,
            OnElementEvent onSubProcessStarted = null,
            OnElementEvent onSubProcessCompleted = null,
            OnElementEvent onSubProcessError = null,
            OnStateChange onStateChange = null,
            OnElementAborted onElementAborted = null,
            IsEventStartValid isEventStartValid = null,
            IsProcessStartValid isProcessStartValid = null,
            IsFlowValid isFlowValid = null,
            ProcessTask processBusinessRuleTask = null,
            StartManualTask beginManualTask = null,
            ProcessTask processRecieveTask = null,
            ProcessTask processScriptTask = null,
            ProcessTask processSendTask = null,
            ProcessTask processServiceTask = null,
            ProcessTask processTask = null,
            StartUserTask beginUserTask = null,
            LogLevels stateLogLevel=LogLevels.None)
        {
            ProcessInstance ret = new ProcessInstance(this, _delegates.Merge(logLine, logException, onEventStarted, onEventCompleted, onEventError,
                onTaskStarted, onTaskCompleted, onTaskError, onProcessStarted, onProcessCompleted, onProcessError,
                onSequenceFlowCompleted, onMessageFlowCompleted, onGatewayStarted, onGatewayCompleted, onGatewayError,
                onSubProcessStarted, onSubProcessCompleted, onSubProcessError, onStateChange, onElementAborted, isEventStartValid,
                isProcessStartValid, isFlowValid, processBusinessRuleTask, beginManualTask, processRecieveTask, processScriptTask,
                processSendTask, processServiceTask, processTask, beginUserTask), stateLogLevel);
            if (ret.LoadState(doc, autoResume))
                return ret;
            return null;
        }

        /// <summary>
        /// Called to render a PNG image of the process
        /// </summary>
        /// <param name="outputVariables">Set true to include outputting variables into the image</param>
        /// <returns>A Bitmap containing a rendered image of the process</returns>
        public byte[] Diagram(ImageOuputTypes type)
        {
            return _Diagram(false,null).ToFile(type);
        }

        internal byte[] Diagram(bool outputVariables,ProcessState state, ImageOuputTypes type)
        {
            return _Diagram(outputVariables, state).ToFile(type);
        }

        private Image _Diagram(bool outputVariables, ProcessState state)
        {
            if (!Image.CanUse)
                throw new Exception("Unable to produce a Diagram or Animated Diagram as there is no valid Drawing Assembly Loaded, please add either System.Drawing.Common or SkiaSharp");
            if (state==null)
                state = new ProcessState(this, null, null,null);
            WriteLogLine((IElement)null, LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Diagram{0}", new object[] { (outputVariables ? " with variables" : " without variables") }));
            int width = 0;
            int height = 0;
            foreach (IElement elem in _Elements)
            {
                if (elem is Definition)
                {
                    foreach (Diagram d in ((Definition)elem).Diagrams)
                    {
                        Size s = d.Size;
                        width = Math.Max(width, s.Width + _DEFAULT_PADDING);
                        height += _DEFAULT_PADDING + s.Height;
                    }
                }
            }
            Image ret = new Image(width, height);
            ret.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            int padding = _DEFAULT_PADDING / 2;
            foreach (IElement elem in _Elements)
            {
                if (elem is Definition)
                {
                    foreach (Diagram d in ((Definition)elem).Diagrams)
                    {
                        ret.DrawImage(d.Render(state.Path, ((Definition)elem)), new Point(_DEFAULT_PADDING / 2, padding));
                        padding += d.Size.Height + _DEFAULT_PADDING;
                    }
                }
            }
            if (outputVariables)
                ret = _AppendVariables(ret,state);
            return ret;
        }

        private Image _ProduceVariablesImage(Image diagram,ProcessState state)
        {
            Size sz = diagram.MeasureString("Variables");
            int varHeight = (int)sz.Height + 2;
            string[] keys = state[null];
            foreach (string str in keys)
                varHeight += (int)diagram.MeasureString(str,null).Height + 2;
            Image ret = new Image(_VARIABLE_IMAGE_WIDTH, varHeight);
            ret.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, ret.Size.Width, ret.Size.Height));
            Pen p = new Pen(Color.Black, Constants.PEN_WIDTH);
            ret.DrawRectangle(p, new Rectangle(0, 0, ret.Size.Width, ret.Size.Height));
            ret.DrawLine(p, new Point(0, (int)sz.Height + 2), new Point(_VARIABLE_IMAGE_WIDTH, (int)sz.Height + 2));
            ret.DrawLine(p, new Point(_VARIABLE_NAME_WIDTH, (int)sz.Height + 2), new Point(_VARIABLE_NAME_WIDTH, ret.Size.Height));
            ret.DrawString("Variables",new SolidBrush(Color.Black),new Rectangle((ret.Size.Width - sz.Width) / 2, 2,sz.Width,sz.Height),true);
            float curY = sz.Height + 2;
            for (int x = 0; x < keys.Length; x++)
            {
                string label = keys[x];
                Size szLabel = ret.MeasureString(keys[x]);
                while (szLabel.Width > _VARIABLE_NAME_WIDTH)
                {
                    if (label.EndsWith("..."))
                        label = label.Substring(0, label.Length - 4) + "...";
                    else
                        label = label.Substring(0, label.Length - 1) + "...";
                    szLabel = ret.MeasureString(label);
                }
                string val = "";
                if (state[null, keys[x]] != null)
                {
                    if (state[null, keys[x]].GetType().IsArray)
                    {
                        val = "";
                        foreach (object o in (IEnumerable)state[null, keys[x]])
                            val += string.Format("{0},", o);
                        val = val.Substring(0, val.Length - 1);
                    }
                    else if (state[null, keys[x]] is Hashtable)
                    {
                        val = "{";
                        foreach (string key in ((Hashtable)state[null, keys[x]]).Keys)
                            val += string.Format("{{\"{0}\":\"{1}\"}},", key, ((Hashtable)state[null, keys[x]])[key]);
                        val = val.Substring(0, val.Length - 1) + "}";
                    }
                    else
                        val = state[null, keys[x]].ToString();
                }
                Size szValue = ret.MeasureString(val);
                if (szValue.Width > _VARIABLE_VALUE_WIDTH)
                {
                    if (val.EndsWith("..."))
                        val = val.Substring(0, val.Length - 4) + "...";
                    else
                        val = val.Substring(0, val.Length - 1) + "...";
                    szValue = ret.MeasureString(val);
                }
                ret.DrawString(label, Color.Black, new Point(2, curY));
                ret.DrawString(val, Color.Black, new Point(2 + _VARIABLE_NAME_WIDTH, curY));
                curY += (int)Math.Max(szLabel.Height, szValue.Height) + 2;
                ret.DrawLine(p, new Point(0, curY), new Point(_VARIABLE_IMAGE_WIDTH, curY));
            }
            ret.Flush();
            return ret;
        }

        private Image _AppendVariables(Image diagram,ProcessState state)
        {
            Image vmap = _ProduceVariablesImage(diagram,state);
            Image ret = new Image(diagram.Size.Width + _DEFAULT_PADDING + vmap.Size.Width, Math.Max(diagram.Size.Height, vmap.Size.Height + _DEFAULT_PADDING));
            ret.Clear(Color.White);
            ret.DrawImage(diagram, new Point(0, 0));
            ret.DrawImage(vmap, new Point(ret.Size.Width + _DEFAULT_PADDING, _DEFAULT_PADDING));
            ret.Flush();
            return ret;
        }

        internal byte[] Animate(bool outputVariables,ProcessState state)
        {
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Animation{0}", new object[] { (outputVariables ? " with variables" : " without variables") }));
            MemoryStream ms = new MemoryStream();
            using (Drawing.GifEncoder enc = new Drawing.GifEncoder(ms))
            {
                enc.FrameDelay = _ANIMATION_DELAY;
                state.Path.StartAnimation();
                Image bd = _Diagram(false,state);
                enc.AddFrame(new Drawing.GifEncoder.sFramePart[] { new Drawing.GifEncoder.sFramePart((outputVariables ? _AppendVariables(bd,state) : bd)) });
                while (state.Path.HasNext())
                {
                    string nxtStep = state.Path.MoveToNextStep();
                    if (nxtStep != null)
                    {
                        List<Drawing.GifEncoder.sFramePart> frames = new List<Drawing.GifEncoder.sFramePart>();
                        Rectangle rect;
                        int padding = _DEFAULT_PADDING / 2;
                        foreach (IElement elem in _Elements)
                        {
                            if (elem is Definition)
                            {
                                foreach (Diagram d in ((Definition)elem).Diagrams)
                                {
                                    if (d.RendersElement(nxtStep))
                                    {
                                        Image img = d.RenderElement(state.Path, (Definition)elem, nxtStep, out rect);
                                        if (rect!=null)
                                        {
                                            frames.Add(new Drawing.GifEncoder.sFramePart(img, (_DEFAULT_PADDING / 2)+(int)rect.X, padding+(int)rect.Y));
                                            //gp.DrawImage(d.UpdateState(_state.Path, ((Definition)elem), nxtStep), new Point(_DEFAULT_PADDING / 2, padding));
                                            break;
                                        }
                                    }
                                    padding += d.Size.Height + _DEFAULT_PADDING;
                                }
                            }
                        }
                        if (outputVariables)
                            frames.Add(new Drawing.GifEncoder.sFramePart(_ProduceVariablesImage(bd,state), bd.Size.Width + _DEFAULT_PADDING, _DEFAULT_PADDING));
                        enc.AddFrame(frames.ToArray());
                    }
                }
                state.Path.FinishAnimation();
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Called to start and instance of the defined BusinessProcess
        /// </summary>
        /// <param name="pars">The variables to start the process with</param>
        /// <param name="logLine">Used to replace existing process delegate specific for this instance</param>
        /// <param name="logException">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onEventError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onTaskError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onProcessError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSequenceFlowCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onMessageFlowCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onGatewayError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessStarted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessCompleted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onSubProcessError">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onStateChange">Used to replace existing process delegate specific for this instance</param>
        /// <param name="onElementAborted">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isEventStartValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isProcessStartValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="isFlowValid">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processBusinessRuleTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="beginManualTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processRecieveTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processScriptTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processSendTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processServiceTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="processTask">Used to replace existing process delegate specific for this instance</param>
        /// <param name="beginUserTask">Used to replace existing process delegate specific for this instance</param>
        /// <returns>a process instance if the process was successfully started</returns>
        public IProcessInstance BeginProcess(Dictionary<string,object> pars,
            LogLine logLine = null,
            LogException logException = null,
            OnElementEvent onEventStarted = null,
            OnElementEvent onEventCompleted = null,
            OnElementEvent onEventError = null,
            OnElementEvent onTaskStarted = null,
            OnElementEvent onTaskCompleted = null,
            OnElementEvent onTaskError = null,
            OnProcessEvent onProcessStarted = null,
            OnProcessEvent onProcessCompleted = null,
            OnProcessErrorEvent onProcessError = null,
            OnFlowComplete onSequenceFlowCompleted = null,
            OnFlowComplete onMessageFlowCompleted = null,
            OnElementEvent onGatewayStarted = null,
            OnElementEvent onGatewayCompleted = null,
            OnElementEvent onGatewayError = null,
            OnElementEvent onSubProcessStarted = null,
            OnElementEvent onSubProcessCompleted = null,
            OnElementEvent onSubProcessError = null,
            OnStateChange onStateChange = null,
            OnElementAborted onElementAborted = null,
            IsEventStartValid isEventStartValid = null,
            IsProcessStartValid isProcessStartValid = null,
            IsFlowValid isFlowValid = null,
            ProcessTask processBusinessRuleTask = null,
            StartManualTask beginManualTask = null,
            ProcessTask processRecieveTask = null,
            ProcessTask processScriptTask = null,
            ProcessTask processSendTask = null,
            ProcessTask processServiceTask = null,
            ProcessTask processTask = null,
            StartUserTask beginUserTask = null,
            LogLevels stateLogLevel = LogLevels.None)
        {
            ProcessInstance ret = new ProcessInstance(this, _delegates.Merge(logLine, logException, onEventStarted, onEventCompleted, onEventError,
               onTaskStarted, onTaskCompleted, onTaskError, onProcessStarted, onProcessCompleted, onProcessError,
               onSequenceFlowCompleted, onMessageFlowCompleted, onGatewayStarted, onGatewayCompleted, onGatewayError,
               onSubProcessStarted, onSubProcessCompleted, onSubProcessError, onStateChange, onElementAborted, isEventStartValid,
               isProcessStartValid, isFlowValid, processBusinessRuleTask, beginManualTask, processRecieveTask, processScriptTask,
               processSendTask, processServiceTask, processTask, beginUserTask), stateLogLevel);
            ProcessVariablesContainer variables = new ProcessVariablesContainer(pars,this,ret);
            ret.WriteLogLine((IElement)null,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, "Attempting to begin process");
            ReadOnlyProcessVariablesContainer ropvc = new ReadOnlyProcessVariablesContainer(variables);
            foreach (IElement elem in _elements.Values)
            {
                if (elem is Elements.Process)
                {
                    if (((Elements.Process)elem).IsStartValid(ropvc, ret.Delegates.IsProcessStartValid))
                    {
                        Elements.Process p = (Elements.Process)elem;
                        foreach (StartEvent se in p.StartEvents)
                        {
                            if (se.IsEventStartValid(ropvc, ret.Delegates.IsEventStartValid))
                            {
                                ret.WriteLogLine(se,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Valid Process Start[{0}] located, beginning process", se.id));
                                _TriggerDelegateAsync(ret.Delegates.OnProcessStarted,new object[] { p, new ReadOnlyProcessVariablesContainer(variables) });
                                _TriggerDelegateAsync(ret.Delegates.OnEventStarted,new object[] { se, new ReadOnlyProcessVariablesContainer(variables) });
                                ret.State.Path.StartEvent(se, null);
                                foreach (string str in variables.Keys)
                                    ret.State[se.id,str]=variables[str];
                                ret.State.Path.SucceedEvent(se);
                                _TriggerDelegateAsync(ret.Delegates.OnEventCompleted,new object[] { se, new ReadOnlyProcessVariablesContainer(se.id, ret) });
                                return ret;
                            }
                        }
                    }
                }
            }
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Unable to begin process, no valid start located");
            return null;
        }

        private void _TriggerDelegateAsync(object dgate,object[] pars)
        {
            if (dgate!=null)
            {
                switch (dgate.GetType().Name)
                {
                    case "OnElementEvent":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnElementEvent)dgate).Invoke((IStepElement)pars[0], (IReadonlyVariables)pars[1]);
                        });
                        break;
                    case "OnProcessEvent":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnProcessEvent)dgate).Invoke((IElement)pars[0], (IReadonlyVariables)pars[1]);
                        });
                        break;
                    case "OnFlowComplete":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnFlowComplete)dgate).Invoke((IElement)pars[0], (IReadonlyVariables)pars[1]);
                        });
                        break;
                    case "OnProcessErrorEvent":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnProcessErrorEvent)dgate).Invoke((IElement)pars[0], (IElement)pars[1], (IReadonlyVariables)pars[2]);
                        });
                        break;
                    case "OnStateChange":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnStateChange)dgate).Invoke((XmlDocument)pars[0]);
                        });
                        break;
                    case "StartManualTask":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((StartManualTask)dgate).Invoke((IManualTask)pars[0]);
                        });
                        break;
                    case "StartUserTask":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((StartUserTask)dgate).Invoke((IUserTask)pars[0]);
                        });
                        break;
                    case "OnElementAborted":
                        System.Threading.Tasks.Task.Run(() =>
                        {
                            ((OnElementAborted)dgate).Invoke((IElement)pars[0],(IElement)pars[1],(IReadonlyVariables)pars[2]);
                        });
                        break;
                }
            }
        }

        private AHandlingEvent[] _GetEventHandlers(EventSubTypes type,object data, AFlowNode source, IReadonlyVariables variables)
        {
            List<AHandlingEvent> ret = new List<AHandlingEvent>();
            int curCost = int.MaxValue;

            int cost;

            foreach (AHandlingEvent handler in _eventHandlers) {
                if (handler.HandlesEvent(type,data,source,variables,out cost))
                {
                    if (cost==curCost)
                        ret.Add(handler);
                    else if (cost<curCost)
                    {
                        ret = new List<AHandlingEvent>(new AHandlingEvent[] { handler });
                        curCost=cost;
                    }
                }
            }

            switch (type)
            {
                case EventSubTypes.Conditional:
                case EventSubTypes.Timer:
                    if (curCost>0)
                        ret.Clear();
                    break;
            }

            return ret.ToArray();
        }

        internal void ProcessStepComplete(ProcessInstance instance,string sourceID,string outgoingID) {
            if (sourceID!=null)
            {
                IElement elem = GetElement(sourceID);
                if (elem is AFlowNode)
                {
                    ReadOnlyProcessVariablesContainer vars = new ReadOnlyProcessVariablesContainer(sourceID,instance);
                    foreach (AHandlingEvent ahe in _GetEventHandlers(EventSubTypes.Timer, null, (AFlowNode)elem, vars))
                    {
                        if (instance.State.Path.GetStatus(ahe.id)==StepStatuses.WaitingStart)
                        {
                            Utility.AbortDelayedEvent(instance, (BoundaryEvent)ahe, sourceID);
                            instance.StateEvent.WaitOne();
                            _AbortStep(instance,sourceID, ahe, vars);
                            instance.StateEvent.Set();
                        }
                    }
                }
            }
            WriteLogLine(sourceID,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Process Step[{0}] has been completed", sourceID));
            if (outgoingID != null)
            {
                IElement elem = GetElement(outgoingID);
                if (elem != null)
                    _ProcessElement(instance,sourceID, elem);
            }
        }

        internal void ProcessStepError(ProcessInstance instance,IElement step, Exception ex) {
            instance.WriteLogLine(step,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Process Step Error occured, checking for valid Intermediate Catch Event");
            bool success = false;
            if (step is AFlowNode)
            {
                AHandlingEvent[] events = _GetEventHandlers(EventSubTypes.Error, ex, (AFlowNode)step, new ReadOnlyProcessVariablesContainer(step.id,instance,ex));
                if (events.Length>0)
                {
                    success=true;
                    foreach (AHandlingEvent ahe in events)
                    {
                        instance.WriteLogLine(step, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Valid Error handle located at {0}", ahe.id));
                        _ProcessElement(instance,step.id, ahe);
                    }
                }
            }
            if (!success)
                _TriggerDelegateAsync(instance.Delegates.OnProcessError,new object[] { ((IStepElement)step).Process, step, new ReadOnlyProcessVariablesContainer(step.id, instance, ex) });
        }

        private void _ProcessElement(ProcessInstance instance,string sourceID,IElement elem)
        {
            if (instance.IsSuspended)
            {
                instance.State.Path.SuspendElement(sourceID, elem);
                instance.MreSuspend.Set();
            }
            else
            {
                instance.WriteLogLine(sourceID,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Processing Element {0} from source {1}", new object[] { elem.id, sourceID }));
                bool abort = false;
                if (elem is AFlowNode)
                {
                    ReadOnlyProcessVariablesContainer ropvc = new ReadOnlyProcessVariablesContainer(sourceID, instance);
                    AHandlingEvent[] evnts = _GetEventHandlers(EventSubTypes.Conditional, null, ((AFlowNode)elem), ropvc);
                    foreach (AHandlingEvent ahe in evnts)
                    {
                        ProcessEvent(instance,elem.id, ahe);
                        abort|=(ahe is BoundaryEvent ? ((BoundaryEvent)ahe).CancelActivity : false);
                    }
                    if (!abort)
                    {
                        evnts = _GetEventHandlers(EventSubTypes.Timer, null, ((AFlowNode)elem), ropvc);
                        foreach (AHandlingEvent ahe in evnts)
                        {
                            TimeSpan? ts = ahe.GetTimeout(ropvc);
                            if (ts.HasValue)
                            {
                                instance.StateEvent.WaitOne();
                                instance.State.Path.DelayEventStart(ahe, elem.id, ts.Value);
                                instance.StateEvent.Set();
                                Utility.DelayStart(ts.Value, instance, (BoundaryEvent)ahe, elem.id);
                            }
                        }
                    }
                }
                if (elem is SequenceFlow)
                    _ProcessSequenceFlow(instance,(SequenceFlow)elem);
                else if (elem is MessageFlow)
                    _ProcessMessageFlow(instance, (MessageFlow)elem);
                else if (elem is AGateway)
                    _ProcessGateway(instance, sourceID, (AGateway)elem);
                else if (elem is AEvent)
                    ProcessEvent(instance, sourceID, (AEvent)elem);
                else if (elem is ATask)
                    _ProcessTask(instance, sourceID, (ATask)elem);
                else if (elem is SubProcess) 
                    _ProcessSubProcess(instance, sourceID, (SubProcess)elem);
            }
        }

        private void _ProcessSubProcess(ProcessInstance instance,string sourceID, SubProcess esp)
        {
            ReadOnlyProcessVariablesContainer variables = new ReadOnlyProcessVariablesContainer(new ProcessVariablesContainer(esp.id, instance));
            if (esp.IsStartValid(variables, instance.Delegates.IsProcessStartValid))
            {
                foreach (StartEvent se in esp.StartEvents)
                {
                    if (se.IsEventStartValid(variables,instance.Delegates.IsEventStartValid))
                    {
                        instance.WriteLogLine(se, LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Valid Sub Process Start[{0}] located, beginning process", se.id));
                        instance.StateEvent.WaitOne();
                        instance.State.Path.StartSubProcess(esp, sourceID);
                        instance.StateEvent.Set();
                        _TriggerDelegateAsync(instance.Delegates.OnSubProcessStarted,new object[] { esp, variables });
                        _TriggerDelegateAsync(instance.Delegates.OnEventStarted,new object[] { se, variables });
                        instance.State.Path.StartEvent(se, null);
                        instance.State.Path.SucceedEvent(se);
                        _TriggerDelegateAsync(instance.Delegates.OnEventCompleted,new object[] { se, new ReadOnlyProcessVariablesContainer(se.id, instance) });
                    }
                }
            }
        }

        private void _ProcessTask(ProcessInstance instance,string sourceID, ATask tsk)
        {
            instance.StateEvent.WaitOne();
            instance.State.Path.StartTask(tsk, sourceID);
            instance.StateEvent.Set();
            _TriggerDelegateAsync(instance.Delegates.OnTaskStarted,new object[] { tsk, new ReadOnlyProcessVariablesContainer(tsk.id, instance) });
            try
            {
                ProcessVariablesContainer variables = new ProcessVariablesContainer(tsk.id, instance);
                ITask task=null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                    case "RecieveTask":
                    case "SendTask":
                    case "ServiceTask":
                    case "Task":
                    case "ScriptTask":
                        task = new Org.Reddragonit.BpmEngine.Tasks.ExternalTask(tsk, variables, instance);
                        break;
                }
                ProcessTask delTask = null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                        delTask = instance.Delegates.ProcessBusinessRuleTask;
                        break;
                    case "ManualTask":
                        _TriggerDelegateAsync(instance.Delegates.BeginManualTask,new object[] { new Org.Reddragonit.BpmEngine.Tasks.ManualTask(tsk, variables, instance) });
                        break;
                    case "RecieveTask":
                        delTask = instance.Delegates.ProcessRecieveTask;
                        break;
                    case "ScriptTask":
                        ((ScriptTask)tsk).ProcessTask(task,instance.Delegates.ProcessScriptTask);
                        break;
                    case "SendTask":
                        delTask = instance.Delegates.ProcessSendTask;
                        break;
                    case "ServiceTask":
                        delTask = instance.Delegates.ProcessServiceTask;
                        break;
                    case "Task":
                        delTask = instance.Delegates.ProcessTask;
                        break;
                    case "UserTask":
                        _TriggerDelegateAsync(instance.Delegates.BeginUserTask,new object[] { new Org.Reddragonit.BpmEngine.Tasks.UserTask(tsk, variables, instance) });
                        break;
                }
                if (delTask!=null)
                    delTask.Invoke(task);
                if (task!=null)
                    instance.MergeVariables(task);
            }
            catch (Exception e)
            {
                instance.WriteLogException(tsk, new StackFrame(1, true), DateTime.Now, e);
                _TriggerDelegateAsync(instance.Delegates.OnTaskError,new object[] { tsk, new ReadOnlyProcessVariablesContainer(tsk.id, instance, e) });
                instance.StateEvent.WaitOne();
                instance.State.Path.FailTask(tsk, e);
                instance.StateEvent.Set();
            }
        }

        internal void ProcessEvent(ProcessInstance instance,string sourceID, AEvent evnt)
        {
            if (evnt is IntermediateCatchEvent)
            {
                SubProcess sp = (SubProcess)evnt.SubProcess;
                if (sp != null)
                    instance.State.Path.StartSubProcess(sp, sourceID);
            }
            instance.StateEvent.WaitOne();
            instance.State.Path.StartEvent(evnt, sourceID);
            instance.StateEvent.Set();
            _TriggerDelegateAsync(instance.Delegates.OnEventStarted,new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.id, instance) });
            if (evnt is BoundaryEvent)
            {
                if (((BoundaryEvent)evnt).CancelActivity)
                {
                    instance.StateEvent.WaitOne();
                    _AbortStep(instance,sourceID, GetElement(((BoundaryEvent)evnt).AttachedToID), new ReadOnlyProcessVariablesContainer(evnt.id, instance));
                    instance.StateEvent.Set();
                }
            }
            bool success = true;
            if (evnt is IntermediateCatchEvent || evnt is IntermediateThrowEvent)
            {
                TimeSpan? ts = evnt.GetTimeout(new ReadOnlyProcessVariablesContainer(evnt.id, instance));
                if (ts.HasValue)
                {
                    instance.StateEvent.WaitOne();
                    instance.State.SuspendStep(evnt.id, ts.Value);
                    instance.StateEvent.Set();
                    if (ts.Value.TotalMilliseconds > 0)
                    {
                        Utility.Sleep(ts.Value, instance, evnt);
                        return;
                    }
                    else
                        success = true;
                }else if (evnt is IntermediateThrowEvent)
                {
                    if (evnt.SubType.HasValue)
                    {
                        AHandlingEvent[] evnts = _GetEventHandlers(evnt.SubType.Value, ((IntermediateThrowEvent)evnt).Message, evnt, new ReadOnlyProcessVariablesContainer(evnt.id, instance));
                        foreach (AHandlingEvent tsk in evnts)
                            ProcessEvent(instance,evnt.id, tsk);
                    }
                }
            }
            else if (instance.Delegates.IsEventStartValid != null && (evnt is IntermediateCatchEvent || evnt is StartEvent))
            {
                try
                {
                    success = instance.Delegates.IsEventStartValid(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(evnt, new StackFrame(1, true), DateTime.Now, e);
                    success = false;
                }
            }
            if (!success)
            {
                instance.StateEvent.WaitOne();
                instance.State.Path.FailEvent(evnt);
                instance.StateEvent.Set();
                _TriggerDelegateAsync(instance.Delegates.OnEventError,new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.id, instance) });
            }
            else
            {
                instance.StateEvent.WaitOne();
                instance.State.Path.SucceedEvent(evnt);
                instance.StateEvent.Set();
                _TriggerDelegateAsync(instance.Delegates.OnEventCompleted,new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.id, instance) });
                if (evnt is EndEvent)
                {
                    if (((EndEvent)evnt).IsProcessEnd)
                    {
                        SubProcess sp = (SubProcess)((EndEvent)evnt).SubProcess;
                        if (sp != null)
                        {
                            instance.StateEvent.WaitOne();
                            instance.State.Path.SucceedSubProcess(sp);
                            instance.StateEvent.Set();
                            _TriggerDelegateAsync(instance.Delegates.OnSubProcessCompleted, new object[] { sp, new ReadOnlyProcessVariablesContainer(sp.id, instance) });
                        }
                        else
                        {
                            _TriggerDelegateAsync(instance.Delegates.OnProcessCompleted, new object[] { ((EndEvent)evnt).Process, new ReadOnlyProcessVariablesContainer(evnt.id, instance)});
                            instance.ProcessLock.Set();
                        }
                    }
                }
            }
        }

        private void _AbortStep(ProcessInstance instance,string sourceID,IElement element,IReadonlyVariables variables)
        {
            instance.State.Path.AbortStep(sourceID, element.id);
            _TriggerDelegateAsync(instance.Delegates.OnStepAborted, new object[] { element, GetElement(sourceID), variables });
            if (element is SubProcess)
            {
                foreach (IElement child in ((SubProcess)element).Children)
                {
                    bool abort = false;
                    switch (instance.State.Path.GetStatus(child.id)) {
                        case StepStatuses.Suspended:
                            abort=true;
                            Utility.AbortSuspendedElement(instance, child.id);
                            break;
                        case StepStatuses.Waiting:
                            abort=true;
                            break;
                    }
                    if (abort)
                        _AbortStep(instance,sourceID, child,variables);
                }
            }
        }

        private void _ProcessGateway(ProcessInstance instance,string sourceID,AGateway gw)
        {
            Definition def = null;
            foreach (IElement e in _Elements)
            {
                if (e is Definition)
                {
                    if (((Definition)e).LocateElement(gw.id) != null)
                    {
                        def = (Definition)e;
                        break;
                    }
                }
            }
            instance.StateEvent.WaitOne();
            bool gatewayComplete = false;
            if (gw.IsIncomingFlowComplete(sourceID, instance.State.Path))
                gatewayComplete = true;
            if (!gw.IsWaiting(instance.State.Path))
                instance.State.Path.StartGateway(gw, sourceID);
            if (gatewayComplete)
            {
                _TriggerDelegateAsync(instance.Delegates.OnGatewayStarted,new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.id, instance) });
                string[] outgoings = null;
                try
                {
                    outgoings = gw.EvaulateOutgoingPaths(def, instance.Delegates.IsFlowValid, new ReadOnlyProcessVariablesContainer(gw.id, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(gw, new StackFrame(1, true), DateTime.Now, e);
                    _TriggerDelegateAsync(instance.Delegates.OnGatewayError,new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.id, instance) });
                    outgoings = null;
                }
                if (outgoings == null)
                    instance.State.Path.FailGateway(gw);
                else
                    instance.State.Path.SuccessGateway(gw, outgoings);
            }
            instance.StateEvent.Set();
        }

        private void _ProcessMessageFlow(ProcessInstance instance,MessageFlow mf)
        {
            instance.StateEvent.WaitOne();
            instance.State.Path.ProcessMessageFlow(mf);
            instance.StateEvent.Set(); 
            _TriggerDelegateAsync(instance.Delegates.OnMessageFlowCompleted,new object[] { mf, new ReadOnlyProcessVariablesContainer(mf.id, instance) });
        }

        private void _ProcessSequenceFlow(ProcessInstance instance,SequenceFlow sf)
        {
            instance.StateEvent.WaitOne();
            instance.State.Path.ProcessSequenceFlow(sf);
            instance.StateEvent.Set();
            _TriggerDelegateAsync(instance.Delegates.OnSequenceFlowCompleted,new object[] { sf, new ReadOnlyProcessVariablesContainer(sf.id, instance) });
        }

        #region Logging
        internal void WriteLogLine(string elementID,LogLevels level,StackFrame sf,DateTime timestamp, string message)
        {
            WriteLogLine((IElement)(elementID == null ? null : GetElement(elementID)), level, sf, timestamp, message);
        }
        internal void WriteLogLine(IElement element, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            if (_delegates.LogLine != null)
                _delegates.LogLine.Invoke(element,sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID,StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : GetElement(elementID)), sf, timestamp, exception);
        }
        
        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if (_delegates.LogException != null)
                _delegates.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            return exception;
        }
        #endregion

        /// <summary>
        /// Called to Dispose of the given process instance.
        /// </summary>
        public void Dispose()
        {
            Utility.UnloadProcess(this);
        }
        /// <summary>
        /// Compares a given process instance to this instance to see if they are the same.
        /// </summary>
        /// <param name="obj">The Business Process instance to compare this one to.</param>
        /// <returns>true if they are the same, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is BusinessProcess)
                return ((BusinessProcess)obj)._id == _id;
            return false;
        }

        /// <summary>
        /// Returns the HashCode of the Business Process instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
