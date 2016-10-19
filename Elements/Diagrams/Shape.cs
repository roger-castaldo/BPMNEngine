using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using Org.Reddragonit.BpmEngine.Elements.Processes.Tasks;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    [XMLTag("bpmndi","BPMNShape")]
    [RequiredAttribute("id")]
    internal class Shape : ADiagramElement
    {
        public RectangleF Rectangle
        {
            get
            {
                foreach (IElement elem in Children)
                {
                    if (elem is Bounds)
                        return ((Bounds)elem).Rectangle;
                }
                return new Rectangle(0,0,0,0);
            }
        }

        public Label Label
        {
            get
            {
                foreach (IElement elem in Children)
                {
                    if (elem is Label)
                        return (Label)elem;
                }
                return null;
            }
        }

        public Shape(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        public BPMIcons? GetIcon(Definition definition)
        {
            BPMIcons? ret = null;
            IElement elem = _GetLinkedElement(definition);
            if (elem != null)
            {
                if (elem is AEvent)
                {
                    AEvent evnt = (AEvent)elem;
                    if (elem is StartEvent)
                    {
                        ret = BPMIcons.StartEvent;
                        if (evnt.SubType.HasValue)
                        {
                            switch (evnt.SubType.Value)
                            {
                                case EventSubTypes.Message:
                                    ret = BPMIcons.MessageStartEvent;
                                    break;
                            }
                        }
                    }
                    else if (elem is IntermediateThrowEvent)
                    {
                        if (evnt.SubType.HasValue)
                        {
                            switch (evnt.SubType.Value)
                            {
                                case EventSubTypes.Message:
                                    ret = BPMIcons.MessageIntermediateThrowEvent;
                                    break;
                            }
                        }
                    }
                    else if (elem is EndEvent)
                    {
                        ret = BPMIcons.EndEvent;
                    }
                }
                else if (elem is AGateway)
                    ret = (BPMIcons)Enum.Parse(typeof(BPMIcons), elem.GetType().Name);
                else if (elem is ATask)
                    ret = (BPMIcons)Enum.Parse(typeof(BPMIcons), elem.GetType().Name);
            }
            return ret;
        }

        public override bool IsValid(out string err)
        {
            bool found = false;
            foreach (IElement elem in Children)
            {
                found = found | (elem is Bounds);
            }
            if (!found)
            {
                err = "No bounds specified.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
