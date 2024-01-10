using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Scripts;
using BPMNEngine.Interfaces.Tasks;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTag("bpmn", "scriptTask")]
    internal class ScriptTask : ATask
    {
        public ScriptTask(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }

        internal void ProcessTask(ITask task, ProcessTask processScriptTask)
        {
            if (ExtensionElement != null && ((ExtensionElements)ExtensionElement).Children!=null)
            {
                var script = ((ExtensionElements)ExtensionElement)
                    .Children
                    .OfType<AScript>()
                    .FirstOrDefault();
                if (script != null)
                    script.Invoke(task.Variables);
            }
            processScriptTask?.Invoke(task);
        }
    }
}
