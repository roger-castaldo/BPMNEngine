namespace BPMNEngine.DelegateContainers.Events
{

    /// <summary>
    /// Class used to define all Flow Events that can complete
    /// </summary>
    public record FlowEvents
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
}