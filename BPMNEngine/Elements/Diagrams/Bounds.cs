using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("dc", "Bounds")]
    [RequiredAttributeAttribute("x")]
    [AttributeRegexAttribute("x", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttributeAttribute("y")]
    [AttributeRegexAttribute("y", "^-?\\d+(\\.\\d+)?$")]
    [RequiredAttributeAttribute("width")]
    [AttributeRegexAttribute("width", "^\\d+(\\.\\d+)?$")]
    [RequiredAttributeAttribute("height")]
    [AttributeRegexAttribute("height", "^\\d+(\\.\\d+)?$")]
    [ValidParent(typeof(Label))]
    [ValidParent(typeof(Shape))]
    internal record Bounds : AElement
    {
        public Bounds(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
        public RectF Rectangle => new(
                    float.Parse(this["x"]),
                    float.Parse(this["y"]),
                    float.Parse(this["width"]),
                    float.Parse(this["height"]));
    }
}
