using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Scripts;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
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
            if (ExtensionElement != null)
            {
                ExtensionElements ee = (ExtensionElements)ExtensionElement;
                if (ee.Children != null)
                {
                    foreach (IElement ie in ee.Children)
                    {
                        if (ie is AScript)
                        {
                            IVariables vars = (ProcessVariablesContainer)((AScript)ie).Invoke(task.Variables);
                            foreach (string str in vars.Keys)
                               ((IVariables)task)[str]=vars[str];
                            break;
                        }
                    }
                }
            }
            if (processScriptTask!=null)
                processScriptTask(task);
        }
    }
}
