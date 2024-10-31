using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Interfaces
{
    internal interface IElementFactory : IExtensionElementFactory
    {
        IElement? ProduceInstance(XmlElement element, IElement? parent=null);

        bool IsOfType<T>(XmlElement element)
            where T : IBaseElement;
    }
}
