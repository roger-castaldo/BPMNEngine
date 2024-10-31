using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "callActivity")]
    internal record CallActivity : ATask
    {
        public CallActivity(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
