using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "userTask")]
    internal class UserTask : ATask
    {
        public UserTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
