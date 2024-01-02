using BPMNEngine.Elements.Processes.Events;

namespace BPMNEngine.Interfaces.Elements
{
    internal interface IEventDefinition : IElement
    {
        EventSubTypes Type { get; }
    }
}
