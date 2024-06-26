using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Scripts;
using BPMNEngine.Interfaces.Tasks;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "scriptTask")]
    internal record ScriptTask : ATask
    {
        public ScriptTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        internal void ProcessTask(ITask task, ProcessTask processScriptTask)
        {
            ExtensionElement?.Children
                .OfType<AScript>()
                .FirstOrDefault()?.Invoke(task.Variables);
            processScriptTask?.Invoke(task);
        }
    }
}
