using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "sendTask")]
    internal record SendTask : ATask
    {
        public SendTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
