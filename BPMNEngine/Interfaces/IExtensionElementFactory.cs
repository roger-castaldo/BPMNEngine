using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;

namespace BPMNEngine.Interfaces
{
    public interface IExtensionElementFactory
    {
        IExtensionElementFactory RegisterExtension<T>(string elementName, string? elementPrefix = null)
            where T : IExtensionElement;

        IExtensionElement? ProduceExtensionElement(XmlElement element, IBaseElement parent);
    }
}
