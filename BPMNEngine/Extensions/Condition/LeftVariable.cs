using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Extensions.Conditions
{
    [XMLTagAttribute("exts", "left")]
    internal record LeftVariable(XmlElement Element,IBaseElement? Parent) : AExtension(Element,Parent)
    {
        public string Value => Element.InnerText;
    }
}
