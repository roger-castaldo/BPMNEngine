using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "manualTask")]
    internal record ManualTask : ATask
    {
        public ManualTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
