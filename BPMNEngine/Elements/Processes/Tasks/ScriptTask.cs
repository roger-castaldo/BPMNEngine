using BPMNEngine.Attributes;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Extensions.Scripts;

namespace BPMNEngine.Elements.Processes.Tasks
{
    [XMLTagAttribute("bpmn", "scriptTask")]
    internal record ScriptTask : ATask
    {
        public ScriptTask(XmlElement elem, IBaseElement? parent, IElementFactory elementFactory)
            : base(elem, parent, elementFactory) { }

        internal void ProcessTask(ITask task, ProcessTask processScriptTask, ILogger? logger)
        {
            ExtensionElement?.Extensions
                .OfType<AScript>()
                .FirstOrDefault()?.Invoke(task.Variables,logger);
            processScriptTask?.Invoke(task);
        }
    }
}
