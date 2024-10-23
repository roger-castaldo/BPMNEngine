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
                    if (Array.Exists(tags, xt => xt.Matches(_map, n.Name)))
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

        protected abstract void ScriptInvoke<T>(T variables, ILogger? logger, out object result) where T : IVariablesContainer;
        protected abstract bool ScriptIsValid(ILogger? logger, out IEnumerable<string> err);

        public void Invoke(IVariables variables, ILogger? logger)
        {
            logger?.LogInformation("Attempting to process script");
            try
            {
                ScriptInvoke<IVariables>(variables, logger, out _);
            }
            catch (Exception e)
            {
                logger?.LogError(e, "An error occured attempting to invoke the script");
                throw;
            }
        }

        public object Invoke(IReadonlyVariables variables, ILogger? logger)
        {
            logger?.LogInformation("Attempting to process script");
            try
            {
                ScriptInvoke<IReadonlyVariables>(variables, logger, out object result);
                return result;
            }
            catch (Exception e)
            {
                logger?.LogError(e, "An error occured attempting to invoke the script");
                throw;
            }
        }

        public sealed override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            (var isValid, var errors) = base.IsValid(logger);
            if (!ScriptIsValid(logger, out IEnumerable<string> errs))
            {
                errors = errors.Concat(errs);
                isValid=false;
            }
            return (isValid, errors);
        }
    }
}
