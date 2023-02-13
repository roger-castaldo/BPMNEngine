using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements.Processes.Conditions;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Scripts
{
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(AConditionSet))]
    internal abstract class AScript : AElement
    {
        private readonly XmlPrefixMap _map;

        protected AScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            _map = map;
        }

        protected string _Code =>
            SubNodes
                .Where(n => n.NodeType==XmlNodeType.Text)
                .Select(n => n.InnerText)
                .FirstOrDefault()
            ??
            SubNodes
                .Where(n => n.NodeType==XmlNodeType.CDATA)
                .Select(n => ((XmlCDataSection)n).InnerText)
                .FirstOrDefault()
            ??
            String.Empty;

        protected bool _IsCondition
        {
            get
            {
                XMLTag[] tags = Utility.GetTagAttributes(typeof(ConditionSet));
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (tags.Any(xt => xt.Matches(_map, n.Name)))
                        return true;
                    n = n.ParentNode;
                }
                return false;
            }
        }

        protected bool _IsTimerEvent
        {
            get
            {
                XMLTag[] tags = Utility.GetTagAttributes(typeof(TimerEventDefinition));
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (tags.Any(xt => xt.Matches(_map, n.Name)))
                        return true;
                    n = n.ParentNode;
                }
                return false;
            }
        }

        protected abstract object _Invoke(IVariables variables);
        protected abstract object _Invoke(IReadonlyVariables variables);
        protected abstract bool _IsValid(out string[] err);

        public object Invoke(IVariables variables)
        {
            Info("Attempting to process script {0}", new object[] { id });
            try
            {
                return _Invoke(variables);
            }
            catch (Exception e) {
                Exception(e);
                throw;
            }
        }

        public object Invoke(IReadonlyVariables variables)
        {
            Info("Attempting to process script {0}", new object[] { id });
            try
            {
                return _Invoke(variables);
            }
            catch (Exception e)
            {
                Exception(e);
                throw;
            }
        }

        public sealed override bool IsValid(out string[] err)
        {
            if (!_IsValid(out err))
                return false;
            return base.IsValid(out err);
        }
    }
}
