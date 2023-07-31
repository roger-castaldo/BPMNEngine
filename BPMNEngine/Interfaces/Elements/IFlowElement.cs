namespace BPMNEngine.Interfaces.Elements
{
    /// <summary>
    /// An interface for a flow element which can be message, sequence, or in some cases association
    /// </summary>
    public interface IFlowElement : IElement
    {
        /// <summary>
        /// The id for the source element 
        /// </summary>
        string SourceRef { get; }
        /// <summary>
        /// the id for the destination element
        /// </summary>
        string TargetRef { get; }
    }
}
