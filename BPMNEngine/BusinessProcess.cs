using BPMNEngine.Attributes;
using BPMNEngine.DelegateContainers;
using BPMNEngine.Elements;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Extensions.Definition;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Logging;
using BPMNEngine.Scheduling;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BPMNEngine
{
    /// <summary>
    /// This class is the primary class for the library.  It implements a Business Process by constructing the object using a BPMN 2.0 compliant definition.
    /// This is followed by assigning delegates for handling the specific process events and then starting the process.  A process can also be suspended and 
    /// the suspended state loaded and resumed.  It can also be cloned, including the current state and delegates in order to have more than once instance 
    /// of the given process executing.
    /// </summary>
    public sealed partial class BusinessProcess : IDisposable
    {
        private readonly Guid id;
        private readonly List<object> components;
        private readonly IEnumerable<AHandlingEvent> eventHandlers = null;
        private readonly Definition definition;
        private readonly ILoggerFactory loggerFactory;

        internal IElement? GetElement(string id) => Elements.FirstOrDefault(elem => elem.ID==id);
        private IEnumerable<IElement> Elements
            => components.OfType<IElement>()
            .Traverse(elem => (elem is IParentElement element ? element.Children.OfType<IElement>() : []));

        /// <summary>
        /// The XML Document that was supplied to the constructor containing the BPMN 2.0 definition
        /// </summary>
        public XmlDocument Document { get; private init; }

        private readonly IEnumerable<SProcessRuntimeConstant> constants;
        /// <summary>
        /// This is used to access the values of the process runtime and definition constants
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The value of the variable</returns>
        public object? this[string name]
        {
            get
            {
                if (constants != null && constants.Any(c => c.Name==name))
                    return constants.FirstOrDefault(c => c.Name==name).Value;
                if (definition==null || definition.ExtensionElement==null)
                    return null;
                var definitionVariable = definition.ExtensionElement.Extensions
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
                    return constants==null ? [] : constants.Select(c => c.Name);
                return (constants==null ? [] : constants.Select(c => c.Name))
                    .Concat(
                        definition.ExtensionElement
                        .Extensions
                        .OfType<DefinitionVariable>()
                        .Select(d => d.Name)
                    )
                    .Concat(
                        definition.ExtensionElement
                        .Extensions
                        .OfType<DefinitionFile>()
                        .Select(d => d.Name)
                    )
                    .Concat(
                        definition.ExtensionElement
                        .Extensions
                        .OfType<DefinitionFile>()
                        .Select(d => string.Format("{0}.{1}", d.Name, d.Extension))
                    )
                    .Distinct();
            }
        }

        private readonly DelegateContainer delegates;

        internal ATask GetTask(string taskID)
        {
            IElement elem = GetElement(taskID);
            if (elem is ATask task)
                return task;
            return null;
        }

        internal async ValueTask<bool> HandleTaskEmissionAsync(ProcessInstance instance, ITask task, object data, EventSubTypes type)
        {
            await (await 
                GetEventHandlersAsync(type, data, (AFlowNode)GetElement(task.ID), new ReadOnlyProcessVariablesContainer(task.Variables), instance.GetLogger(task.ID))
            ).ForEachAsync(ahe => ProcessEventAsync(instance, task.ID, ahe));
            return instance.State.Path.GetStatus(task.ID)==StepStatuses.Aborted;
        }

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
            StepValidations validations = null,
            ProcessTasks tasks = null,
            ILoggerFactory loggerFactory = null,
            IServiceProvider serviceProvider = null
            )
        {
            id = Guid.NewGuid();
            this.constants = constants;
            this.loggerFactory = loggerFactory;
            serviceProvider??=new ServiceCollection().BuildServiceProvider();
            delegates = new DelegateContainer()
            {
                Events=ProcessEvents.Merge(null, events),
                Validations=StepValidations.Merge(null, validations),
                Tasks=ProcessTasks.Merge(null, tasks)
            };

            var logger = loggerFactory?.CreateLogger<BusinessProcess>();

            IEnumerable<Exception> exceptions = [];
            Document = new XmlDocument();
            Document.LoadXml(doc.OuterXml);
            IElementFactory elementFactory = ElementFactory.Instance(serviceProvider, logger);
            var stopwatch = Stopwatch.StartNew();
            logger?.LogInformation("Producing new Business Process from XML Document");
            components = [];
            XmlPrefixMap map = new(logger);
            _=doc.ChildNodes.Cast<XmlNode>().ForEach(n =>
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    var elem = elementFactory.ProduceInstance((XmlElement)n);
                    if (elem != null)
                    {
                        if (elem is Definition def)
                            def.OwningProcess = this;
                        components.Add(elem);
                    }
                    else
                        components.Add(n);
                }
                else
                    components.Add(n);
            });
            definition = components.OfType<Definition>().FirstOrDefault();
            if (!Elements.Any())
                exceptions = exceptions.Append(new XmlException("Unable to load a bussiness process from the supplied document.  No bpmn elements were located."));
            else if (definition==null)
                exceptions = exceptions.Append(new XmlException("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
            if (!exceptions.Any())
                exceptions = exceptions.Concat(ValidateElement(definition,definition,logger));
            if (exceptions.Any())
            {
                Exception ex = new InvalidProcessDefinitionException(exceptions);
                logger?.LogCritical(ex, "Invalid Process Definition found");
                throw ex;
            }
            eventHandlers = Elements
                .OfType<AHandlingEvent>();
            stopwatch.Stop();
            logger?.LogInformation("Time to load Process Document {TotalMilliseconds}ms", stopwatch.ElapsedMilliseconds);
        }

        private static IEnumerable<Exception> ValidateElement(Definition? definition,IElement elem,ILogger? logger)
        {
            using var scope = MultiLogger.DefineElementLogScope(elem, logger);
            logger?.LogDebug("Attempting to validate element");
            IEnumerable<Exception> result = [];
            result = result.Concat(
                elem.GetType().GetCustomAttributes(true).OfType<RequiredAttributeAttribute>()
                .Where(ra => elem[ra.Name]==null)
                .Select(ra => new MissingAttributeException(definition, elem.Element, ra))
                .ToArray()
            ).Concat(
                elem.GetType().GetCustomAttributes(true).OfType<AttributeRegexAttribute>()
                .Where(ar => !ar.IsValid(elem))
                .Select(ar => new InvalidAttributeValueException(definition, elem.Element, ar))
                .ToArray()
            );
            var isValid = true;
            IEnumerable<string> errors = [];
            if (elem is IValidatableElement validatableElement)
                (isValid, errors) = validatableElement.IsValid(logger);
            if (!isValid)
                result = result.Append(new InvalidElementException(definition, elem.Element, errors));
            if (elem is AParentElement element)
                result = result.Concat(
                    element.Children
                    .OfType<AElement>()
                    .Select(e => ValidateElement(definition,e,logger))
                    .SelectMany(res => res)
                    .ToArray()
                );
            return result.ToArray();
        }

        private ProcessInstance ProduceInstance(ProcessEvents events,
            StepValidations validations,
            ProcessTasks tasks,
            LogLevel stateLogLevel)
            => new(this, 
                DelegateContainer.Merge(delegates, new DelegateContainer()
                {
                    Events = events,
                    Validations = validations,
                    Tasks = tasks
                }), 
                stateLogLevel,
                loggerFactory?.CreateLogger<ProcessInstance>()
            );

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
            bool autoResume = false,
            ProcessEvents events = null,
            StepValidations validations = null,
            ProcessTasks tasks = null,
            LogLevel stateLogLevel = LogLevel.None)
        {
            ProcessInstance ret = ProduceInstance(events, validations, tasks, stateLogLevel);
            return ret.LoadState(doc, autoResume) ? ret : null;
        }

        /// <summary>
        /// Called to load a Process Instance from a stored State Document
        /// </summary>
        /// <param name="reader">The json based process state</param>
        /// <param name="autoResume">set true if the process was suspended and needs to resume once loaded</param>
        /// <param name="events">The Process Events delegates container</param>
        /// <param name="validations">The Process Validations delegates container</param>
        /// <param name="tasks">The Process Tasks delegates container</param>
        /// <param name="logging">The Process Logging delegates container</param>
        /// <param name="stateLogLevel">Used to set the logging level for the process state document</param>
        /// <returns>an instance of IProcessInstance if successful or null it failed</returns>
        public IProcessInstance LoadState(Utf8JsonReader reader,
            bool autoResume = false,
            ProcessEvents events = null,
            StepValidations validations = null,
            ProcessTasks tasks = null,
            LogLevel stateLogLevel = LogLevel.None)
        {
            ProcessInstance ret = ProduceInstance(events, validations, tasks, stateLogLevel);
            return ret.LoadState(reader, autoResume) ? ret : null;
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
        public async ValueTask<IProcessInstance> BeginProcessAsync(
            Dictionary<string, object> pars = null,
            ProcessEvents events = null,
            StepValidations validations = null,
            ProcessTasks tasks = null,
            LogLevel stateLogLevel = LogLevel.None)
        {
            ProcessInstance ret = new(
                this, 
                DelegateContainer.Merge(delegates, new DelegateContainer()
                {
                    Events = events,
                    Validations = validations,
                    Tasks = tasks
                }), 
                stateLogLevel,
                loggerFactory?.CreateLogger<ProcessInstance>()
            );
            ProcessVariablesContainer variables = new(pars, this);
            using var logger = ret.GetLogger();
            logger.LogDebug("Attempting to being process");
            ReadOnlyProcessVariablesContainer ropvc = new(variables);
            var proc = await Elements.OfType<Elements.Process>().FirstOrDefaultAsync(p => p.IsStartValidAsync(ropvc, ret.Delegates.Validations.IsProcessStartValid,ret.GetLogger(p)));
            if (proc != null)
            {
                var start = await proc.StartEvents.FirstOrDefaultAsync(se => se.IsEventStartValidAsync(ropvc, ret.Delegates.Validations.IsEventStartValid, logger));
                if (start!=null)
                {
                    logger.LogInformation("Valid Process Start[{ElementID}] located, beginning process", start.ID);
                    TriggerDelegateAsync(
                        ret.Delegates.Events.Processes.Started,
                        proc,
                        new ReadOnlyProcessVariablesContainer(variables)
                    );
                    TriggerDelegateAsync(
                        ret.Delegates.Events.Events.Started,
                        start,
                        new ReadOnlyProcessVariablesContainer(variables)
                    );
                    using var elementLogger = ret.GetLogger(start);
                    ret.State.Path.StartFlowNode(start, null, elementLogger);
                    variables.Keys.ForEach(key => ret.State[start.ID, key] = variables[key]);
                    ret.State.Path.SucceedFlowNode(start, elementLogger);
                    TriggerDelegateAsync(
                        ret.Delegates.Events.Events.Completed,
                        start,
                        new ReadOnlyProcessVariablesContainer(start.ID, ret)
                    );
                    return ret;
                }
            }
            logger.LogInformation("Unable to begin process, no valid start located");
            return null;
        }

        /// <summary>
        /// Called to Dispose of the given process instance.
        /// </summary>
        public void Dispose()
            => StepScheduler.Instance.UnloadProcess(this);
        /// <summary>
        /// Compares a given process instance to this instance to see if they are the same.
        /// </summary>
        /// <param name="obj">The Business Process instance to compare this one to.</param>
        /// <returns>true if they are the same, false if they are not.</returns>
        public override bool Equals(object obj)
            => obj is BusinessProcess process &&process.id==id;

        /// <summary>
        /// Returns the HashCode of the Business Process instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => id.GetHashCode();
    }
}
