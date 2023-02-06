using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Scripts;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Tasks
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
                var script = (AScript)((ExtensionElements)ExtensionElement)
                    .Children
                    .Where(ie => ie is AScript)
                    .FirstOrDefault();
                if (script != null)
                {
                    IVariables vars = (ProcessVariablesContainer)script.Invoke(task.Variables);
                    foreach (string str in vars.Keys)
                        task.Variables[str]=vars[str];
                }
            }
            if (processScriptTask!=null)
                processScriptTask(task);
        }
    }
}
