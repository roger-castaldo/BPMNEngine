using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Extensions.Conditions
{
    [XMLTagAttribute("exts", "right")]
    internal record RightVariable(XmlElement Element, IBaseElement? Parent) : AExtension(Element, Parent)
    {
        public string Value => Element.InnerText;
    }
}
