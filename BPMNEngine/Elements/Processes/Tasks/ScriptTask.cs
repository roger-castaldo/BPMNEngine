using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "scriptTask")]
    internal record ScriptTask : ATask
    {
        public ScriptTask(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }
    }
}
