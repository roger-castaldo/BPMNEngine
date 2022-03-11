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
        private bool _isSuspended = false;
        private ManualResetEvent _mreSuspend;
        private List<object> _components;
        private Dictionary<string, IElement> _elements;
        private AHandlingEvent[] _eventHandlers = null;

        private IElement _GetElement(string id)
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

        private ProcessState _state;
        /// <summary>
        /// The Process State of the current process
        /// </summary>
        public XmlDocument State { get { return _state.Document; } }

        private AutoResetEvent _stateEvent=new AutoResetEvent(true);

        private LogLevels _stateLogLevel = LogLevels.None;

        /// <summary>
        /// The log level to use inside the state document for logging
        /// </summary>
        public LogLevels StateLogLevel { get { return _stateLogLevel; } set { _stateLogLevel = value; } }

        private ManualResetEvent _processLock;

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

        /// <summary>
        /// Called to obtain the names of all process runtime and definition constants
        /// </summary>
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

        [ThreadStatic()]
        private static BusinessProcess _current;
        /// <summary>
        /// The current Business Process inside the current thread
        /// </summary>
        public static BusinessProcess Current { get { return _current; } }

        #region delegates
        #region Ons
        private OnEventStarted _onEventStarted;
        /// <summary>
        /// Used to define the Event Started delegate
        /// </summary>
        public OnEventStarted OnEventStarted { get { return _onEventStarted; } set { _onEventStarted = value; } }

        private OnEventCompleted _onEventCompleted;
        /// <summary>
        /// Used to define the Event Completed delegate
        /// </summary>
        public OnEventCompleted OnEventCompleted{get{return _onEventCompleted;}set{_onEventCompleted = value;}}

        private OnEventError _onEventError;
        /// <summary>
        /// Used to define the Event Error delegate
        /// </summary>
        public OnEventError OnEventError{get{return _onEventError;}set{_onEventError=value;}}

        private OnTaskStarted _onTaskStarted;
        /// <summary>
        /// Used to define the Task Started delegate
        /// </summary>
        public OnTaskStarted OnTaskStarted{get{return _onTaskStarted;}set{_onTaskStarted=value;}}

        private OnTaskCompleted _onTaskCompleted;
        /// <summary>
        /// Used to define the Task Completed delegate
        /// </summary>
        public OnTaskCompleted OnTaskCompleted{get{return _onTaskCompleted;}set{_onTaskCompleted=value;}}
        
        private OnTaskError _onTaskError;
        /// <summary>
        /// Used to define the Task Error delegate
        /// </summary>
        public OnTaskError OnTaskError{get{return _onTaskError;}set{_onTaskError = value;}}

        private OnProcessStarted _onProcessStarted;
        /// <summary>
        /// Used to define the Process Started delegate
        /// </summary>
        public OnProcessStarted OnProcessStarted{get{return _onProcessStarted;}set{_onProcessStarted=value;}}
        
        private OnProcessCompleted _onProcessCompleted;
        /// <summary>
        /// Used to define the Process Completed delegate
        /// </summary>
        public OnProcessCompleted OnProcessCompleted{get{return _onProcessCompleted;}set{_onProcessCompleted = value;}}

        private OnProcessError _onProcessError;
        /// <summary>
        /// Used to define the Process Error delegate
        /// </summary>
        public OnProcessError OnProcessError { get { return _onProcessError; } set { _onProcessError = value; } }

        private OnSequenceFlowCompleted _onSequenceFlowCompleted;
        /// <summary>
        /// Used to define the Sequence Flow Complete delegate
        /// </summary>
        public OnSequenceFlowCompleted OnSequenceFlowCompleted { get { return _onSequenceFlowCompleted; } set { _onSequenceFlowCompleted = value; } }

        private OnMessageFlowCompleted _onMessageFlowCompleted;
        /// <summary>
        /// Used to define the Message Flow Complete delegate
        /// </summary>
        public OnMessageFlowCompleted OnMessageFlowCompleted { get { return _onMessageFlowCompleted; } set { _onMessageFlowCompleted = value; } }

        private OnGatewayStarted _onGatewayStarted;
        /// <summary>
        /// Used to define the Gateway Started delegate
        /// </summary>
        public OnGatewayStarted OnGatewayStarted { get { return _onGatewayStarted; } set { _onGatewayStarted = value; } }

        private OnGatewayCompleted _onGatewayCompleted;
        /// <summary>
        /// Used to define the Gateway Completed delegate
        /// </summary>
        public OnGatewayCompleted OnGatewayCompleted { get { return _onGatewayCompleted; } set { _onGatewayCompleted = value; } }

        private OnGatewayError _onGatewayError;
        /// <summary>
        /// Used to define the Gateway Error delegate
        /// </summary>
        public OnGatewayError OnGatewayError { get { return _onGatewayError; } set { _onGatewayError = value; } }

        private OnSubProcessStarted _onSubProcessStarted;
        /// <summary>
        /// Used to define the Sub Process Started delegate
        /// </summary>
        public OnSubProcessStarted OnSubProcessStarted { get { return _onSubProcessStarted; } set { _onSubProcessStarted = value; } }

        private OnSubProcessCompleted _onSubProcessCompleted;
        /// <summary>
        /// Used to define the Sub Process Completed delegate
        /// </summary>
        public OnSubProcessCompleted OnSubProcessCompleted { get { return _onSubProcessCompleted; } set { _onSubProcessCompleted = value; } }

        private OnSubProcessError _onSubProcessError;
        /// <summary>
        /// Used to define the Sub Process Error delegate
        /// </summary>
        public OnSubProcessError OnSubProcessError { get { return _onSubProcessError; } set { _onSubProcessError = value; } }

        /// <summary>
        /// Used to define the State Change delegate
        /// </summary>
        public OnStateChange OnStateChange { set { _state.OnStateChange = value; } }
        #endregion

        #region Validations
        private static bool _DefaultEventStartValid(IElement Event, IReadonlyVariables variables){return true;}
        private IsEventStartValid _isEventStartValid = new IsEventStartValid(_DefaultEventStartValid);
        /// <summary>
        /// Used to define the Is Event Start Valid delegate
        /// </summary>
        public IsEventStartValid IsEventStartValid { get { return _isEventStartValid; } set { _isEventStartValid = value; } }

        private static bool _DefaultProcessStartValid(IElement Event, IReadonlyVariables variables){return true;}
        private IsProcessStartValid _isProcessStartValid = new IsProcessStartValid(_DefaultProcessStartValid);
        /// <summary>
        /// Used to define the Is Process Start Valid delegate
        /// </summary>
        public IsProcessStartValid IsProcessStartValid { get { return _isProcessStartValid; } set { _isProcessStartValid = value; } }
        #endregion

        #region Conditions
        private static bool _DefaultFlowValid(IElement flow, IReadonlyVariables variables) { return true; }
        private IsFlowValid _isFlowValid = new IsFlowValid(_DefaultFlowValid);
        /// <summary>
        /// Used to define the Is Flow Valid delegate
        /// </summary>
        public IsFlowValid IsFlowValid { get { return _isFlowValid; } set { _isFlowValid = value; } }
        #endregion

        #region Tasks
        private ProcessBusinessRuleTask _processBusinessRuleTask;
        /// <summary>
        /// Used to define the Process Business Rule Task delegate
        /// </summary>
        public ProcessBusinessRuleTask ProcessBusinessRuleTask { get { return _processBusinessRuleTask; } set { _processBusinessRuleTask = value; } }

        private BeginManualTask _beginManualTask;
        /// <summary>
        /// Used to define the Begin Manual Task delegate
        /// </summary>
        public BeginManualTask BeginManualTask { get { return _beginManualTask; } set { _beginManualTask = value; } }

        private ProcessRecieveTask _processRecieveTask;
        /// <summary>
        /// Used to define the Process Recieve Task delegate
        /// </summary>
        public ProcessRecieveTask ProcessRecieveTask { get { return _processRecieveTask; } set { _processRecieveTask = value; } }

        private ProcessScriptTask _processScriptTask;
        /// <summary>
        /// Used to define the Process Script Task delegate
        /// </summary>
        public ProcessScriptTask ProcessScriptTask { get { return _processScriptTask; } set { _processScriptTask=value; } }

        private ProcessSendTask _processSendTask;
        /// <summary>
        /// Used to define the Process Send Task delegate
        /// </summary>
        public ProcessSendTask ProcessSendTask { get { return _processSendTask; } set { _processSendTask = value; } }

        private ProcessServiceTask _processServiceTask;
        /// <summary>
        /// Used to define the Process Service Task delegate
        /// </summary>
        public ProcessServiceTask ProcessServiceTask { get { return _processServiceTask; } set { _processServiceTask = value; } }

        private ProcessTask _processTask;
        /// <summary>
        /// Used to define the Process Task delegate
        /// </summary>
        public ProcessTask ProcessTask { get { return _processTask; } set { _processTask = value; } }

        private BeginUserTask _beginUserTask;
        /// <summary>
        /// Used to define the Begin User Task delegate
        /// </summary>
        public BeginUserTask BeginUserTask { get { return _beginUserTask; } set { _beginUserTask = value; } }

        #region TaskCallBacks
        private void _CompleteExternalTask(string taskID, ProcessVariablesContainer variables)
        {
            bool success = false;
            IElement elem = _GetElement(taskID);
            if (elem != null && elem is ATask)
            {
                _MergeVariables((ATask)elem, variables);
                success = true;
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        private void _CompleteUserTask(string taskID, ProcessVariablesContainer variables,string completedByID)
        {
            bool success = false;
            IElement elem = _GetElement(taskID);
            if (elem != null && elem is ATask)
            {
                _MergeVariables((UserTask)elem, variables, completedByID);
                success = true;
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        private void _ErrorExternalTask(string taskID, Exception ex)
        {
            bool success = false;
            IElement elem = _GetElement(taskID);
            if (elem != null && elem is ATask)
            {
                if (_onTaskError != null)
                    _onTaskError((ATask)elem, new ReadOnlyProcessVariablesContainer(elem.id, _state, this, ex));
                _stateEvent.WaitOne();
                _state.Path.FailTask((ATask)elem, ex);
                _stateEvent.Set();
                success = true;
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        /// <summary>
        /// This function is used to indicate the completion of a User Task within the given process instance.
        /// </summary>
        /// <param name="taskID">The ID of the User Task element completed.</param>
        /// <param name="variables">The variables associated with the completion of the task.  Supplying values here will update the given process variables for the next step, or it can remain empty if there are no changes to be made.</param>
        /// <param name="completedByID">Optional to specify a User ID that will show up in the state for indicating the user that completed the task.</param>
        public void CompleteUserTask(string taskID, ProcessVariablesContainer variables,string completedByID)
        {
            _CompleteUserTask(taskID, variables,completedByID);
        }

        /// <summary>
        /// This function is used to indicate an error occured completing a User Task within the given process instance.
        /// </summary>
        /// <param name="taskID">The ID of the User Task element that had the error.</param>
        /// <param name="ex">The error that occured on the User Task.</param>
        public void ErrorUserTask(string taskID, Exception ex)
        {
            _ErrorExternalTask(taskID, ex);
        }

        /// <summary>
        /// This function is used to indicate the completion of a Manual Task within the given process instance.
        /// </summary>
        /// <param name="taskID">The ID of the Manual Task element completed.</param>
        /// <param name="variables">The variables associated with the completion of the task.  Supplying values here will update the given process variables for the next step, or it can remain empty if there are no changes to be made.</param>
        public void CompleteManualTask(string taskID, ProcessVariablesContainer variables)
        {
            _CompleteExternalTask(taskID, variables);
        }

        /// <summary>
        /// This function is used to indicate an error occured completing a Manual Task within the given process instance.
        /// </summary>
        /// <param name="taskID">The ID of the Manual Task element that had the error.</param>
        /// <param name="ex">The error that occured on the Manual Task.</param>
        public void ErrorManualTask(string taskID, Exception ex)
        {
            _ErrorExternalTask(taskID, ex);
        }
        #endregion

        #endregion

        #region Logging
        private LogLine _logLine;
        /// <summary>
        /// Used to define the Log Line delegate
        /// </summary>
        public LogLine LogLine { get { return _logLine; }set { _logLine = value; } }

        private LogException _logException;
        /// <summary>
        /// Used to define the Log Exception delegate
        /// </summary>
        public LogException LogException { get { return _logException; }set { _logException = value; } }
        #endregion

        #endregion

        private BusinessProcess() {
            _id = Utility.NextRandomGuid();
            _processLock = new ManualResetEvent(false);
            _mreSuspend = new ManualResetEvent(false);
        }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition only
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        public BusinessProcess(XmlDocument doc)
            :this(doc,LogLevels.None) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition and LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="logLine">The LogLine delegate</param>
        public BusinessProcess(XmlDocument doc,LogLine logLine)
            : this(doc, LogLevels.None,logLine) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition and runtime constants
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        public BusinessProcess(XmlDocument doc,sProcessRuntimeConstant[] constants)
            : this(doc, LogLevels.None,constants) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, runtime constants and the LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        /// <param name="logLine">The LogLine delegate</param>
        public BusinessProcess(XmlDocument doc, sProcessRuntimeConstant[] constants,LogLine logLine)
            : this(doc, LogLevels.None, constants,logLine) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition and StateLogLevel
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="stateLogLevel">The log level to use for logging data into the process state</param>
        public BusinessProcess(XmlDocument doc, LogLevels stateLogLevel)
            : this(doc, stateLogLevel, null,null) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, the StateLogLevel and the LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="stateLogLevel">The log level to use for logging data into the process state</param>
        /// <param name="logLine">The LogLine delegate</param>
        public BusinessProcess(XmlDocument doc, LogLevels stateLogLevel,LogLine logLine)
            : this(doc, stateLogLevel, null,logLine) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, the StateLogLevel and runtime constants
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="stateLogLevel">The log level to use for logging data into the process state</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        public BusinessProcess(XmlDocument doc, LogLevels stateLogLevel, sProcessRuntimeConstant[] constants)
            : this(doc, stateLogLevel, constants, null) { }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, StateLogLevel, runtime constants and LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="stateLogLevel">The log level to use for logging data into the process state</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        /// <param name="logLine">The LogLine delegate</param>
        public BusinessProcess(XmlDocument doc, LogLevels stateLogLevel,sProcessRuntimeConstant[] constants,LogLine logLine)
        {
            _id = Utility.NextRandomGuid();
            _stateLogLevel = stateLogLevel;
            _constants = constants;
            _logLine = logLine;
            List<Exception> exceptions = new List<Exception>();
            _processLock = new ManualResetEvent(false);
            _mreSuspend = new ManualResetEvent(false);
            _doc = new XmlDocument();
            _doc.LoadXml(doc.OuterXml);
            _current = this;
            BpmEngine.ElementTypeCache elementMapCache = new BpmEngine.ElementTypeCache();
            DateTime start = DateTime.Now;
            WriteLogLine((IElement)null,LogLevels.Info,new StackFrame(1,true),DateTime.Now,"Producing new Business Process from XML Document");
            _components = new List<object>();
            _elements = new Dictionary<string, Interfaces.IElement>();
            XmlPrefixMap map = new XmlPrefixMap(this);
            ConcurrentQueue<System.Threading.Tasks.Task> loadTasks = new ConcurrentQueue<System.Threading.Tasks.Task>();
            foreach (XmlNode n in doc.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (map.Load((XmlElement)n))
                        elementMapCache.MapIdeals(map);
                    IElement elem = Utility.ConstructElementType((XmlElement)n, ref map,ref elementMapCache,null);
                    if (elem != null)
                    {
                        _components.Add(elem);
                        if (elem is AParentElement)
                            ((AParentElement)elem).LoadChildren(ref map, ref elementMapCache,ref loadTasks);
                    }
                    else
                        _components.Add(n);
                }
                else
                    _components.Add(n);
            }
            System.Threading.Tasks.Task tsk;
            while(loadTasks.TryDequeue(out tsk))
                tsk.Wait();
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
            if (exceptions.Count == 0)
            {
                List<AHandlingEvent> tmp = new List<AHandlingEvent>();
                foreach (IElement elem in _Elements)
                {
                    if (elem is AHandlingEvent)
                        tmp.Add((AHandlingEvent)elem);
                    _ValidateElement((AElement)elem, ref exceptions);
                }
                _eventHandlers=tmp.ToArray();
            }
            if (exceptions.Count != 0)
            {
                Exception ex = new InvalidProcessDefinitionException(exceptions);
                WriteLogException((IElement)null,new StackFrame(1, true), DateTime.Now, ex);
                throw ex;
            }
            
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Time to load Process Document {0}ms",DateTime.Now.Subtract(start).TotalMilliseconds));
            _state = new ProcessState(this,new ProcessStepComplete(_ProcessStepComplete), new ProcessStepError(_ProcessStepError));
            _InitDefinition();
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
        /// Called to load the process state from an XmlDocument
        /// </summary>
        /// <param name="doc">The process state document</param>
        /// <returns>true if succeeded or false if failed</returns>
        public bool LoadState(XmlDocument doc)
        {
            return LoadState(doc, false);
        }

        /// <summary>
        /// Called to load the process state from an XmlDocument
        /// </summary>
        /// <param name="doc">The process state document</param>
        /// <param name="autoResume">set true if the process was suspended and needs to resume once loaded</param>
        /// <returns>true if successed or false if failed</returns>
        public bool LoadState(XmlDocument doc,bool autoResume)
        {
            _current = this;
            if (_state.Load(doc))
            {
                WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "State loaded for Business Process");
                _isSuspended = _state.IsSuspended;
                if (autoResume&&_isSuspended)
                    Resume();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called to Resume a suspended process.  Will fail if the process is not currently suspended.
        /// </summary>
        public void Resume()
        {
            _current = this;
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Attempting to resmue Business Process");
            if (_isSuspended)
            {
                _isSuspended = false;
                sSuspendedStep[] resumeSteps = _state.ResumeSteps;
                _state.Resume();
                if (resumeSteps != null)
                {
                    foreach (sSuspendedStep ss in resumeSteps)
                        _ProcessStepComplete(ss.IncomingID, ss.ElementID);
                }
                foreach (sStepSuspension ss in _state.SuspendedSteps)
                {
                    if (DateTime.Now.Ticks < ss.EndTime.Ticks)
                        Utility.Sleep(ss.EndTime.Subtract(DateTime.Now), this, (AEvent)_GetElement(ss.id));
                    else
                        CompleteTimedEvent((AEvent)_GetElement(ss.id));
                }
                WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Business Process Resume Complete");
            }
            else
            {
                Exception ex = new NotSuspendedException();
                WriteLogException((IElement)null,new StackFrame(1, true), DateTime.Now, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Called to render a PNG image of the process at its current state
        /// </summary>
        /// <param name="outputVariables">Set true to include outputting variables into the image</param>
        /// <returns>A Bitmap containing a rendered image of the process at its current state</returns>
        public byte[] Diagram(bool outputVariables, ImageOuputTypes type)
        {
            return _Diagram(outputVariables).ToFile(type);
        }

        private Image _Diagram(bool outputVariables)
        {
            if (!Image.CanUse)
                throw new Exception("Unable to produce a Diagram or Animated Diagram as there is no valid Drawing Assembly Loaded, please add either System.Drawing.Common or SkiaSharp");
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
                        ret.DrawImage(d.Render(_state.Path, ((Definition)elem)), new Point(_DEFAULT_PADDING / 2, padding));
                        padding += d.Size.Height + _DEFAULT_PADDING;
                    }
                }
            }
            if (outputVariables)
                ret = _AppendVariables(ret);
            return ret;
        }

        private Image _ProduceVariablesImage(Image diagram)
        {
            Size sz = diagram.MeasureString("Variables");
            int varHeight = (int)sz.Height + 2;
            string[] keys = _state[null];
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
                if (_state[null, keys[x]] != null)
                {
                    if (_state[null, keys[x]].GetType().IsArray)
                    {
                        val = "";
                        foreach (object o in (IEnumerable)_state[null, keys[x]])
                            val += string.Format("{0},", o);
                        val = val.Substring(0, val.Length - 1);
                    }
                    else if (_state[null, keys[x]] is Hashtable)
                    {
                        val = "{";
                        foreach (string key in ((Hashtable)_state[null, keys[x]]).Keys)
                            val += string.Format("{{\"{0}\":\"{1}\"}},", key, ((Hashtable)_state[null, keys[x]])[key]);
                        val = val.Substring(0, val.Length - 1) + "}";
                    }
                    else
                        val = _state[null, keys[x]].ToString();
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

        private Image _AppendVariables(Image diagram)
        {
            Image vmap = _ProduceVariablesImage(diagram);
            Image ret = new Image(diagram.Size.Width + _DEFAULT_PADDING + vmap.Size.Width, Math.Max(diagram.Size.Height, vmap.Size.Height + _DEFAULT_PADDING));
            ret.Clear(Color.White);
            ret.DrawImage(diagram, new Point(0, 0));
            ret.DrawImage(vmap, new Point(ret.Size.Width + _DEFAULT_PADDING, _DEFAULT_PADDING));
            ret.Flush();
            return ret;
        }

        /// <summary>
        /// Called to render an animated version of the process (output in GIF format).  This will animate each step of the process using the current state.
        /// </summary>
        /// <param name="outputVariables">Set true to output the variables into the diagram</param>
        /// <returns>a binary array of data containing the animated GIF</returns>
        public byte[] Animate(bool outputVariables)
        {
            _current = this;
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Animation{0}", new object[] { (outputVariables ? " with variables" : " without variables") }));
            MemoryStream ms = new MemoryStream();
            using (Drawing.GifEncoder enc = new Drawing.GifEncoder(ms))
            {
                enc.FrameDelay = _ANIMATION_DELAY;
                _state.Path.StartAnimation();
                Image bd = _Diagram(false);
                enc.AddFrame(new Drawing.GifEncoder.sFramePart[] { new Drawing.GifEncoder.sFramePart((outputVariables ? _AppendVariables(bd) : bd)) });
                while (_state.Path.HasNext())
                {
                    string nxtStep = _state.Path.MoveToNextStep();
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
                                        Image img = d.RenderElement(_state.Path, (Definition)elem, nxtStep, out rect);
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
                            frames.Add(new Drawing.GifEncoder.sFramePart(_ProduceVariablesImage(bd), bd.Size.Width + _DEFAULT_PADDING, _DEFAULT_PADDING));
                        enc.AddFrame(frames.ToArray());
                    }
                }
                _state.Path.FinishAnimation();
            }
            return ms.ToArray();
        }

        /// <summary>
        /// Called to clone a new instance of the process.  This would be used for efficincy when executing multiple instances since this will have already read and loaded the definition.
        /// </summary>
        /// <param name="includeState">Set true to include the current process state</param>
        /// <param name="includeDelegates">Set true to include all the delegates</param>
        /// <returns>A new instance of the current BusinessProcess with the process defintion already loaded</returns>
        public BusinessProcess Clone(bool includeState,bool includeDelegates)
        {
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Cloning Business Process {0} {1}",new object[] {
                (includeState ? "including state":"without state"),
                (includeDelegates ? "including delegates" : "without delegates")
            }));
            BusinessProcess ret = new BusinessProcess();
            ret._doc = _doc;
            ret._components = new List<object>(_components.ToArray());
            ret._eventHandlers = new List<AHandlingEvent>(_eventHandlers).ToArray();
            ret._InitDefinition();
            ret._constants = _constants;
            ret._elements = _elements;
            ret._stateLogLevel = _stateLogLevel;
            if (includeState)
                ret._state = _state;
            else
                ret._state = new ProcessState(ret,new ProcessStepComplete(ret._ProcessStepComplete), new ProcessStepError(ret._ProcessStepError));
            if (includeDelegates)
            {
                ret.OnEventStarted = OnEventStarted;
                ret.OnEventCompleted = OnEventCompleted;
                ret.OnEventError = OnEventError;
                ret.OnTaskStarted = OnTaskStarted;
                ret.OnTaskCompleted = OnTaskCompleted;
                ret.OnTaskError = OnTaskError;
                ret.OnProcessStarted = OnProcessStarted;
                ret.OnProcessCompleted = OnProcessCompleted;
                ret.OnProcessError = OnProcessError;
                ret.OnSubProcessStarted = OnSubProcessStarted;
                ret.OnSubProcessCompleted = OnSubProcessCompleted;
                ret.OnSubProcessError = OnSubProcessError;
                ret.OnSequenceFlowCompleted = OnSequenceFlowCompleted;
                ret.OnMessageFlowCompleted = OnMessageFlowCompleted;
                ret.IsEventStartValid = IsEventStartValid;
                ret.IsProcessStartValid = IsProcessStartValid;
                ret.IsFlowValid = IsFlowValid;
                ret.ProcessBusinessRuleTask = ProcessBusinessRuleTask;
                ret.BeginManualTask = BeginManualTask;
                ret.ProcessRecieveTask = ProcessRecieveTask;
                ret.ProcessScriptTask = ProcessScriptTask;
                ret.ProcessSendTask = ProcessSendTask;
                ret.ProcessServiceTask = ProcessServiceTask;
                ret.ProcessTask = ProcessTask;
                ret.BeginUserTask = BeginUserTask;
                ret.LogException = LogException;
                ret.LogLine = LogLine;
            }
            return ret;
        }

        /// <summary>
        /// Called to start this instance of the defined BusinessProcess
        /// </summary>
        /// <param name="variables">The variables to start the process with</param>
        /// <returns>true if the process was successfully started</returns>
        public bool BeginProcess(ProcessVariablesContainer variables)
        {
            _current = this;
            variables.SetProcess(this);
            WriteLogLine((IElement)null,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, "Attempting to begin process");
            bool ret = false;
            ReadOnlyProcessVariablesContainer ropvc = new ReadOnlyProcessVariablesContainer(variables);
            foreach (IElement elem in _elements.Values)
            {
                if (elem is Elements.Process)
                {
                    if (((Elements.Process)elem).IsStartValid(ropvc, _isProcessStartValid))
                    {
                        Elements.Process p = (Elements.Process)elem;
                        foreach (StartEvent se in p.StartEvents)
                        {
                            if (se.IsEventStartValid(ropvc, _isEventStartValid))
                            {
                                WriteLogLine(se,LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Valid Process Start[{0}] located, beginning process", se.id));
                                if (_onProcessStarted != null)
                                    _onProcessStarted(p, new ReadOnlyProcessVariablesContainer(variables));
                                if (_onEventStarted!=null)
                                    _onEventStarted(se, new ReadOnlyProcessVariablesContainer(variables));
                                _state.Path.StartEvent(se, null);
                                foreach (string str in variables.Keys)
                                    _state[se.id,str]=variables[str];
                                _state.Path.SucceedEvent(se);
                                if (_onEventCompleted!=null)
                                    _onEventCompleted(se, new ReadOnlyProcessVariablesContainer(se.id, _state,this));
                                ret=true;
                            }
                        }
                    }
                }
                if (ret)
                    break;
            }
            if (!ret)
                WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Unable to begin process, no valid start located");
            return ret;
        }

        /// <summary>
        /// Called to suspend a running process
        /// </summary>
        public void Suspend()
        {
            WriteLogLine((IElement)null,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Suspending Business Process");
            _isSuspended = true;
            _state.Suspend();
            _mreSuspend.WaitOne(5000);
            if (_state.OnStateChange != null)
                _state.OnStateChange(_state.Document);
        }

        #region ProcessLock
        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete
        /// </summary>
        /// <returns>the result of calling WaitOne on the Process Complete manual reset event</returns>
        public bool WaitForCompletion()
        {
            return _processLock.WaitOne();
        }

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout for the process to complete</param>
        /// <returns>the result of calling WaitOne(millisecondsTimeout) on the Process Complete manual reset event</returns>
        public bool WaitForCompletion(int millisecondsTimeout)
        {
            return _processLock.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout
        /// </summary>
        /// <param name="timeout">The timeout for the process to complete</param>
        /// <returns>the result of calling WaitOne(timeout) on the Process Complete manual reset event</returns>
        public bool WaitForCompletion(TimeSpan timeout)
        {
            return _processLock.WaitOne(timeout);
        }

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout and exit context
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout for the process to complete</param>
        /// <param name="exitContext">The exitContext variable</param>
        /// <returns>the result of calling WaitOne(millisecondsTimeout,exitContext) on the Process Complete manual reset event</returns>
        public bool WaitForCompletion(int millisecondsTimeout,bool exitContext)
        {
            return _processLock.WaitOne(millisecondsTimeout,exitContext);
        }

        /// <summary>
        /// Used to lock a Thread into waiting for the process to complete including a timeout and exit context
        /// </summary>
        /// <param name="timeout">The timeout for the process to complete</param>
        /// <param name="exitContext">The exitContext variable</param>
        /// <returns>the result of calling WaitOne(timeout,exitContext) on the Process Complete manual reset event</returns>
        public bool WaitForCompletion(TimeSpan timeout,bool exitContext)
        {
            return _processLock.WaitOne(timeout,exitContext);
        }

        #endregion

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

        private void _ProcessStepComplete(string sourceID,string outgoingID) {
            _current = this;
            WriteLogLine(sourceID,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Process Step[{0}] has been completed", sourceID));
            if (outgoingID != null)
            {
                IElement elem = _GetElement(outgoingID);
                if (elem != null)
                    _ProcessElement(sourceID, elem);
            }
        }

        private void _ProcessStepError(IElement step, Exception ex) {
            _current = this;
            WriteLogLine(step,LogLevels.Info, new StackFrame(1, true), DateTime.Now, "Process Step Error occured, checking for valid Intermediate Catch Event");
            bool success = false;
            if (step is AFlowNode)
            {
                AHandlingEvent[] events = _GetEventHandlers(EventSubTypes.Error, ex, (AFlowNode)step, new ReadOnlyProcessVariablesContainer(step.id,_state,this,ex));
                if (events.Length>0)
                {
                    success=true;
                    foreach (AHandlingEvent ahe in events)
                    {
                        WriteLogLine(step, LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Valid Error handle located at {0}", ahe.id));
                        _ProcessElement(step.id, ahe);
                    }
                }
            }
            if (!success)
            {
                if (_onProcessError!=null)
                    _onProcessError.Invoke(((IStepElement)step).Process,step,new ReadOnlyProcessVariablesContainer(step.id,_state,this,ex));
            }
        }

        private void _ProcessElement(string sourceID,IElement elem)
        {
            if (_isSuspended)
            {
                _state.Path.SuspendElement(sourceID, elem);
                _mreSuspend.Set();
            }
            else
            {
                WriteLogLine(sourceID,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Processing Element {0} from source {1}", new object[] { elem.id, sourceID }));
                _current = this;
                bool abort = false;
                if (elem is AFlowNode)
                {
                    ReadOnlyProcessVariablesContainer ropvc = new ReadOnlyProcessVariablesContainer(sourceID, _state, this);
                    AHandlingEvent[] evnts = _GetEventHandlers(EventSubTypes.Conditional, null, ((AFlowNode)elem), ropvc);
                    foreach (AHandlingEvent ahe in evnts)
                    {
                        _ProcessEvent(elem.id, ahe);
                        abort|=(ahe is BoundaryEvent ? ((BoundaryEvent)ahe).CancelActivity : false);
                    }
                    if (!abort)
                    {
                        evnts = _GetEventHandlers(EventSubTypes.Timer, null, ((AFlowNode)elem), ropvc);
                        foreach (AHandlingEvent ahe in evnts)
                        {
                            TimeSpan? ts = ahe.GetTimeout(ropvc);
                            if (ts.HasValue)
                                Utility.DelayStart(ts.Value, this, (BoundaryEvent)ahe, elem.id);
                        }
                    }
                }
                if (elem is SequenceFlow)
                    _ProcessSequenceFlow((SequenceFlow)elem);
                else if (elem is MessageFlow)
                    _ProcessMessageFlow((MessageFlow)elem);
                else if (elem is AGateway)
                    _ProcessGateway(sourceID, (AGateway)elem);
                else if (elem is AEvent)
                    _ProcessEvent(sourceID, (AEvent)elem);
                else if (elem is ATask)
                    _ProcessTask(sourceID, (ATask)elem);
                else if (elem is SubProcess) ;
                    _ProcessSubProcess(sourceID, (SubProcess)elem);
            }
        }

        private void _ProcessSubProcess(string sourceID, SubProcess esp)
        {
            ReadOnlyProcessVariablesContainer variables = new ReadOnlyProcessVariablesContainer(new ProcessVariablesContainer(esp.id, _state, this));
            if (esp.IsStartValid(variables, _isProcessStartValid))
            {
                foreach (StartEvent se in esp.StartEvents)
                {
                    if (se.IsEventStartValid(variables, _isEventStartValid))
                    {
                        WriteLogLine(se, LogLevels.Info, new StackFrame(1, true), DateTime.Now, string.Format("Valid Sub Process Start[{0}] located, beginning process", se.id));
                        _stateEvent.WaitOne();
                        _state.Path.StartSubProcess(esp, sourceID);
                        _stateEvent.Set();
                        if (_onSubProcessStarted!= null)
                            _onSubProcessStarted(esp, new ReadOnlyProcessVariablesContainer(variables));
                        if (_onEventStarted != null)
                            _onEventStarted(se, new ReadOnlyProcessVariablesContainer(variables));
                        _state.Path.StartEvent(se, null);
                        _state.Path.SucceedEvent(se);
                        if (_onEventCompleted != null)
                            _onEventCompleted(se, new ReadOnlyProcessVariablesContainer(se.id, _state, this));
                    }
                }
            }
        }

        private void _ProcessTask(string sourceID, ATask tsk)
        {
            _stateEvent.WaitOne();
            _state.Path.StartTask(tsk, sourceID);
            _stateEvent.Set();
            if (_onTaskStarted != null)
                _onTaskStarted(tsk, new ReadOnlyProcessVariablesContainer(tsk.id, _state, this));
            try
            {
                ProcessVariablesContainer variables = new ProcessVariablesContainer(tsk.id, _state, this);
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                        _processBusinessRuleTask(tsk, ref variables);
                        _MergeVariables(tsk, variables);
                        break;
                    case "ManualTask":
                        _beginManualTask(tsk, variables, new CompleteManualTask(_CompleteExternalTask), new ErrorManualTask(_ErrorExternalTask));
                        break;
                    case "RecieveTask":
                        _processRecieveTask(tsk, ref variables);
                        _MergeVariables(tsk, variables);
                        break;
                    case "ScriptTask":
                        ((ScriptTask)tsk).ProcessTask(ref variables, _processScriptTask);
                        _MergeVariables(tsk, variables);
                        break;
                    case "SendTask":
                        _processSendTask(tsk, ref variables);
                        _MergeVariables(tsk, variables);
                        break;
                    case "ServiceTask":
                        _processServiceTask(tsk, ref variables);
                        _MergeVariables(tsk, variables);
                        break;
                    case "Task":
                        _processTask(tsk, ref variables);
                        _MergeVariables(tsk, variables);
                        break;
                    case "UserTask":
                        _beginUserTask(tsk, variables, new CompleteUserTask(_CompleteUserTask), new ErrorUserTask(_ErrorExternalTask));
                        break;
                }
            }
            catch (Exception e)
            {
                WriteLogException(tsk, new StackFrame(1, true), DateTime.Now, e);
                if (_onTaskError != null)
                    _onTaskError(tsk, new ReadOnlyProcessVariablesContainer(tsk.id, _state, this, e));
                _stateEvent.WaitOne();
                _state.Path.FailTask(tsk, e);
                _stateEvent.Set();
            }
        }

        private void _ProcessEvent(string sourceID, AEvent evnt)
        {
            if (evnt is BoundaryEvent)
            {
                if (((BoundaryEvent)evnt).CancelActivity)
                {
                    _stateEvent.WaitOne();
                    _AbortStep(sourceID, _GetElement(((BoundaryEvent)evnt).AttachedToID));
                    _stateEvent.Set();
                }
            }
            if (evnt is IntermediateCatchEvent)
            {
                SubProcess sp = (SubProcess)evnt.SubProcess;
                if (sp != null)
                    _state.Path.StartSubProcess(sp, sourceID);
            }
            _stateEvent.WaitOne();
            _state.Path.StartEvent(evnt, sourceID);
            _stateEvent.Set();
            if (_onEventStarted != null)
                _onEventStarted(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
            bool success = true;
            if (evnt is IntermediateCatchEvent || evnt is IntermediateThrowEvent)
            {
                TimeSpan? ts = evnt.GetTimeout(new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
                if (ts.HasValue)
                {
                    _stateEvent.WaitOne();
                    _state.SuspendStep(evnt.id, ts.Value);
                    _stateEvent.Set();
                    if (ts.Value.TotalMilliseconds > 0)
                    {
                        Utility.Sleep(ts.Value, this, evnt);
                        return;
                    }
                    else
                        success = true;
                }else if (evnt is IntermediateThrowEvent)
                {
                    if (evnt.SubType.HasValue)
                    {
                        AHandlingEvent[] evnts = _GetEventHandlers(evnt.SubType.Value, ((IntermediateThrowEvent)evnt).Message, evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
                        foreach (AHandlingEvent tsk in evnts)
                            _ProcessEvent(evnt.id, tsk);
                    }
                }
            }
            else if (_isEventStartValid != null && (evnt is IntermediateCatchEvent || evnt is StartEvent))
            {
                try
                {
                    success = _isEventStartValid(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
                }
                catch (Exception e)
                {
                    WriteLogException(evnt, new StackFrame(1, true), DateTime.Now, e);
                    success = false;
                }
            }
            if (!success)
            {
                _stateEvent.WaitOne();
                _state.Path.FailEvent(evnt);
                _stateEvent.Set();
                if (_onEventError != null)
                    _onEventError(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
            }
            else
            {
                _stateEvent.WaitOne();
                _state.Path.SucceedEvent(evnt);
                _stateEvent.Set();
                if (_onEventCompleted != null)
                    _onEventCompleted(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
                if (evnt is EndEvent)
                {
                    if (((EndEvent)evnt).IsProcessEnd)
                    {
                        SubProcess sp = (SubProcess)((EndEvent)evnt).SubProcess;
                        if (sp != null)
                        {
                            _stateEvent.WaitOne();
                            _state.Path.SucceedSubProcess(sp);
                            _stateEvent.Set();
                            if (_onSubProcessCompleted != null)
                                _onSubProcessCompleted(sp, new ReadOnlyProcessVariablesContainer(sp.id, _state, this));
                        }
                        else
                        {
                            if (_onProcessCompleted != null)
                                _onProcessCompleted(((EndEvent)evnt).Process, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
                            _processLock.Set();
                        }
                    }
                }
            }
        }

        private void _AbortStep(string sourceID,IElement element)
        {
            _state.Path.AbortStep(sourceID, element.id);
            if (element is SubProcess)
            {
                foreach (IElement child in ((SubProcess)element).Children)
                {
                    bool abort = false;
                    switch (_state.Path.GetStatus(child.id)) {
                        case StepStatuses.Suspended:
                            abort=true;
                            Utility.AbortSuspendedElement(this, child.id);
                            break;
                        case StepStatuses.Waiting:
                            abort=true;
                            break;
                    }
                    if (abort)
                        _AbortStep(sourceID, child);
                }
            }
        }

        private void _ProcessGateway(string sourceID,AGateway gw)
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
            _stateEvent.WaitOne();
            bool gatewayComplete = false;
            if (gw.IsIncomingFlowComplete(sourceID, _state.Path))
                gatewayComplete = true;
            if (!gw.IsWaiting(_state.Path))
                _state.Path.StartGateway(gw, sourceID);
            if (gatewayComplete)
            {
                if (_onGatewayStarted != null)
                    _onGatewayStarted(gw, new ReadOnlyProcessVariablesContainer(gw.id, _state, this));
                string[] outgoings = null;
                try
                {
                    outgoings = gw.EvaulateOutgoingPaths(def, _isFlowValid, new ReadOnlyProcessVariablesContainer(gw.id, _state, this));
                }
                catch (Exception e)
                {
                    WriteLogException(gw, new StackFrame(1, true), DateTime.Now, e);
                    if (_onGatewayError != null)
                        _onGatewayError(gw, new ReadOnlyProcessVariablesContainer(gw.id, _state, this));
                    outgoings = null;
                }
                if (outgoings == null)
                    _state.Path.FailGateway(gw);
                else
                    _state.Path.SuccessGateway(gw, outgoings);
            }
            _stateEvent.Set();
        }

        private void _ProcessMessageFlow(MessageFlow mf)
        {
            _stateEvent.WaitOne();
            _state.Path.ProcessMessageFlow(mf);
            _stateEvent.Set();
            if (_onMessageFlowCompleted != null)
                _onMessageFlowCompleted(mf, new ReadOnlyProcessVariablesContainer(mf.id, _state, this));
        }

        private void _ProcessSequenceFlow(SequenceFlow sf)
        {
            _stateEvent.WaitOne();
            _state.Path.ProcessSequenceFlow(sf);
            _stateEvent.Set();
            if (_onSequenceFlowCompleted != null)
                _onSequenceFlowCompleted(sf, new ReadOnlyProcessVariablesContainer(sf.id, _state, this));
        }

        internal void CompleteTimedEvent(AEvent evnt)
        {
            _stateEvent.WaitOne();
            _state.Path.SucceedEvent(evnt);
            _stateEvent.Set();
            if (_onEventCompleted != null)
                _onEventCompleted(evnt, new ReadOnlyProcessVariablesContainer(evnt.id, _state, this));
        }

        internal void StartTimedEvent(BoundaryEvent evnt,string sourceID)
        {
            _ProcessEvent(sourceID, evnt);
        }

        private void _MergeVariables(UserTask task, ProcessVariablesContainer variables, string completedByID)
        {
            _MergeVariables((ATask)task, variables, completedByID);
        }

        private void _MergeVariables(ATask task, ProcessVariablesContainer variables)
        {
            _MergeVariables(task, variables, null);
        }

        private void _MergeVariables(ATask task, ProcessVariablesContainer variables,string completedByID)
        {
            WriteLogLine(task,LogLevels.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Merging variables from Task[{0}] complete by {1} into the state", new object[] { task.id, completedByID }));
            _stateEvent.WaitOne();
            foreach (string str in variables.Keys)
                {
                    object left = variables[str];
                    object right = _state[task.id, str];
                    if (!_IsVariablesEqual(left,right))
                        _state[task.id, str] = left;
                }
                if (_onTaskCompleted != null)
                    _onTaskCompleted(task, new ReadOnlyProcessVariablesContainer(task.id, _state,this));
                if (task is UserTask)
                    _state.Path.SucceedTask((UserTask)task,completedByID);
                else
                    _state.Path.SucceedTask(task);
            _stateEvent.Set();
        }

        private bool _IsVariablesEqual(object left, object right)
        {
            if (left == null && right != null)
                return false;
            else if (left != null && right == null)
                return false;
            else if (left == null && right == null)
                return true;
            else 
            {
                if (left is Array)
                {
                    if (!(right is Array))
                        return false;
                    else
                    {
                        Array aleft = (Array)left;
                        Array aright = (Array)right;
                        if (aleft.Length != aright.Length)
                            return false;
                        for (int x = 0; x < aleft.Length; x++)
                        {
                            if (!_IsVariablesEqual(aleft.GetValue(x), aright.GetValue(x)))
                                return false;
                        }
                        return true;
                    }
                }
                else if (left is Hashtable)
                {
                    if (!(right is Hashtable))
                        return false;
                    else
                    {
                        Hashtable hleft = (Hashtable)left;
                        Hashtable hright = (Hashtable)right;
                        if (hleft.Count != hright.Count)
                            return false;
                        foreach (object key in hleft.Keys)
                        {
                            if (!hright.ContainsKey(key))
                                return false;
                            else if (!_IsVariablesEqual(hleft[key], hright[key]))
                                return false;
                        }
                        foreach (object key in hright.Keys)
                        {
                            if (!hleft.ContainsKey(key))
                                return false;
                        }
                        return true;
                    }
                }
                else
                {
                    try { return left.Equals(right); }
                    catch (Exception e) { WriteLogException((string)null,new StackFrame(2,true),DateTime.Now,e); return false; }
                }
            }
        }

        #region Logging
        internal void WriteLogLine(string elementID,LogLevels level,StackFrame sf,DateTime timestamp, string message)
        {
            WriteLogLine((IElement)(elementID == null ? null : _GetElement(elementID)), level, sf, timestamp, message);
        }
        internal void WriteLogLine(IElement element, LogLevels level, StackFrame sf, DateTime timestamp, string message)
        {
            if ((int)level <= (int)_stateLogLevel && _state!=null)
                _state.LogLine((element==null ? null : element.id),sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
            if (_logLine != null)
                _logLine.Invoke(element,sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID,StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : _GetElement(elementID)), sf, timestamp, exception);
        }
        
        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if ((int)LogLevels.Error <= (int)_stateLogLevel)
                _state.LogException((element==null ? null : element.id),sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
            if (_logException != null)
                _logException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
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
