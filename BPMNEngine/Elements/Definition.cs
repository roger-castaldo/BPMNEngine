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

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err?? []).Append("No child elements found in the definition.");
                return false;
            }
            return res;
        }

        internal BusinessProcess OwningProcess { get; set; }

        internal void LogLine(LogLevel level, IElement element, string message, params object[] pars)
            => OwningProcess?.WriteLogLine(element, level, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        internal Exception Exception(IElement element, Exception exception)
        {
            OwningProcess?.WriteLogException(element, new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }
    }
}
