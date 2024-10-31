using BPMNEngine.Extensions.Condition.Conditions;
using BPMNEngine.Extensions.Scripts;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Conditions
{
    internal abstract record ACondition : AExtension,IStepElementStartCheckExtensionElement
    {
        private readonly IExtensionElementFactory elementFactory;
        private IEnumerable<IExtensionElement>? children;

        protected ACondition(XmlElement xmlElement, IBaseElement? parent, IExtensionElementFactory elementFactory)
            : base(xmlElement, parent)
            => this.elementFactory=elementFactory;

        protected IEnumerable<IExtensionElement> Children 
            => children??=Element.ChildNodes
                .OfType<XmlElement>()
                .Select(elem => elementFactory.ProduceExtensionElement(elem, this))
                .OfType<IExtensionElement>()
                .Select(ie => (ie is AScript script ? new ScriptCondition(script, this, elementFactory) : ie))
                .OfType<IExtensionElement>();

        public abstract ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger);
    }
}
