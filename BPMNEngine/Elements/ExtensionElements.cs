using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using System.Collections.Immutable;
using BPMNEngine.Interfaces.Extensions;

namespace BPMNEngine.Elements.Processes
{
    [XMLTagAttribute("bpmn", "extensionElements")]
    [ValidParent(null)]
    internal record ExtensionElements : AElement,IExtensionsElement
    {
        private readonly IElementFactory elementFactory;
        public ExtensionElements(XmlElement elem, IBaseElement parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory)
            => this.elementFactory=elementFactory;

        private ImmutableArray<IExtensionElement>? extensions;
        public ImmutableArray<IExtensionElement> Extensions
            => extensions??=SubNodes.OfType<XmlElement>()
                .Select(e => elementFactory.ProduceExtensionElement(e, this))
                .OfType<IExtensionElement>()
                .ToImmutableArray();

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            var isValid = true;
            IEnumerable<string> errors = [];
            Extensions.OfType<IValidatableElement>().ForEach(e =>
            {
                (var isChildValid, var childErrors) = e.IsValid(logger);
                errors = errors.Concat(childErrors);
                isValid &= isChildValid;
            });
            return (isValid, errors);
        }
    }
}
