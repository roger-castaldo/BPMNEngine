using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks.Scripts;
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
            bool callDelegate = true;
            if (ExtensionElement != null)
            {
                if (((ExtensionElements)ExtensionElement).IsInternalExtension)
                {
                    ExtensionElements ee = (ExtensionElements)ExtensionElement;
                    if (ee.Children != null)
                    {
                        if (ee.Children[0] is AScript)
                        {
                            callDelegate = false;
                            ((AScript)ee.Children[0]).Invoke(ref variables);
                        }
                    }
                }
            }
            if (callDelegate)
                processScriptTask(this, ref variables);
        }
    }
}
