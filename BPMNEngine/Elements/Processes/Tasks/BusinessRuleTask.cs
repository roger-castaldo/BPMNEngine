using BPMNEngine.Attributes;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "businessRuleTask")]
    internal class BusinessRuleTask : ATask
    {
        public BusinessRuleTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
    }
}
