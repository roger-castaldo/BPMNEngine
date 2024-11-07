using BPMNEngine.Elements.Processes.Events.Definitions;
using BPMNEngine.Extensions.Condition;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Extensions.Scripts
{
    internal abstract record AScript(XmlElement Element,IBaseElement? Parent) : AExtension(Element,Parent), ITaskExtensionElementElement,
        IStepElementStartCheckExtensionElement
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
                var parent = Parent;
                while (parent!=null)
                {
                    if (parent is ConditionSet)
                        return true;
                    parent = parent.Parent;
                }
                return false;
            }
        }

        protected bool IsTimerEvent
        {
            get
            {
                var parent = Parent;
                while (parent!=null)
                {
                    if (parent is TimerEventDefinition)
                        return true;
                    parent = parent.Parent;
                }
                return false;
            }
        }

        protected abstract void ScriptInvoke<T>(T variables, ILogger? logger, out object? result) where T : IVariablesContainer;
        protected abstract bool ScriptIsValid(ILogger? logger, out IEnumerable<string>? err);

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

        public override (bool isValid, IEnumerable<string> errors) IsValid(ILogger? logger)
        {
            var isValid = true;
            IEnumerable<string> errors = [];
            if (!ScriptIsValid(logger, out var errs))
            {
                errors = errors.Concat(errs?? []);
                isValid=false;
            }
            return (isValid, errors);
        }

        public async ValueTask<bool> ExecuteTaskExtensionAsync(ITask task)
        {
            task.Logger.LogInformation("Attempting to process script");
            try
            {
                ScriptInvoke<IVariables>(task.Variables, task.Logger, out _);
            }
            catch (Exception e)
            {
                task.Logger.LogError(e, "An error occured attempting to invoke the script");
                await task.EmitErrorAsync(e);
                return false;
            }
            return true;
        }

        public ValueTask<bool> IsElementStartValidAsync(IReadonlyVariables variables, IElement owningElement, ILogger? logger)
            => ValueTask.FromResult((bool)Invoke(variables, logger));
    }
}
