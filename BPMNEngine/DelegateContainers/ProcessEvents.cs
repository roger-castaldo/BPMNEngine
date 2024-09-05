using BPMNEngine.DelegateContainers.Events;

namespace BPMNEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the event based delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public record class ProcessEvents
    {
        /// <summary>
        /// Houses the delegates for Events related to a Business Process Event item
        /// </summary>
        public BasicEvents Events { get; init; } = new BasicEvents();
        /// <summary>
        /// Houses the delegates for Events related to a Business Process Task item
        /// </summary>
        public BasicEvents Tasks { get; init; } = new BasicEvents();
        /// <summary>
        /// Houses the delegates for Events related to a Business Process Process item
        /// </summary>
        public ElementProcessEvents Processes { get; init; } = new ElementProcessEvents();
        /// <summary>
        /// Houses the delegates for Events related to a Business Process SubProcess item
        /// </summary>
        public BasicEvents SubProcesses { get; init; } = new BasicEvents();
        /// <summary>
        /// Houses the delegates for Events related to a Business Process Gateway item
        /// </summary>
        public BasicEvents Gateways { get; init; } = new BasicEvents();
        /// <summary>
        /// Houses the delegates for Events related to a Business Process flow item
        /// </summary>
        public FlowEvents Flows { get; init; } = new FlowEvents();
        /// <summary>
        /// A delegate called when an element is aborted within the Business Process
        /// <code>
        /// public void OnStepAborted(IElement element, IElement source, IReadonlyVariables variables){
        ///         Console.WriteLine("Element {0} inside process {1} has been aborted by {2} with the following variables:",element.id,element.Process.id,source.id);
        ///         foreach (string key in variables.FullKeys){
        ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
        ///         }
        ///     }
        /// </code>
        /// </summary>
        public OnElementAborted OnStepAborted { get; init; }
        /// <summary>
        /// A delegate called when the Business Process Instance state document has changed
        /// <code>
        /// public void OnStateChange(XmlDocument stateDocument){
        ///         Console.WriteLine("Current Process State: \n{0}",stateDocument.OuterXML);
        ///     }
        /// </code>
        /// </summary>
        public OnStateChange OnStateChange { get; init; }

        internal static ProcessEvents Merge(ProcessEvents source, ProcessEvents append)
        {
            source??=new ProcessEvents();
            append??=new ProcessEvents();
            return new ProcessEvents()
            {
                Events = BasicEvents.Merge(source.Events, append.Events),
                Tasks = BasicEvents.Merge(source.Tasks, append.Tasks),
                Processes=ElementProcessEvents.Merge(source.Processes, append.Processes),
                SubProcesses=BasicEvents.Merge(source.SubProcesses, append.SubProcesses),
                Gateways=BasicEvents.Merge(source.Gateways, append.Gateways),
                Flows=FlowEvents.Merge(source.Flows, append.Flows),
                OnStepAborted=(source.OnStepAborted!=null&&append.OnStepAborted!=null ?
                                source.OnStepAborted+append.OnStepAborted
                                : source.OnStepAborted??append.OnStepAborted),
                OnStateChange=(source.OnStateChange!=null&&append.OnStateChange!=null ?
                                source.OnStateChange+append.OnStateChange
                                : source.OnStateChange??append.OnStateChange)
            };
        }
    }
}
