using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "callActivity")]
    internal class CallActivity : ATask
    {
        public CallActivity(XmlElement elem, XmlPrefixMap map, AElement parent) 
            : base(elem, map, parent) {}
    }
}
