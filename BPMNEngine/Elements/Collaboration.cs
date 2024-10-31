using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "collaboration")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid,var errors) = base.IsValid(logger);
            if (!Children.Any())
            {
                errors = errors.Append("Collaboration requires at least 1 child element.");
                isValid=false;
            }
            return (isValid, errors);
        }
    }
}
