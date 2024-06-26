using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.Elements
{
    /// <summary>
    /// This interface is the interface for all process elements with children
    /// </summary>
    public interface IParentElement : IElement
    {
        /// <summary>
        /// The child elements of the given process element
        /// </summary>
        ImmutableArray<IElement> Children { get; }
    }
}
