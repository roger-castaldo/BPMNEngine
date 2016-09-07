using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes
{
    [XMLTag("bpmn", "extensionElements")]
    internal class ExtensionElements : AParentElement
    {
        public ExtensionElements(XmlElement elem)
            : base(elem) { }

        public bool IsInternalExtension
        {
            get
            {
                bool ret = false;
                if (SubNodes != null)
                {
                    int cnt = 0;
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            cnt++;
                        }
                    }
                    if (Children != null)
                        ret = cnt == Children.Length;
                }
                return ret;
            }
        }
    }
}
