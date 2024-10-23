using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Interfaces
{
    public interface IElementFactory
    {
        IElementFactory Register<T>(string elementName, string? elementPrefix = null)
            where T : IElement;

        IElement? ProduceInstance(XmlElement element, IElement? parent=null);
    }
}
