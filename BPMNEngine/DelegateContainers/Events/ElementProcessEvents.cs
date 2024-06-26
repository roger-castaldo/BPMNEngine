namespace BPMNEngine.DelegateContainers.Events
{
    /// <summary>
    /// Class used to define the properties (event types) for a Process
    /// </summary>
    public record ElementProcessEvents
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
        ///     public void OnEventError(IStepElement process,IStepElement Event, IReadonlyVariables variables){
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
}