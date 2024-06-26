using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "receiveTask")]
    internal record ReceiveTask : ATask
    {
        public ReceiveTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
