using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "receiveTask")]
    internal class ReceiveTask : ATask
    {
        public ReceiveTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
