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
        public ScriptTask(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        internal void ProcessTask(ref ProcessVariablesContainer variables, ProcessScriptTask processScriptTask)
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
                            variables = (ProcessVariablesContainer)((AScript)ie).Invoke(variables);
                            break;
                        }
                    }
                }
            }
            processScriptTask(this, ref variables);
        }
    }
}
