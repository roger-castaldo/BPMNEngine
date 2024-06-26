using BPMNEngine.Attributes;
using BPMNEngine.Elements.Processes.Conditions;
using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Elements.Processes.Scripts
{
    [ValidParent(typeof(ExtensionElements))]
    [ValidParent(typeof(AConditionSet))]
    internal abstract record AScript : AElement
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
                XMLTagAttribute[] tags = Utility.GetTagAttributes(typeof(ConditionSet));
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (Array.Exists(tags,xt => xt.Matches(_map, n.Name)))
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
                XMLTagAttribute[] tags = Utility.GetTagAttributes(typeof(TimerEventDefinition));
                XmlNode n = Element.ParentNode;
                while (n != null)
                {
                    if (Array.Exists(tags, xt => xt.Matches(_map, n.Name)))
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

        protected abstract void ScriptInvoke<T>(T variables, out object result) where T : IVariablesContainer;
        protected abstract bool ScriptIsValid(out IEnumerable<string> err);

        public void Invoke(IVariables variables)
        {
            Info("Attempting to process script {0}", ID);
            try
            {
                ScriptInvoke<IVariables>(variables,out _);
            }
            catch (Exception e) {
                Exception(e);
                throw;
            }
        }

        public object Invoke(IReadonlyVariables variables)
        {
            Info("Attempting to process script {0}", ID);
            try
            {
                ScriptInvoke<IReadonlyVariables>(variables,out object result);
                return result;
            }
            catch (Exception e)
            {
                Exception(e);
                throw;
            }
        }

        public sealed override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!ScriptIsValid(out IEnumerable<string> errs))
            {
                err=(err?? []).Concat(errs);
                res=false;
            }
            return res;
        }
    }
}
