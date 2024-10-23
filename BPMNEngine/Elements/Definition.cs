using BPMNEngine.Attributes;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    [XMLTagAttribute("bpmn", "definitions")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(null)]
    internal record Definition : AParentElement
    {
        public Definition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public override Definition OwningDefinition => this;

        public IEnumerable<Diagram> Diagrams => Children.OfType<Diagram>();

        public IEnumerable<MessageFlow> MessageFlows => LocateElementsOfType<MessageFlow>();

        public IElement LocateElement(string id)
            => (this.ID==id
                ? this
                : Children.Traverse(ielem => (ielem is IParentElement element ? element.Children : Array.Empty<IElement>())).FirstOrDefault(elem => elem.ID==id)
            );

        public IEnumerable<T> LocateElementsOfType<T>() where T : IElement
            => Children.Traverse(ielem => (ielem is IParentElement element ? element.Children : Array.Empty<IElement>())).OfType<T>();

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!Children.Any())
            {
                errors = errors.Append("No child elements found in the definition.");
                isValid = false;
            }
            return (isValid, errors);
        }

        internal BusinessProcess OwningProcess { get; set; }
    }
}
