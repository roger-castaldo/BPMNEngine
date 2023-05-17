using BpmEngine.Attributes;
using BpmEngine.Elements.Processes.Scripts;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace BpmEngine.Elements.Processes.Tasks
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
                {
                    IVariables vars = (ProcessVariablesContainer)script.Invoke(task.Variables);
                    vars.Keys.ToArray().ForEach(str => task.Variables[str]=vars[str]);
                }
            }
            if (processScriptTask!=null)
                processScriptTask(task);
        }
    }
}
