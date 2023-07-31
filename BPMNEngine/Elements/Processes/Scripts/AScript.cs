using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Scripts
{
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(AConditionSet))]
    internal abstract class AScript : AElement
    {
        protected string Code =>
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
            string.Empty;

        protected bool IsCondition
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

        protected bool IsTimerEvent
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

        private readonly XmlPrefixMap _map;

        protected AScript(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent)
        {
            _map = map;
        }

        protected abstract object ScriptInvoke(IVariables variables);
        protected abstract object ScriptInvoke(IReadonlyVariables variables);
        protected abstract bool ScriptIsValid(out string[] err);

        public object Invoke(IVariables variables)
        {
            Info("Attempting to process script {0}", new object[] { ID });
            try
            {
                return ScriptInvoke(variables);
            }
            catch (Exception e) {
                Exception(e);
                throw;
            }
        }

        public object Invoke(IReadonlyVariables variables)
        {
            Info("Attempting to process script {0}", new object[] { ID });
            try
            {
                return ScriptInvoke(variables);
            }
            catch (Exception e)
            {
                Exception(e);
                throw;
            }
        }

        public sealed override bool IsValid(out string[] err)
        {
            if (!ScriptIsValid(out err))
                return false;
            return base.IsValid(out err);
        }
    }
}
