﻿using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using BPMNEngine.Attributes;
using BPMNEngine.DelegateContainers;
using BPMNEngine.Drawing;
using BPMNEngine.Elements;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces;
using BPMNEngine.State;
using SkiaSharp;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Scheduling;

namespace BPMNEngine
{
    /// <summary>
    /// This class is the primary class for the library.  It implements a Business Process by constructing the object using a BPMN 2.0 compliant definition.
    /// This is followed by assigning delegates for handling the specific process events and then starting the process.  A process can also be suspended and 
    /// the suspended state loaded and resumed.  It can also be cloned, including the current state and delegates in order to have more than once instance 
    /// of the given process executing.
    /// </summary>
    public sealed class BusinessProcess : IDisposable
    {
        private static readonly TimeSpan _ANIMATION_DELAY = new(0,0,1);
        private const float _DEFAULT_PADDING = 100;
        private const int _VARIABLE_NAME_WIDTH = 200;
        private const int _VARIABLE_VALUE_WIDTH = 300;
        private const int _VARIABLE_IMAGE_WIDTH = _VARIABLE_NAME_WIDTH+_VARIABLE_VALUE_WIDTH;

        private readonly Guid _id;
        private readonly List<object> _components;
        private readonly IEnumerable<AHandlingEvent> _eventHandlers = null;
        private readonly Definition definition;

        internal IElement GetElement(string id) => Elements.FirstOrDefault(elem=>elem.ID==id);
        private IEnumerable<IElement> Elements => _components
            .OfType<IElement>()
            .Traverse(elem => (elem is IParentElement element ? element.Children : Array.Empty<IElement>()));

        /// <summary>
        /// The XML Document that was supplied to the constructor containing the BPMN 2.0 definition
        /// </summary>
        public XmlDocument Document { get; private init; }

        private readonly IEnumerable<SProcessRuntimeConstant> _constants;
        /// <summary>
        /// This is used to access the values of the process runtime and definition constants
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable</returns>
        public object this[string name]
        {
            get
            {
                if (_constants != null && _constants.Any(c => c.Name==name))
                    return _constants.FirstOrDefault(c => c.Name==name).Value;
                if (definition==null || definition.ExtensionElement==null)
                    return null;
                var definitionVariable = ((ExtensionElements)definition.ExtensionElement).Children
                    .FirstOrDefault(elem =>
                    (elem is DefinitionVariable variable && variable.Name==name) ||
                    (elem is DefinitionFile file &&
                        (string.Format("{0}.{1}", file.Name, file.Extension)==name
                        || file.Name==name)
                    ));
                if (definitionVariable!=null)
                    return (definitionVariable is DefinitionVariable variable ? variable.Value
                        : new SFile((DefinitionFile)definitionVariable)
                    );
                return null;
            }
        }

        
        internal IEnumerable<string> Keys
        {
            get
            {
                if (definition==null || definition.ExtensionElement==null)
                    return _constants==null ? Array.Empty<string>() : _constants.Select(c => c.Name);
                return (_constants==null ? Array.Empty<string>() : _constants.Select(c => c.Name))
                    .Concat(
                        ((ExtensionElements)definition.ExtensionElement)
                        .Children
                        .OfType<DefinitionVariable>()
                        .Select(d => d.Name)
                    )
                    .Concat(
                        ((ExtensionElements)definition.ExtensionElement)
                        .Children
                        .OfType<DefinitionFile>()
                        .Select(d => d.Name)
                    )
                    .Concat(
                        ((ExtensionElements)definition.ExtensionElement)
                        .Children
                        .OfType<DefinitionFile>()
                        .Select(d => string.Format("{0}.{1}", d.Name,d.Extension))
                    )
                    .Distinct();
            }
        }

        private readonly DelegateContainer _delegates;

        internal ATask GetTask(string taskID)
        {
            IElement elem = GetElement(taskID);
            if (elem is ATask task)
                return task;
            return null;
        }

        internal void HandleTaskEmission(ProcessInstance instance, ITask task, object data, EventSubTypes type,out bool isAborted)
        {
            var events = GetEventHandlers(type, data, (AFlowNode)GetElement(task.ID), new ReadOnlyProcessVariablesContainer(task.Variables));
            events.ForEach(ahe => ProcessEvent(instance, task.ID, ahe));
            isAborted = instance.State.Path.GetStatus(task.ID)==StepStatuses.Aborted;
        }

        /// <summary>
        /// A Utility call used to extract the variable values from a Business Process State Document.
        /// </summary>
        /// <param name="doc">The State XML Document file to extract the values from</param>
        /// <returns>The variables extracted from the Process State Document</returns>
        public static Dictionary<string,object> ExtractProcessVariablesFromStateDocument(XmlDocument doc) { return ProcessVariables.ExtractVariables(doc); }

        /// <summary>
        /// Creates a new instance of the BusinessProcess passing it the definition, StateLogLevel, runtime constants and LogLine delegate
        /// </summary>
        /// <param name="doc">The Xml Document containing the BPMN 2.0 definition</param>
        /// <param name="constants">An array of runtime constants that are set for this particular instance of the process</param>
        /// <param name="events">The Process Events delegates container</param>
        /// <param name="validations">The Process Validations delegates container</param>
        /// <param name="tasks">The Process Tasks delegates container</param>
        /// <param name="logging">The Process Logging delegates container</param>
        public BusinessProcess(XmlDocument doc,
             IEnumerable<SProcessRuntimeConstant> constants = null,
            ProcessEvents events = null,
            StepValidations validations=null,
            ProcessTasks tasks=null,
            ProcessLogging logging=null
            )
        {
            _id = Guid.NewGuid();
            _constants = constants;
            _delegates = new DelegateContainer()
            {
                Events=ProcessEvents.Merge(null,events),
                Validations=StepValidations.Merge(null,validations),
                Tasks=ProcessTasks.Merge(null,tasks),
                Logging=ProcessLogging.Merge(null,logging)
            };


            IEnumerable<Exception> exceptions = Array.Empty<Exception>();
            Document = new XmlDocument();
            Document.LoadXml(doc.OuterXml);
            var elementMapCache = new BPMNEngine.ElementTypeCache();
            DateTime start = DateTime.Now;
            WriteLogLine((IElement)null,LogLevel.Information,new StackFrame(1,true),DateTime.Now,"Producing new Business Process from XML Document");
            _components = new List<object>();
            XmlPrefixMap map = new(this);
            _=doc.ChildNodes.Cast<XmlNode>().ForEach(n =>
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (map.Load((XmlElement)n))
                        elementMapCache.MapIdeals(map);
                    IElement elem = Utility.ConstructElementType((XmlElement)n, ref map, ref elementMapCache, null);
                    if (elem != null)
                    {
                        if (elem is Definition definition)
                            definition.OwningProcess = this;
                        if (elem is AParentElement element)
                            element.LoadChildren(ref map, ref elementMapCache);
                        ((AElement)elem).LoadExtensionElement(ref map, ref elementMapCache);
                        _components.Add(elem);
                    }
                    else
                        _components.Add(n);
                }
                else
                    _components.Add(n);
            });
            definition = _components.OfType<Definition>().FirstOrDefault();
            if (!Elements.Any())
                exceptions = exceptions.Append(new XmlException("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
            else
            {
                if (definition==null)
                    exceptions = exceptions.Append(new XmlException("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
            }
            if (!exceptions.Any())
                Elements.ForEach(elem => { exceptions = exceptions.Concat(ValidateElement((AElement)elem)); });
            if (exceptions.Any())
            {
                Exception ex = new InvalidProcessDefinitionException(exceptions);
                WriteLogException((IElement)null,new StackFrame(1, true), DateTime.Now, ex);
                throw ex;
            }
            _eventHandlers = Elements
                .OfType<AHandlingEvent>();
            WriteLogLine((IElement)null,LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Time to load Process Document {0}ms",DateTime.Now.Subtract(start).TotalMilliseconds));
        }

        private IEnumerable<Exception> ValidateElement(AElement elem)
        {
            WriteLogLine(elem,LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Validating element {0}", new object[] { elem.ID }));
            IEnumerable<Exception> result = Array.Empty<Exception>();
            result = result.Concat(
                elem.GetType().GetCustomAttributes(true).OfType<RequiredAttribute>()
                .Where(ra => elem[ra.Name]==null)
                .Select(ra=> new MissingAttributeException(elem.Definition, elem.Element, ra))
            );
            result = result.Concat(
                elem.GetType().GetCustomAttributes(true).OfType<AttributeRegex>()
                .Where(ar => !ar.IsValid(elem))
                .Select(ar => new InvalidAttributeValueException(elem.Definition, elem.Element, ar))
            );
            if (!elem.IsValid(out string[] err))
                result = result.Append(new InvalidElementException(elem.Definition, elem.Element, err));
            if (elem.ExtensionElement != null)
                result = result.Concat(ValidateElement((ExtensionElements)elem.ExtensionElement));
            if (elem is AParentElement element)
                result = result.Concat(
                    element.Children
                    .OfType<AElement>()
                    .Select(e=>ValidateElement(e))
                    .SelectMany(res=>res)
                );
            return result;
        }

        /// <summary>
        /// Called to load a Process Instance from a stored State Document
        /// </summary>
        /// <param name="doc">The process state document</param>
        /// <param name="autoResume">set true if the process was suspended and needs to resume once loaded</param>
        /// <param name="events">The Process Events delegates container</param>
        /// <param name="validations">The Process Validations delegates container</param>
        /// <param name="tasks">The Process Tasks delegates container</param>
        /// <param name="logging">The Process Logging delegates container</param>
        /// <param name="stateLogLevel">Used to set the logging level for the process state document</param>
        /// <returns>an instance of IProcessInstance if successful or null it failed</returns>
        public IProcessInstance LoadState(XmlDocument doc,
            bool autoResume=false,
            ProcessEvents events = null,
            StepValidations validations = null,
            ProcessTasks tasks = null,
            ProcessLogging logging = null,
            LogLevel stateLogLevel=LogLevel.None)
        {
            ProcessInstance ret = new(this, DelegateContainer.Merge(_delegates,new DelegateContainer()
            {
                Events = events,
                Validations = validations,
                Tasks = tasks,
                Logging = logging
            }), stateLogLevel);
            if (ret.LoadState(doc, autoResume))
                return ret;
            return null;
        }

        /// <summary>
        /// Called to render a PNG image of the process
        /// </summary>
        /// <param name="type">The output image format to generate, this being jpeg,png or bmp</param>
        /// <returns>A Bitmap containing a rendered image of the process</returns>
        public byte[] Diagram(ImageFormat type)
        {
            var tmp = Diagram(false);
            return tmp?.AsBytes(type);
        }

        internal byte[] Diagram(bool outputVariables,ProcessState state, ImageFormat type)
        {
            var tmp = Diagram(outputVariables, state:state);
            return tmp?.AsBytes(type);
        }

        private IImage Diagram(bool outputVariables, ProcessState state =null)
        {
            if (definition==null)
                throw new DiagramException("Process definition is null or missing");
            if (!definition.Diagrams.Any())
                throw new DiagramException("Process definition does not contain any Diagrams");
            state??=new ProcessState(this, null, null, null);
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Diagram{0}", new object[] { (outputVariables ? " with variables" : " without variables") }));
            double width = 0;
            double height = 0;
            width = definition.Diagrams.Max(d => d.Size.Width+_DEFAULT_PADDING);
            height = definition.Diagrams.Sum(d => d.Size.Height+_DEFAULT_PADDING);
            IImage ret = null;
            try
            {
                var image = BPMNEngine.Elements.Diagram.ProduceImage((int)Math.Ceiling(width),(int)Math.Ceiling(height));
                var surface = image.Canvas;
                surface.FillColor=Colors.White;
                surface.FillRectangle(new Rect(0, 0, width, height));
                float padding = _DEFAULT_PADDING / 2;
                definition.Diagrams.ForEach(d => { 
                    surface.DrawImage(d.Render(state.Path, this.definition), _DEFAULT_PADDING / 2, padding, d.Size.Width, d.Size.Height); 
                    padding += d.Size.Height + _DEFAULT_PADDING; 
                });
                ret = image.Image;
                if (outputVariables)
                    ret = BusinessProcess.AppendVariables(ret, state);
            }catch(Exception e)
            {
                WriteLogException((IElement)null, new StackFrame(1, true), DateTime.Now, e);
                ret=null;
            }
            return ret;
        }

        private static IImage ProduceVariablesImage(ProcessState state)
        {
            var image = BPMNEngine.Elements.Diagram.ProduceImage(1,1);
            var canvas = image.Canvas;
            SizeF sz = canvas.GetStringSize("Variables", BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
            int varHeight = (int)sz.Height + 2;
            var keys = state[null];
            varHeight+=keys.Sum(key => (int)canvas.GetStringSize(key, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE).Height + 2);

            image = BPMNEngine.Elements.Diagram.ProduceImage(_VARIABLE_IMAGE_WIDTH, varHeight);
            var surface = image.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, image.Width, image.Height);

            surface.StrokeColor = Colors.Black;
            surface.StrokeDashPattern=null;
            surface.StrokeSize=1.0f;

            surface.DrawRectangle(0, 0, image.Width, image.Height);

            surface.DrawLine(new Point(0, (int)sz.Height + 2), new Point(_VARIABLE_IMAGE_WIDTH, (int)sz.Height + 2));
            surface.DrawLine(new Point(_VARIABLE_NAME_WIDTH, (int)sz.Height + 2), new Point(_VARIABLE_NAME_WIDTH, image.Height));
            surface.DrawString("Variables", new Rect(0, 2, image.Width, sz.Height), HorizontalAlignment.Center, VerticalAlignment.Center);
            float curY = sz.Height + 2;
            keys.ForEach(key =>
            {
                string label = key;
                SizeF szLabel = canvas.GetStringSize(label, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                while (szLabel.Width > _VARIABLE_NAME_WIDTH)
                {
                    if (label.EndsWith("..."))
                        label = string.Concat(label.AsSpan(0, label.Length - 4), "...");
                    else
                        label = string.Concat(label.AsSpan(0, label.Length - 1), "...");
                    szLabel = canvas.GetStringSize(label, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                }
                StringBuilder val = new();
                if (state[null, key] != null)
                {
                    if (state[null, key].GetType().IsArray)
                    {
                        ((IEnumerable)state[null, key]).Cast<object>().ForEach(o => val.AppendFormat("{0},", o));
                        val.Length--;
                    }
                    else if (state[null, key] is Hashtable hashtable)
                    {
                        val.Append('{');
                        hashtable.Keys.Cast<string>().ForEach(k => val.Append($"{{\"{k}\":\"{hashtable[k]}\"}},"));
                        val.Length--;
                        val.Append('}');
                    }
                    else
                        val.Append(state[null, key].ToString());
                }
                var sval = val.ToString();
                Size szValue = canvas.GetStringSize(sval, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                if (szValue.Width > _VARIABLE_VALUE_WIDTH)
                {
                    if (sval.EndsWith("..."))
                        sval = string.Concat(sval.AsSpan(0, sval.Length - 4), "...");
                    else
                        sval = string.Concat(sval.AsSpan(0, sval.Length - 1), "...");
                    canvas.GetStringSize(sval, BPMNEngine.Elements.Diagram.DefaultFont, BPMNEngine.Elements.Diagram.FONT_SIZE);
                }
                surface.DrawString(label, 2, curY, HorizontalAlignment.Left);
                surface.DrawString(sval, 2+_VARIABLE_NAME_WIDTH, curY, HorizontalAlignment.Left);
                curY += (float)Math.Max(szLabel.Height, szValue.Height) + 2;
                surface.DrawLine(new Point(0, curY), new Point(_VARIABLE_IMAGE_WIDTH, curY));
            });
            return image.Image;
        }

        private static IImage AppendVariables(IImage diagram,ProcessState state)
        {
            var vmap = BusinessProcess.ProduceVariablesImage(state);
            var ret = BPMNEngine.Elements.Diagram.ProduceImage(
                (int)Math.Ceiling(diagram.Width + _DEFAULT_PADDING + vmap.Width), 
                (int)Math.Ceiling(Math.Max(diagram.Height, vmap.Height + _DEFAULT_PADDING))
            );
            var surface = ret.Canvas;
            surface.FillColor = Colors.White;
            surface.FillRectangle(0, 0, ret.Width, ret.Height);
            surface.DrawImage(diagram, 0, 0, diagram.Width, diagram.Height);
            surface.DrawImage(vmap, diagram.Width + _DEFAULT_PADDING, _DEFAULT_PADDING,vmap.Width,vmap.Height);
            return ret.Image;
        }

        internal byte[] Animate(bool outputVariables, ProcessState state)
        {
            WriteLogLine((IElement)null, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Rendering Business Process Animation{0}", new object[] { (outputVariables ? " with variables" : " without variables") }));
            var result = Array.Empty<byte>();
            try
            {
                state.Path.StartAnimation();
                IImage bd = Diagram(false, state);
#pragma warning disable IDE0270 // Use coalesce expression
                if (bd==null)
                    throw new DiagramException("Unable to create first diagram frame");
#pragma warning restore IDE0270 // Use coalesce expression
                AnimatedPNG apng = new((outputVariables ? BusinessProcess.AppendVariables(bd, state) : bd))
                {
                    DefaultFrameDelay= _ANIMATION_DELAY
                };
                while (state.Path.HasNext())
                {
                    string nxtStep = state.Path.MoveToNextStep();
                    if (nxtStep != null)
                    {
                        double padding = _DEFAULT_PADDING / 2;
                        if (definition!=null)
                        {
                            var diagram = definition.Diagrams.FirstOrDefault(d => d.RendersElement(nxtStep));
                            if (diagram!=null)
                            {
                                var rect = diagram.GetElementRectangle(nxtStep);
                                IImage img = diagram.Render(state.Path, definition, nxtStep);
                                apng.AddFrame(img, (int)Math.Ceiling((_DEFAULT_PADDING / 2)+rect.X)+3, (int)Math.Ceiling(padding+rect.Y)+3);
                            }
                        }
                        if (outputVariables)
                            apng.AddFrame(BusinessProcess.ProduceVariablesImage(state), (int)Math.Ceiling(bd.Width + _DEFAULT_PADDING), (int)_DEFAULT_PADDING,delay:new TimeSpan(0,0,0));
                    }
                }
                state.Path.FinishAnimation();
                result = apng.ToBinary();
                apng.Dispose();
            }
            catch (Exception e)
            {
                WriteLogException((IElement)null, new StackFrame(1, true), DateTime.Now, e);
                result=null;
            }
            return result;
        }

        /// <summary>
        /// Called to start and instance of the defined BusinessProcess
        /// </summary>
        /// <param name="pars">The variables to start the process with</param>
        /// <param name="events">The Process Events delegates container</param>
        /// <param name="validations">The Process Validations delegates container</param>
        /// <param name="tasks">The Process Tasks delegates container</param>
        /// <param name="logging">The Process Logging delegates container</param>
        /// <param name="stateLogLevel">Used to set the logging level for the process state document</param>
        /// <returns>a process instance if the process was successfully started</returns>
        public IProcessInstance BeginProcess(
            Dictionary<string,object> pars,
            ProcessEvents events = null,
            StepValidations validations = null,
            ProcessTasks tasks = null,
            ProcessLogging logging = null,
            LogLevel stateLogLevel = LogLevel.None)
        {
            ProcessInstance ret = new(this, DelegateContainer.Merge(_delegates, new DelegateContainer()
            {
                Events = events,
                Validations = validations,
                Tasks = tasks,
                Logging = logging
            }), stateLogLevel);
            ProcessVariablesContainer variables = new(pars,this,ret);
            ret.WriteLogLine((IElement)null,LogLevel.Debug, new StackFrame(1, true), DateTime.Now, "Attempting to begin process");
            ReadOnlyProcessVariablesContainer ropvc = new(variables);
            var proc = Elements.OfType<Elements.Process>().FirstOrDefault(p => p.IsStartValid(ropvc, ret.Delegates.Validations.IsProcessStartValid));
            if (proc != null)
            {
                var start = proc.StartEvents.FirstOrDefault(se => se.IsEventStartValid(ropvc, ret.Delegates.Validations.IsEventStartValid));
                if (start!=null)
                {
                    ret.WriteLogLine(start, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Valid Process Start[{0}] located, beginning process", start.ID));
                    BusinessProcess.TriggerDelegateAsync(ret.Delegates.Events.Processes.Started, new object[] { proc, new ReadOnlyProcessVariablesContainer(variables) });
                    BusinessProcess.TriggerDelegateAsync(ret.Delegates.Events.Events.Started, new object[] { start, new ReadOnlyProcessVariablesContainer(variables) });
                    ret.State.Path.StartFlowNode(start, null);
                    variables.Keys.ForEach(key => ret.State[start.ID, key] = variables[key]);
                    ret.State.Path.SucceedFlowNode(start);
                    BusinessProcess.TriggerDelegateAsync(ret.Delegates.Events.Events.Completed, new object[] { start, new ReadOnlyProcessVariablesContainer(start.ID, ret) });
                    return ret;
                }
            }
            WriteLogLine((IElement)null,LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Unable to begin process, no valid start located");
            return null;
        }

        private static void TriggerDelegateAsync(Delegate dgate,object[] pars)
        {
            if (dgate!=null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    dgate.DynamicInvoke(pars);
                });
            }
        }

        private IEnumerable<AHandlingEvent> GetEventHandlers(EventSubTypes type,object data, AFlowNode source, IReadonlyVariables variables)
        {
            var handlerGroup = _eventHandlers
                .GroupBy(handler => handler.EventCost(type, data, source, variables))
                .OrderBy(grp => grp.Key)
                .FirstOrDefault();

            switch (type)
            {
                case EventSubTypes.Conditional:
                case EventSubTypes.Timer:
                    if (handlerGroup!=null && handlerGroup.Key>0)
                        return Array.Empty<AHandlingEvent>();
                    break;
            }

            return (handlerGroup==null || handlerGroup.Key==int.MaxValue ? Array.Empty<AHandlingEvent>() : handlerGroup.ToList());
        }

        internal void ProcessStepComplete(ProcessInstance instance,string sourceID,string outgoingID) {
            if (sourceID!=null)
            {
                IElement elem = GetElement(sourceID);
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer vars = new(sourceID,instance);
                    GetEventHandlers(EventSubTypes.Timer, null, node, vars).ForEach(ahe =>
                    {
                        if (instance.State.Path.GetStatus(ahe.ID)==StepStatuses.WaitingStart)
                        {
                            StepScheduler.Instance.AbortDelayedEvent(instance, (BoundaryEvent)ahe, sourceID);
                            AbortStep(instance, sourceID, ahe, vars);
                        }
                    });
                }
            }
            WriteLogLine(sourceID,LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Process Step[{0}] has been completed", sourceID));
            if (outgoingID != null)
            {
                IElement elem = GetElement(outgoingID);
                if (elem != null)
                    ProcessElement(instance,sourceID, elem);
            }
        }

        internal void ProcessStepError(ProcessInstance instance,IElement step, Exception ex) {
            instance.WriteLogLine(step,LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Process Step Error occured, checking for valid Intermediate Catch Event");
            bool success = false;
            if (step is AFlowNode node)
            {
                var events = GetEventHandlers(EventSubTypes.Error, ex, node, new ReadOnlyProcessVariablesContainer(step.ID,instance,ex));
                if (events.Any())
                {
                    success=true;
                    events.ForEach(ahe =>
                    {
                        instance.WriteLogLine(step, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Valid Error handle located at {0}", ahe.ID));
                        ProcessElement(instance, step.ID, ahe);
                    });
                }
            }
            if (!success)
            {
                if (((IStepElement)step).SubProcess!=null)
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.SubProcesses.Error, new object[] { (IStepElement)((IStepElement)step).SubProcess, new ReadOnlyProcessVariablesContainer(step.ID, instance, ex) });
                else
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Processes.Error, new object[] { ((IStepElement)step).Process, step, new ReadOnlyProcessVariablesContainer(step.ID, instance, ex) });
            }
        }

        private void ProcessElement(ProcessInstance instance,string sourceID,IElement elem)
        {
            if (instance.IsSuspended)
            {
                instance.State.Path.SuspendElement(sourceID, elem);
                instance.MreSuspend.Set();
            }
            else
            {
                instance.WriteLogLine(sourceID,LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Processing Element {0} from source {1}", new object[] { elem.ID, sourceID }));
                bool abort = false;
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer ropvc = new(sourceID, instance);
                    var evnts = GetEventHandlers(EventSubTypes.Conditional, null, node, ropvc);
                    evnts.ForEach(ahe =>
                    {
                        ProcessEvent(instance, elem.ID, ahe);
                        abort|=(ahe is BoundaryEvent @event &&@event.CancelActivity);
                    });
                    if (!abort)
                    {
                        GetEventHandlers(EventSubTypes.Timer, null, node, ropvc).ForEach(ahe =>
                        {
                            TimeSpan? ts = ahe.GetTimeout(ropvc);
                            if (ts.HasValue)
                            {
                                instance.State.Path.DelayEventStart(ahe, elem.ID, ts.Value);
                                StepScheduler.Instance.DelayStart(ts.Value, instance, (BoundaryEvent)ahe, elem.ID);
                            }
                        });
                    }
                }
                if (elem is IFlowElement flowElement)
                    BusinessProcess.ProcessFlowElement(instance, flowElement);
                else if (elem is AGateway aGateway)
                    ProcessGateway(instance, sourceID, aGateway);
                else if (elem is AEvent aEvent)
                    ProcessEvent(instance, sourceID, aEvent);
                else if (elem is ATask aTask)
                    BusinessProcess.ProcessTask(instance, sourceID, aTask);
                else if (elem is SubProcess subProcess)
                    BusinessProcess.ProcessSubProcess(instance, sourceID, subProcess);
            }
        }

        private static void ProcessSubProcess(ProcessInstance instance,string sourceID, SubProcess esp)
        {
            ReadOnlyProcessVariablesContainer variables = new(new ProcessVariablesContainer(esp.ID, instance));
            if (esp.IsStartValid(variables, instance.Delegates.Validations.IsProcessStartValid))
            {
                var startEvent = esp.StartEvents.FirstOrDefault(se => se.IsEventStartValid(variables, instance.Delegates.Validations.IsEventStartValid));
                if (startEvent!=null)
                {
                    instance.WriteLogLine(startEvent, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Valid Sub Process Start[{0}] located, beginning process", startEvent.ID));
                    instance.State.Path.StartFlowNode(esp, sourceID);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.SubProcesses.Started, new object[] { esp, variables });
                    instance.State.Path.StartFlowNode(startEvent, null);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Events.Started, new object[] { startEvent, variables });
                    instance.State.Path.SucceedFlowNode(startEvent);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Events.Completed, new object[] { startEvent, variables });
                }
            }
        }

        private static void ProcessTask(ProcessInstance instance,string sourceID, ATask tsk)
        {
            instance.State.Path.StartFlowNode(tsk, sourceID);
            BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Tasks.Started,new object[] { tsk, new ReadOnlyProcessVariablesContainer(tsk.ID, instance) });
            try
            {
                ProcessVariablesContainer variables = new(tsk.ID, instance);
                Tasks.ExternalTask task =null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                    case "ReceiveTask":
                    case "SendTask":
                    case "ServiceTask":
                    case "Task":
                    case "ScriptTask":
                    case "CallActivity":
                        task = new Tasks.ExternalTask(tsk, variables, instance);
                        break;
                }
                ProcessTask delTask = null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                        delTask = instance.Delegates.Tasks.ProcessBusinessRuleTask;
                        break;
                    case "ManualTask":
                        BusinessProcess.TriggerDelegateAsync(instance.Delegates.Tasks.BeginManualTask,new object[] { new Tasks.ManualTask(tsk, variables, instance) });
                        break;
                    case "ReceiveTask":
                        delTask = instance.Delegates.Tasks.ProcessRecieveTask;
                        break;
                    case "ScriptTask":
                        ((ScriptTask)tsk).ProcessTask(task,instance.Delegates.Tasks.ProcessScriptTask);
                        break;
                    case "SendTask":
                        delTask = instance.Delegates.Tasks.ProcessSendTask;
                        break;
                    case "ServiceTask":
                        delTask = instance.Delegates.Tasks.ProcessServiceTask;
                        break;
                    case "Task":
                        delTask = instance.Delegates.Tasks.ProcessTask;
                        break;
                    case "CallActivity":
                        delTask = instance.Delegates.Tasks.CallActivity;
                        break;
                    case "UserTask":
                        BusinessProcess.TriggerDelegateAsync(instance.Delegates.Tasks.BeginUserTask,new object[] { new Tasks.UserTask(tsk, variables, instance) });
                        break;
                }
                delTask?.Invoke(task);
                if (task!=null && !task.Aborted)
                    instance.MergeVariables(task);
            }
            catch (Exception e)
            {
                instance.WriteLogException(tsk, new StackFrame(1, true), DateTime.Now, e);
                BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Tasks.Error,new object[] { tsk, new ReadOnlyProcessVariablesContainer(tsk.ID, instance, e) });
                instance.State.Path.FailFlowNode(tsk, error:e);
            }
        }

        internal void ProcessEvent(ProcessInstance instance, string sourceID, AEvent evnt)
        {
            if (evnt is IntermediateCatchEvent)
            {
                SubProcess sp = (SubProcess)evnt.SubProcess;
                if (sp != null)
                    instance.State.Path.StartFlowNode(sp, sourceID);
            }
            instance.State.Path.StartFlowNode(evnt, sourceID);
            BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Events.Started, new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance) });
            if (evnt is BoundaryEvent @event && @event.CancelActivity)
                AbortStep(instance, sourceID, GetElement(@event.AttachedToID), new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
            bool success = true;
            TimeSpan? ts = null;
            if (evnt is IntermediateCatchEvent || evnt is IntermediateThrowEvent)
                ts = evnt.GetTimeout(new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
            if (ts.HasValue)
            {
                instance.State.SuspendStep(sourceID,evnt.ID, ts.Value);
                if (ts.Value.TotalMilliseconds > 0)
                {
                    StepScheduler.Instance.Sleep(ts.Value, instance, evnt);
                    return;
                }
                else
                    success = true;
            }else if (evnt is IntermediateThrowEvent event1)
            {
                if (evnt.SubType.HasValue)
                    GetEventHandlers(evnt.SubType.Value, event1.Message, evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance))
                        .ForEach(tsk => { ProcessEvent(instance, evnt.ID, tsk); });
            }
            else if (instance.Delegates.Validations.IsEventStartValid != null && (evnt is IntermediateCatchEvent || evnt is StartEvent))
            {
                try
                {
                    success = instance.Delegates.Validations.IsEventStartValid(evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(evnt, new StackFrame(1, true), DateTime.Now, e);
                    success = false;
                }
            }
            if (!success)
            {
                instance.State.Path.FailFlowNode(evnt);
                BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Events.Error,new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance) });
            }
            else
            {
                instance.State.Path.SucceedFlowNode(evnt);
                BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Events.Completed,new object[] { evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance) });
                if (evnt is EndEvent event1 && event1.IsProcessEnd)
                {
                    if (!event1.IsTermination)
                    {
                        SubProcess sp = (SubProcess)event1.SubProcess;
                        if (sp != null)
                        {
                            instance.State.Path.SucceedFlowNode(sp);
                            BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.SubProcesses.Completed, new object[] { sp, new ReadOnlyProcessVariablesContainer(sp.ID, instance) });
                        }
                        else
                        {
                            BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Processes.Completed, new object[] { event1.Process, new ReadOnlyProcessVariablesContainer(evnt.ID, instance) });
                            instance.CompleteProcess();
                        }
                    }
                    else
                    {
                        ReadOnlyProcessVariablesContainer vars = new(evnt.ID, instance);
                        instance.State.AbortableSteps.ForEach(str => { AbortStep(instance, evnt.ID, GetElement(str), vars); });
                        BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Processes.Completed, new object[] { event1.Process, new ReadOnlyProcessVariablesContainer(evnt.ID, instance) });
                        instance.CompleteProcess();
                    }
                }
            }
        }

        private void AbortStep(ProcessInstance instance,string sourceID,IElement element,IReadonlyVariables variables)
        {
            instance.State.Path.AbortStep(sourceID, element.ID);
            BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.OnStepAborted, new object[] { element, GetElement(sourceID), variables });
            if (element is SubProcess process)
            {
                process.Children.ForEach(child =>
                {
                    bool abort = false;
                    switch (instance.State.Path.GetStatus(child.ID))
                    {
                        case StepStatuses.Suspended:
                            abort=true;
                            StepScheduler.Instance.AbortSuspendedElement(instance, child.ID);
                            break;
                        case StepStatuses.Waiting:
                        case StepStatuses.Started:
                            abort=true;
                            break;
                    }
                    if (abort)
                        AbortStep(instance, sourceID, child, variables);
                });
            }
        }

        private void ProcessGateway(ProcessInstance instance,string sourceID,AGateway gw)
        {
            if (instance.State.Path.ProcessGateway(gw,sourceID))
            {
                BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Gateways.Started,new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.ID, instance) });
                IEnumerable<string> outgoings = null;
                try
                {
                    outgoings = gw.EvaulateOutgoingPaths(definition, instance.Delegates.Validations.IsFlowValid, new ReadOnlyProcessVariablesContainer(gw.ID, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(gw, new StackFrame(1, true), DateTime.Now, e);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Gateways.Error,new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.ID, instance,e) });
                    outgoings = null;
                }
                if (outgoings==null || !outgoings.Any())
                {
                    instance.State.Path.FailFlowNode(gw);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Gateways.Error, new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.ID, instance,new Exception("No valid outgoing path located")) });
                }
                else
                {
                    instance.State.Path.SucceedFlowNode(gw, outgoing:outgoings);
                    BusinessProcess.TriggerDelegateAsync(instance.Delegates.Events.Gateways.Completed, new object[] { gw, new ReadOnlyProcessVariablesContainer(gw.ID, instance) });
                }
            }
        }

        private static void ProcessFlowElement(ProcessInstance instance,IFlowElement flowElement)
        {
            instance.State.Path.ProcessFlowElement(flowElement);
            Delegate delCall = instance.Delegates.Events.Flows.SequenceFlow;
            if (flowElement is MessageFlow)
                delCall = instance.Delegates.Events.Flows.MessageFlow;
            else if (flowElement is Association)
                delCall = instance.Delegates.Events.Flows.AssociationFlow;
            BusinessProcess.TriggerDelegateAsync(delCall,new object[] { flowElement, new ReadOnlyProcessVariablesContainer(flowElement.ID, instance) });
        }

        #region Logging
        internal void WriteLogLine(string elementID,LogLevel level,StackFrame sf,DateTime timestamp, string message)
        {
            WriteLogLine((IElement)(elementID == null ? null : GetElement(elementID)), level, sf, timestamp, message);
        }
        internal void WriteLogLine(IElement element, LogLevel level, StackFrame sf, DateTime timestamp, string message)
        {
            _delegates.Logging.LogLine?.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), level, timestamp, message);
        }

        internal Exception WriteLogException(string elementID,StackFrame sf, DateTime timestamp, Exception exception)
        {
            return WriteLogException((IElement)(elementID == null ? null : GetElement(elementID)), sf, timestamp, exception);
        }
        
        internal Exception WriteLogException(IElement element, StackFrame sf, DateTime timestamp, Exception exception)
        {
            if (_delegates.Logging.LogException != null)
            {
                _delegates.Logging.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, exception);
                if (exception is InvalidProcessDefinitionException processDefinitionException)
                {
                    processDefinitionException.ProcessExceptions
                        .ForEach(e =>{ _delegates.Logging.LogException.Invoke(element, sf.GetMethod().DeclaringType.Assembly.GetName(), sf.GetFileName(), sf.GetFileLineNumber(), timestamp, e); });
                }
            }
            return exception;
        }
        #endregion

        /// <summary>
        /// Called to Dispose of the given process instance.
        /// </summary>
        public void Dispose()
        {
            StepScheduler.Instance.UnloadProcess(this);
        }
        /// <summary>
        /// Compares a given process instance to this instance to see if they are the same.
        /// </summary>
        /// <param name="obj">The Business Process instance to compare this one to.</param>
        /// <returns>true if they are the same, false if they are not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is BusinessProcess process)
                return process._id == _id;
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
