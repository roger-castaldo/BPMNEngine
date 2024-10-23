using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTagAttribute("bpmn", "textAnnotation")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    [ValidParent(typeof(IProcess))]
    internal record TextAnnotation : AParentElement
    {
        public TextAnnotation(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public string Content
            => Children.OfType<Text>()
                    .Select(elem => elem.Value)
                    .FirstOrDefault() ?? string.Empty;

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid,var errors) = base.IsValid(logger);
            if (string.IsNullOrEmpty(Content))
            {
                errors = errors.Append("No content for the text annotation was specified.");
                isValid = false;
            }
            return (isValid, errors);
        }
    }
}
