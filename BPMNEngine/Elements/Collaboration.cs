using BPMNEngine.Attributes;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "collaboration")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Definition))]
    internal record Collaboration : AParentElement
    {
        public Collaboration(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

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
