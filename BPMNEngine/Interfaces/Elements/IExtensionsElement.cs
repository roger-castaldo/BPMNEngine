using BPMNEngine.Interfaces.Extensions;
using System.Collections.Immutable;

namespace BPMNEngine.Interfaces.Elements
{
    public interface IExtensionsElement : IValidatableElement
    {
        ImmutableArray<IExtensionElement> Extensions{ get; }
    }
}
