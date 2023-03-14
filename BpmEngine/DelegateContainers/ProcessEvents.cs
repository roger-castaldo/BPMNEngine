using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the event based delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public class ProcessEvents
    {
        /// <summary>
        /// Base class used to define the properties (event types) for a given
        /// element types events
        /// </summary>
        public class BasicEvents
        {
            /// <summary>
            /// This is the delegate called when a particular element starts
            /// <code>
            /// public void OnEventStarted(IStepElement Event, IReadonlyVariables variables);{
            ///     Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
            ///     foreach (string key in variables.FullKeys){
            ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///     }
            /// }
            /// </code>
            /// </summary>
            public OnElementEvent Started{get;init;}
            /// <summary>
            /// This delegate is called when a particular element completes
            /// <code>
            /// public void OnEventCompleted(IStepElement Event, IReadonlyVariables variables){
            ///     Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
            ///     foreach (string key in variables.FullKeys){
            ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///     }
            /// }
            /// </code>
            /// </summary>
            public OnElementEvent Completed { get; init; }
            /// <summary>
            /// This delegate is called when a particular element has an error
            /// <code>
            ///     public void OnEventError(IStepElement Event, IReadonlyVariables variables){
            ///         Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
            ///         foreach (string key in variables.FullKeys){
            ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///         }
            ///     }
            /// </code>
            /// </summary>
            public OnElementEvent Error { get; init; }

            internal static BasicEvents Merge(BasicEvents source,BasicEvents append)
            {
                if (source==null&&append==null) return null;
                if (source==null) return append;
                if (append==null) return source;
                return new BasicEvents()
                {
                    Started = (source.Started!=null&&append.Started!=null ?
                                source.Started+append.Started
                                : source.Started ?? append.Started),
                    Completed = (source.Completed!=null&&append.Completed!=null ?
                                source.Completed+append.Completed
                                : source.Completed ?? append.Completed),
                    Error = (source.Error!=null&&append.Error!=null ?
                                source.Error+append.Error
                                : source.Error ?? append.Error)
                };
            }
        }

        /// <summary>
        /// Class used to define the properties (event types) for a Process
        /// </summary>
        public class ElementProcessEvents
        {
            /// <summary>
            /// This is the delegate called when a particular element starts
            /// <code>
            /// public void OnEventStarted(IStepElement Event, IReadonlyVariables variables);{
            ///     Console.WriteLine("Event {0} inside process {1} has been started with the following variables:",Event.id,Event.Process.id);
            ///     foreach (string key in variables.FullKeys){
            ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///     }
            /// }
            /// </code>
            /// </summary>
            public OnElementEvent Started { get; init; }
            /// <summary>
            /// This delegate is called when a particular element completes
            /// <code>
            /// public void OnEventCompleted(IStepElement Event, IReadonlyVariables variables){
            ///     Console.WriteLine("Event {0} inside process {1} has completed with the following variables:",Event.id,Event.Process.id);
            ///     foreach (string key in variables.FullKeys){
            ///         Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///     }
            /// }
            /// </code>
            /// </summary>
            public OnElementEvent Completed { get; init; }
            /// <summary>
            /// This delegate is called when a particular element has an error
            /// <code>
            ///     public void OnEventError(IStepElement Event, IReadonlyVariables variables){
            ///         Console.WriteLine("Event {0} inside process {1} had the error {2} occur with the following variables:",new object[]{Event.id,Event.Process.id,variables.Error.Message});
            ///         foreach (string key in variables.FullKeys){
            ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///         }
            ///     }
            /// </code>
            /// </summary>
            public OnProcessErrorEvent Error { get; init; }

            internal static ElementProcessEvents Merge(ElementProcessEvents source, ElementProcessEvents append)
            {
                if (source==null&&append==null) return null;
                if (source==null) return append;
                if (append==null) return source;
                return new ElementProcessEvents()
                {
                    Started = (source.Started!=null&&append.Started!=null ?
                                source.Started+append.Started
                                : source.Started ?? append.Started),
                    Completed = (source.Completed!=null&&append.Completed!=null ?
                                source.Completed+append.Completed
                                : source.Completed ?? append.Completed),
                    Error = (source.Error!=null&&append.Error!=null ?
                                source.Error+append.Error
                                : source.Error ?? append.Error)
                };
            }
        }

        /// <summary>
        /// Class used to define all Flow Events that can complete
        /// </summary>
        public class FlowEvents
        {
            /// <summary>
            /// Called when a Sequence Flow completes
            /// <code>
            /// public void OnSequenceFlowCompleted(IFlowElement flow, IReadonlyVariables variables){
            ///         Console.WriteLine("Sequence Flow {0} has been completed with the following variables:",flow.id);
            ///         foreach (string key in variables.FullKeys){
            ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///         }
            ///     }
            /// </code>
            /// </summary>
            public OnFlowComplete SequenceFlow { get; init; }
            /// <summary>
            /// Called when a Message Flow completes
            /// </summary>
            /// <code>
            /// public void OnMessageFlowCompleted(IFlowElement flow, IReadonlyVariables variables){
            ///         Console.WriteLine("Message Flow {0} has been completed with the following variables:",flow.id);
            ///         foreach (string key in variables.FullKeys){
            ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///         }
            ///     }
            /// </code>
            public OnFlowComplete MessageFlow { get; init; }
            /// <summary>
            /// Called when an Association Flow completes
            /// <code>
            /// public void onAssociationFlowCompleted(IFlowElement flow, IReadonlyVariables variables){
            ///         Console.WriteLine("Association Flow {0} has been completed with the following variables:",flow.id);
            ///         foreach (string key in variables.FullKeys){
            ///             Console.WriteLine("\t{0}:{1}",key,variables[key]);
            ///         }
            ///     }
            /// </code>
            /// </summary>
            public OnFlowComplete AssociationFlow { get; init; }

            internal static FlowEvents Merge(FlowEvents source, FlowEvents append)
            {
                if (source==null&&append==null) return null;
                if (source==null) return append;
                if (append==null) return source;
                return new FlowEvents()
                {
                    SequenceFlow = (source.SequenceFlow!=null&&append.SequenceFlow!=null ?
                                source.SequenceFlow+append.SequenceFlow
                                : source.SequenceFlow ?? append.SequenceFlow),
                    MessageFlow = (source.MessageFlow!=null&&append.MessageFlow!=null ?
                                source.MessageFlow+append.MessageFlow
                                : source.MessageFlow ?? append.MessageFlow),
                    AssociationFlow = (source.AssociationFlow!=null&&append.AssociationFlow!=null ?
                                source.AssociationFlow+append.AssociationFlow
                                : source.AssociationFlow ?? append.AssociationFlow)
                };
            }
        }
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
            if(source==null&&append==null) return new ProcessEvents();
            if (source==null) return append;
            if (append==null) return source;
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
