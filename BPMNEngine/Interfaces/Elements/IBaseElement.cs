using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.Elements
{
    public interface IBaseElement
    {
        IBaseElement? Parent { get; }
        XmlElement Element { get; }
        /// <summary>
        /// The child XMLNodes from the process element
        /// </summary>
        ImmutableArray<XmlNode> SubNodes { get; }
    }
}
