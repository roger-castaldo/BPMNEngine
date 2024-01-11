using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.Elements
{
    internal interface IParentElement : IElement
    {
        ImmutableArray<IElement> Children { get; }
    }
}
