using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Events
{
    [ValidParent(typeof(IProcess))]
    internal abstract class AEvent : AFlowNode
    {
        public EventSubTypes? SubType
            => (
            Children.Any(elem=>elem is IEventDefinition) ? 
            Children
            .OfType<IEventDefinition>()
            .Select(ie => ie.Type)
            .First()
            : null);

        public AEvent(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        public TimeSpan? GetTimeout(IReadonlyVariables variables)
        {
            return Children
                .OfType<TimerEventDefinition>()
                .Select(ie => ie.GetTimeout(variables))
                .FirstOrDefault();
        }
    }
}
