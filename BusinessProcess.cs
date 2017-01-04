using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Elements.Collaborations;
using Org.Reddragonit.BpmEngine.Elements.Processes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public sealed class BusinessProcess
    {
        private const int _ANIMATION_DELAY = 1000;
        private const int _DEFAULT_PADDING = 100;
        private const int _VARIABLE_NAME_WIDTH = 200;
        private const int _VARIABLE_VALUE_WIDTH = 300;
        private const int _VARIABLE_IMAGE_WIDTH = _VARIABLE_NAME_WIDTH+_VARIABLE_VALUE_WIDTH;

        private List<object> _components;
        private List<IElement> _Elements
        {
            get
            {
                List<IElement> ret = new List<IElement>();
                foreach (object obj in _components)
                {
                    if (new List<Type>(obj.GetType().GetInterfaces()).Contains(typeof(IElement)))
                        ret.Add((IElement)obj);
                }
                return ret;
            }
        }
        private List<IElement> _FullElements
        {
            get
            {
                List<IElement> ret = new List<IElement>();
                foreach (IElement elem in _Elements){
                    _RecurAddChildren(elem, ref ret);
                }
                return ret;
            }
        }

        private void _RecurAddChildren(IElement parent, ref List<IElement> elems)
        {
            elems.Add(parent);
            if (parent is IParentElement)
            {
                foreach (IElement elem in ((IParentElement)parent).Children)
                    _RecurAddChildren(elem, ref elems);
            }
        }

        private XmlDocument _doc;
        public XmlDocument Document { get { return _doc; } }

        private ProcessState _state;
        public ProcessState State { get { return _state; } }

        private ManualResetEvent _processLock;

        #region delegates
        #region Ons
        private OnEventStarted _onEventStarted;
        public OnEventStarted OnEventStarted { get { return _onEventStarted; } set { _onEventStarted = value; } }

        private OnEventCompleted _onEventCompleted;
        public OnEventCompleted OnEventCompleted{get{return _onEventCompleted;}set{_onEventCompleted = value;}}

        private OnEventError _onEventError;
        public OnEventError OnEventError{get{return _onEventError;}set{_onEventError=value;}}

        private OnTaskStarted _onTaskStarted;
        public OnTaskStarted OnTaskStarted{get{return _onTaskStarted;}set{_onTaskStarted=value;}}

        private OnTaskCompleted _onTaskCompleted;
        public OnTaskCompleted OnTaskCompleted{get{return _onTaskCompleted;}set{_onTaskCompleted=value;}}
        
        private OnTaskError _onTaskError;
        public OnTaskError OnTaskError{get{return _onTaskError;}set{_onTaskError = value;}}

        private OnProcessStarted _onProcessStarted;
        public OnProcessStarted OnProcessStarted{get{return _onProcessStarted;}set{_onProcessStarted=value;}}
        
        private OnProcessCompleted _onProcessCompleted;
        public OnProcessCompleted OnProcessCompleted{get{return _onProcessCompleted;}set{_onProcessCompleted = value;}}

        private OnProcessError _onProcessError;
        public OnProcessError OnProcessError { get { return _onProcessError; } set { _onProcessError = value; } }

        private OnSequenceFlowCompleted _onSequenceFlowCompleted;
        public OnSequenceFlowCompleted OnSequenceFlowCompleted { get { return _onSequenceFlowCompleted; } set { _onSequenceFlowCompleted = value; } }

        private OnMessageFlowCompleted _onMessageFlowCompleted;
        public OnMessageFlowCompleted OnMessageFlowCompleted { get { return _onMessageFlowCompleted; } set { _onMessageFlowCompleted = value; } }

        private OnGatewayStarted _onGatewayStarted;
        public OnGatewayStarted OnGatewayStarted { get { return _onGatewayStarted; } set { _onGatewayStarted = value; } }

        private OnGatewayCompleted _onGatewayCompleted;
        public OnGatewayCompleted OnGatewayCompleted { get { return _onGatewayCompleted; } set { _onGatewayCompleted = value; } }

        private OnGatewayError _onGatewayError;
        public OnGatewayError OnGatewayError { get { return _onGatewayError; } set { _onGatewayError = value; } }

        public OnStateChange OnStateChange { set { _state.OnStateChange = value; } }
        #endregion

        #region Validations
        private static bool _DefaultEventStartValid(IElement Event, ProcessVariablesContainer variables){return true;}
        private IsEventStartValid _isEventStartValid = new IsEventStartValid(_DefaultEventStartValid);
        public IsEventStartValid IsEventStartValid { get { return _isEventStartValid; } set { _isEventStartValid = value; } }

        private static bool _DefaultProcessStartValid(IElement Event, ProcessVariablesContainer variables){return true;}
        private IsProcessStartValid _isProcessStartValid = new IsProcessStartValid(_DefaultProcessStartValid);
        public IsProcessStartValid IsProcessStartValid { get { return _isProcessStartValid; } set { _isProcessStartValid = value; } }
        #endregion

        #region Conditions
        private static bool _DefaultFlowValid(IElement flow, ProcessVariablesContainer variables) { return true; }
        private IsFlowValid _isFlowValid = new IsFlowValid(_DefaultFlowValid);
        public IsFlowValid IsFlowValid { get { return _isFlowValid; } set { _isFlowValid = value; } }
        #endregion

        #region Tasks
        private ProcessBusinessRuleTask _processBusinessRuleTask;
        public ProcessBusinessRuleTask ProcessBusinessRuleTask { get { return _processBusinessRuleTask; } set { _processBusinessRuleTask = value; } }

        private BeginManualTask _beginManualTask;
        public BeginManualTask BeginManualTask { get { return _beginManualTask; } set { _beginManualTask = value; } }

        private ProcessRecieveTask _processRecieveTask;
        public ProcessRecieveTask ProcessRecieveTask { get { return _processRecieveTask; } set { _processRecieveTask = value; } }

        private ProcessScriptTask _processScriptTask;
        public ProcessScriptTask ProcessScriptTask { get { return _processScriptTask; } set { _processScriptTask=value; } }

        private ProcessSendTask _processSendTask;
        public ProcessSendTask ProcessSendTask { get { return _processSendTask; } set { _processSendTask = value; } }

        private ProcessServiceTask _processServiceTask;
        public ProcessServiceTask ProcessServiceTask { get { return _processServiceTask; } set { _processServiceTask = value; } }

        private ProcessTask _processTask;
        public ProcessTask ProcessTask { get { return _processTask; } set { _processTask = value; } }

        private BeginUserTask _beginUserTask;
        public BeginUserTask BeginUserTask { get { return _beginUserTask; } set { _beginUserTask = value; } }

        #region TaskCallBacks
        private void _CompleteExternalTask(string taskID, ProcessVariablesContainer variables)
        {
            bool success = false;
            foreach (IElement elem in _FullElements)
            {
                if (elem.id == taskID && elem is ATask)
                {
                    _MergeVariables((ATask)elem, variables);
                    success = true;
                    break;
                }
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        private void _CompleteUserTask(string taskID, ProcessVariablesContainer variables,string completedByID)
        {
            bool success = false;
            foreach (IElement elem in _FullElements)
            {
                if (elem.id == taskID && elem is ATask)
                {
                    _MergeVariables((UserTask)elem, variables,completedByID);
                    success = true;
                    break;
                }
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        private void _ErrorExternalTask(string taskID, Exception ex)
        {
            bool success = false;
            foreach (IElement elem in _FullElements)
            {
                if (elem.id == taskID && elem is ATask)
                {
                    if (_onTaskError != null)
                        _onTaskError((ATask)elem);
                    lock (_state)
                    {
                        _state.Path.FailTask((ATask)elem);
                    }
                    break;
                }
            }
            if (!success)
                throw new Exception(string.Format("Unable to locate task with id {0}", taskID));
        }

        public void CompleteUserTask(string taskID, ProcessVariablesContainer variables,string completedByID)
        {
            _CompleteUserTask(taskID, variables,completedByID);
        }

        public void ErrorUserTask(string taskID, Exception ex)
        {
            _ErrorExternalTask(taskID, ex);
        }

        public void CompleteManualTask(string taskID, ProcessVariablesContainer variables)
        {
            _CompleteExternalTask(taskID, variables);
        }

        public void ErrorManualTask(string taskID, Exception ex)
        {
            _ErrorExternalTask(taskID, ex);
        }
        #endregion

        #endregion

        #endregion

        private BusinessProcess() {
            _processLock = new ManualResetEvent(false);
        }

        public BusinessProcess(XmlDocument doc)
        {
            List<Exception> exceptions = new List<Exception>();
            _processLock = new ManualResetEvent(false);
            _doc = doc;
            _state = new ProcessState(new ProcessStepComplete(_ProcessStepComplete),new ProcessStepError(_ProcessStepError));
            _components = new List<object>();
            XmlPrefixMap map = new XmlPrefixMap();
            foreach (XmlNode n in doc.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    map.Load((XmlElement)n);
                    IElement elem = Utility.ConstructElementType((XmlElement)n, map,null);
                    if (elem != null)
                        _components.Add(elem);
                    else
                        _components.Add(n);
                }
                else
                    _components.Add(n);
            }
            if (_Elements.Count == 0)
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
                foreach (IElement elem in _Elements)
                    _ValidateElement((AElement)elem,ref exceptions);
            }
            if (exceptions.Count!=0)
                throw new InvalidProcessDefinitionException(exceptions);
        }

        private void _ValidateElement(AElement elem,ref List<Exception> exceptions)
        {
            foreach (RequiredAttribute ra in Utility.GetCustomAttributesForClass(elem.GetType(),typeof(RequiredAttribute)))
            {
                if (elem.Element.Attributes[ra.Name]==null)
                    exceptions.Add(new MissingAttributeException(elem.Element,ra));
            }
            foreach (AttributeRegex ar in Utility.GetCustomAttributesForClass(elem.GetType(), typeof(AttributeRegex)))
            {
                if (!ar.IsValid(elem))
                    exceptions.Add(new InvalidAttributeValueException(elem.Element, ar));
            }
            string[] err;
            if (!elem.IsValid(out err))
                exceptions.Add(new InvalidElementException(elem.Element, err));
            if (elem.ExtensionElement != null)
                _ValidateElement((ExtensionElements)elem.ExtensionElement, ref exceptions);
            if (elem is AParentElement)
            {
                foreach (AElement e in ((AParentElement)elem).Children)
                    _ValidateElement(e,ref exceptions);
            }
        }

        public bool LoadState(XmlDocument doc)
        {
            if (_state.Load(doc))
            {
                foreach ( sStepSuspension ss in _state.SuspendedSteps)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(_suspendEvent));
                    th.Start((object)(new object[] { ss.id, ss.EndTime }));
                }
                return true;
            }
            return false;
        }

        private void _suspendEvent(object parameters)
        {
            string id = (string)((object[])parameters)[0];
            DateTime release = (DateTime)((object[])parameters)[1];
            TimeSpan ts = release.Subtract(DateTime.Now);
            if (ts.TotalMilliseconds > 0)
                Thread.Sleep(ts);
            foreach(IElement ie in _FullElements)
            {
                if (ie.id == id)
                {
                    AEvent evnt = (AEvent)ie;
                    lock (_state) { _state.Path.SucceedEvent(evnt); }
                    if (_onEventCompleted != null)
                        _onEventCompleted(evnt);
                    break;
                }
            }
        }

        public Bitmap Diagram(bool outputVariables)
        {
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
            Bitmap ret = new Bitmap(width, height);
            Graphics gp = Graphics.FromImage(ret);
            gp.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
            int padding = _DEFAULT_PADDING / 2;
            foreach (IElement elem in _Elements)
            {
                if (elem is Definition)
                {
                    foreach (Diagram d in ((Definition)elem).Diagrams)
                    {
                        gp.DrawImage(d.Render(_state.Path, ((Definition)elem)), new Point(_DEFAULT_PADDING / 2, padding));
                        padding += d.Size.Height + _DEFAULT_PADDING;
                    }
                }
            }
            if (outputVariables)
            {
                SizeF sz = gp.MeasureString("Variables", Constants.FONT);
                int varHeight = (int)sz.Height+2;
                string[] keys = _state[null];
                foreach (string str in keys)
                    varHeight += (int)gp.MeasureString(str, Constants.FONT).Height + 2;
                Bitmap vmap = new Bitmap(_VARIABLE_IMAGE_WIDTH, varHeight);
                gp = Graphics.FromImage(vmap);
                gp.FillRectangle(Brushes.White, new Rectangle(0, 0, vmap.Width, vmap.Height));
                Pen p = new Pen(Brushes.Black, Constants.PEN_WIDTH);
                gp.DrawRectangle(p, new Rectangle(0, 0, vmap.Width, vmap.Height));
                gp.DrawLine(p, new Point(0, (int)sz.Height + 2), new Point(_VARIABLE_IMAGE_WIDTH, (int)sz.Height + 2));
                gp.DrawLine(p,new Point(_VARIABLE_NAME_WIDTH,(int)sz.Height + 2),new Point(_VARIABLE_NAME_WIDTH,vmap.Height));
                gp.DrawString("Variables", Constants.FONT, Brushes.Black, new PointF((vmap.Width - sz.Width) / 2, 2));
                int curY = (int)sz.Height+2;
                for (int x = 0; x < keys.Length; x++)
                {
                    string label = keys[x];
                    SizeF szLabel = gp.MeasureString(keys[x], Constants.FONT);
                    while (szLabel.Width > _VARIABLE_NAME_WIDTH)
                    {
                        if (label.EndsWith("..."))
                            label = label.Substring(0, label.Length - 4) + "...";
                        else
                            label = label.Substring(0, label.Length - 1) + "...";
                        szLabel = gp.MeasureString(label, Constants.FONT);
                    }
                    string val = (_state[null, keys[x]] == null ? "" : _state[null, keys[x]].ToString());
                    SizeF szValue = gp.MeasureString(val, Constants.FONT);
                    if (szValue.Width > _VARIABLE_VALUE_WIDTH)
                    {
                        if (val.EndsWith("..."))
                            val = val.Substring(0, val.Length - 4) + "...";
                        else
                            val = val.Substring(0, val.Length - 1) + "...";
                        szValue = gp.MeasureString(val, Constants.FONT);
                    }
                    gp.DrawString(label, Constants.FONT, Brushes.Black, new Point(2, curY));
                    gp.DrawString(val, Constants.FONT, Brushes.Black, new Point(2 + _VARIABLE_NAME_WIDTH, curY));
                    curY += (int)Math.Max(szLabel.Height, szValue.Height) + 2;
                    gp.DrawLine(p, new Point(0, curY), new Point(_VARIABLE_IMAGE_WIDTH, curY));
                }
                gp.Flush();
                Bitmap tret = new Bitmap(ret.Width + _DEFAULT_PADDING + vmap.Width, Math.Max(ret.Height, vmap.Height + _DEFAULT_PADDING));
                gp = Graphics.FromImage(tret);
                gp.FillRectangle(Brushes.White, new Rectangle(0, 0, tret.Width, tret.Height));
                gp.DrawImage(ret, new Point(0, 0));
                gp.DrawImage(vmap, new Point(ret.Width + _DEFAULT_PADDING, _DEFAULT_PADDING));
                gp.Flush();
                ret = tret;
            }
            return ret;
        }

        public byte[] Animate(bool outputVariables)
        {
            MemoryStream ms = new MemoryStream();
            Drawing.Gif.Encoder enc = new Drawing.Gif.Encoder();
            enc.Start(ms);
            enc.SetDelay(_ANIMATION_DELAY);
            enc.SetRepeat(0);
            _state.Path.StartAnimation();
            while (_state.Path.HasNext())
            {
                enc.AddFrame(Diagram(outputVariables));
                _state.Path.MoveToNextStep();
            }
            _state.Path.FinishAnimation();
            enc.Finish();
            return ms.ToArray();
        }

        public BusinessProcess Clone(bool includeState,bool includeDelegates)
        {
            BusinessProcess ret = new BusinessProcess();
            ret._doc = _doc;
            ret._components = new List<object>(_components.ToArray());
            if (includeState)
                ret._state = _state;
            else
                ret._state = new ProcessState(new ProcessStepComplete(ret._ProcessStepComplete), new ProcessStepError(ret._ProcessStepError));
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
                ret.OnSequenceFlowCompleted = OnSequenceFlowCompleted;
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
            }
            return ret;
        }

        public bool BeginProcess(ProcessVariablesContainer variables)
        {
            bool ret = false;
            foreach (IElement elem in _FullElements)
            {
                if (elem is Process)
                {
                    if (((Process)elem).IsProcessStartvalid(variables, _isProcessStartValid))
                    {
                        Process p = (Process)elem;
                        foreach (StartEvent se in p.StartEvents)
                        {
                            if (se.IsEventStartValid(variables, _isEventStartValid))
                            {
                                if (_onProcessStarted != null)
                                    _onProcessStarted(p);
                                if (_onEventStarted!=null)
                                    _onEventStarted(se);
                                _state.Path.StartEvent(se, null);
                                foreach (string str in variables.Keys)
                                    _state[se.id,str]=variables[str];
                                _state.Path.SucceedEvent(se);
                                if (_onEventCompleted!=null)
                                    _onEventCompleted(se);
                                ret=true;
                            }
                        }
                    }
                }
                if (ret)
                    break;
            }
            return ret;
        }

        #region ProcessLock

        public bool WaitForCompletion()
        {
            return _processLock.WaitOne();
        }

        public bool WaitForCompletion(int millisecondsTimeout)
        {
            return _processLock.WaitOne(millisecondsTimeout);
        }

        public bool WaitForCompletion(TimeSpan timeout)
        {
            return _processLock.WaitOne(timeout);
        }

        public bool WaitForCompletion(int millisecondsTimeout,bool exitContext)
        {
            return _processLock.WaitOne(millisecondsTimeout,exitContext);
        }

        public bool WaitForCompletion(TimeSpan timeout,bool exitContext)
        {
            return _processLock.WaitOne(timeout,exitContext);
        }

        #endregion

        private void _ProcessStepComplete(string sourceID,string outgoingID) {
            if (outgoingID != null)
            {
                foreach (IElement elem in _FullElements)
                {
                    if (elem.id == outgoingID)
                        _ProcessElement(sourceID,elem);
                }
            }
        }

        private void _ProcessStepError(IElement step) {
            if (_isEventStartValid != null)
            {
                Definition def = null;
                foreach (IElement elem in _Elements)
                {
                    if (elem is Definition)
                    {
                        if (((Definition)elem).LocateElement(step.id) != null)
                        {
                            def = (Definition)elem;
                            break;
                        }
                    }
                }
                if (def != null)
                {
                    foreach (IElement elem in _FullElements)
                    {
                        if (elem is IntermediateCatchEvent)
                        {
                            if (_isEventStartValid(elem, new ProcessVariablesContainer(step.id, _state)))
                            {
                                _ProcessElement(step.id, elem);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void _ProcessElement(string sourceID,IElement elem)
        {
            if (elem is SequenceFlow)
            {
                SequenceFlow sf = (SequenceFlow)elem;
                lock (_state)
                {
                    _state.Path.ProcessSequenceFlow(sf);
                }
                if (_onSequenceFlowCompleted != null)
                    _onSequenceFlowCompleted(sf);
            }else if (elem is MessageFlow)
            {
                MessageFlow mf = (MessageFlow)elem;
                lock (_state)
                {
                    _state.Path.ProcessMessageFlow(mf);
                }
                if (_onMessageFlowCompleted != null)
                    _onMessageFlowCompleted(mf);
            }
            else if (elem is AGateway)
            {
                AGateway gw = (AGateway)elem;
                Definition def = null;
                foreach (IElement e in _Elements){
                    if (e is Definition){
                        if (((Definition)e).LocateElement(gw.id)!=null){
                            def = (Definition)e;
                            break;
                        }
                    }
                }
                lock (_state)
                {
                    _state.Path.StartGateway(gw, sourceID);
                }
                if (_onGatewayStarted != null)
                    _onGatewayStarted(gw);
                string[] outgoings = null;
                try
                {
                    outgoings = gw.EvaulateOutgoingPaths(def, _isFlowValid, new ProcessVariablesContainer(elem.id, _state));
                }
                catch (Exception e)
                {
                    if (_onGatewayError != null)
                        _onGatewayError(gw);
                    outgoings = null;
                }
                lock (_state)
                {
                    if (outgoings == null)
                        _state.Path.FailGateway(gw);
                    else
                        _state.Path.SuccessGateway(gw, outgoings);
                }
            }
            else if (elem is AEvent)
            {
                AEvent evnt = (AEvent)elem;
                if (_onEventStarted != null)
                    _onEventStarted(evnt);
                lock (_state)
                {
                    _state.Path.StartEvent(evnt, sourceID);
                }
                bool success = true;
                if (_isEventStartValid != null  && (evnt is IntermediateCatchEvent || evnt is StartEvent))
                {
                    try
                    {
                        success = _isEventStartValid(evnt, new ProcessVariablesContainer(evnt.id, _state));
                    }
                    catch (Exception e)
                    {
                        success = false;
                    }
                }else if (evnt is IntermediateThrowEvent)
                {
                    TimeSpan? ts = evnt.GetTimeout(new ProcessVariablesContainer(evnt.id, _state));
                    if (ts.HasValue)
                    {
                        lock (_state)
                        {
                            _state.SuspendStep(evnt.id, ts.Value);
                        }
                        if (ts.Value.TotalMilliseconds > 0)
                            Thread.Sleep(ts.Value);
                        success = true;
                    }
                }
                if (!success){
                    lock (_state) { _state.Path.FailEvent(evnt); }
                    if (_onEventError != null)
                        _onEventError(evnt);
                } else{
                    lock (_state) { _state.Path.SucceedEvent(evnt); }
                    if (_onEventCompleted != null)
                        _onEventCompleted(evnt);
                    if (evnt is EndEvent)
                    {
                        if (_onProcessCompleted != null)
                            _onProcessCompleted(((EndEvent)evnt).Process);
                        _processLock.Set();
                    }
                }
            }
            else if (elem is ATask)
            {
                ATask tsk = (ATask)elem;
                if (_onTaskStarted != null)
                    _onTaskStarted(tsk);
                lock (_state)
                {
                    _state.Path.StartTask(tsk, sourceID);
                }
                try
                {
                    ProcessVariablesContainer variables = new ProcessVariablesContainer(tsk.id,_state);
                    switch (elem.GetType().Name)
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
                            Lane ln = null;
                            foreach (IElement e in _FullElements){
                                if (e is Lane){
                                    if (new List<string>(((Lane)e).Nodes).Contains(tsk.id)){
                                        ln = (Lane)e;
                                        break;
                                    }
                                }
                            }
                            _beginUserTask(tsk, variables, ln, new CompleteUserTask(_CompleteUserTask), new ErrorUserTask(_ErrorExternalTask));
                            break;
                    }
                }
                catch (Exception e)
                {
                    if (_onTaskError != null)
                        _onTaskError(tsk);
                    lock (_state) { _state.Path.FailTask(tsk); }
                }
            }
        }

        private void _MergeVariables(UserTask task, ProcessVariablesContainer variables, string completedByID)
        {
            _MergeVariables(task, variables, completedByID);
        }

        private void _MergeVariables(ATask task, ProcessVariablesContainer variables)
        {
            _MergeVariables(task, variables, null);
        }

        private void _MergeVariables(ATask task, ProcessVariablesContainer variables,string completedByID)
        {
            lock (_state)
            {
                foreach (string str in variables.Keys)
                {
                    if (variables[str] == null && _state[task.id, str] != null)
                        _state[task.id, str] = null;
                    else if (_state[task.id, str] == null && variables[str] != null)
                        _state[task.id, str] = variables[str];
                    else if (_state[task.id, str] != null && variables[str] != null)
                    {
                        try
                        {
                            if (!variables[str].Equals(_state[task.id, str]))
                                _state[task.id, str] = variables[str];
                        }
                        catch (Exception e)
                        {
                            _state[task.id, str] = variables[str];
                        }
                    }
                }
                if (_onTaskCompleted != null)
                    _onTaskCompleted(task);
                if (task is UserTask)
                    _state.Path.SucceedTask((UserTask)task,completedByID);
                else
                    _state.Path.SucceedTask(task);
            }
        }
    }
}
