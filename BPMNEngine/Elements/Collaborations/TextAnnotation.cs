using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements.Collaborations
{
    [XMLTag("bpmn","textAnnotation")]
    [RequiredAttribute("id")]
    [ValidParent(typeof(Collaboration))]
    [ValidParent(typeof(IProcess))]
    internal class TextAnnotation : AParentElement
    {
        public string Content 
            => Children.OfType<Text>()
                    .Select(elem=>elem.Value)
                    .FirstOrDefault() ?? string.Empty;

        public TextAnnotation(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (string.IsNullOrEmpty(Content))
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "No content for the text annotation was specified." });
                return false;
            }
            return res;
        }
    }
}
