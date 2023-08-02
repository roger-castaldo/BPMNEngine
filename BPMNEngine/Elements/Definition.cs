using BPMNEngine.Attributes;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Interfaces.Elements;

namespace BPMNEngine.Elements
{
    [XMLTag("bpmn","definitions")]
    [RequiredAttribute("id")]
    [ValidParent(null)]
    internal class Definition : AParentElement
    {
        public Definition(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public IEnumerable<Diagram> Diagrams => Children.OfType<Diagram>();

        public IEnumerable<MessageFlow> MessageFlows => LocateElementsOfType<MessageFlow>();

        public IElement LocateElement(string id)
            => (this.ID==id
                ? this
                : Children.Traverse(ielem => (ielem is IParentElement element ? element.Children : Array.Empty<IElement>())).FirstOrDefault(elem => elem.ID==id)
            );

        public IEnumerable<T> LocateElementsOfType<T>() where T: IElement
            => Children.Traverse(ielem => (ielem is IParentElement element ? element.Children : Array.Empty<IElement>())).OfType<T>();

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any())
            {
                err = (err??Array.Empty<string>()).Concat(new string[] { "No child elements found." });
                return false;
            }
            return res;
        }

        internal BusinessProcess OwningProcess { get; set; }

        internal void Debug(IElement element,string message)
            => OwningProcess?.WriteLogLine(element, LogLevel.Debug, new StackFrame(2, true), DateTime.Now, message);

        internal void Debug(IElement element, string message, object[] pars)
            => OwningProcess?.WriteLogLine(element, LogLevel.Debug, new StackFrame(2, true), DateTime.Now, string.Format(message,pars));

        internal void Info(IElement element,string message)
            => OwningProcess?.WriteLogLine(element, LogLevel.Information, new StackFrame(2, true), DateTime.Now, message);
        internal void Info(IElement element, string message, object[] pars)
            => OwningProcess?.WriteLogLine(element, LogLevel.Information, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        internal void Error(IElement element, string message)
            => OwningProcess?.WriteLogLine(element, LogLevel.Error, new StackFrame(2, true), DateTime.Now, message);
        internal void Error(IElement element, string message, object[] pars)
            => OwningProcess?.WriteLogLine(element, LogLevel.Error, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        internal void Fatal(IElement element, string message)
            => OwningProcess?.WriteLogLine(element, LogLevel.Critical, new StackFrame(2, true), DateTime.Now, message);
        internal void Fatal(IElement element, string message, object[] pars)
            => OwningProcess?.WriteLogLine(element, LogLevel.Critical, new StackFrame(2, true), DateTime.Now, string.Format(message, pars));
        internal Exception Exception(IElement element, Exception exception)
        {
            OwningProcess?.WriteLogException(element, new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }
    }
}
