using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;
using System.Collections.Immutable;

namespace BPMNEngine.Tasks
{
    internal record ExternalTask : ITask
    {
        private readonly ATask task;
        protected readonly ProcessInstance businessProcess;
        public bool Aborted { get; private set; }
        public IVariables Variables { get; private init; }
        public ILogger Logger => businessProcess.GetLogger(task);

        public ExternalTask(ATask task, ProcessVariablesContainer variables, ProcessInstance process)
        {
            this.task=task;
            Variables=variables;
            businessProcess=process;
        }

        #region IElement
        string? IElement.this[string attributeName]
            => task[attributeName];

        public IElement? Process
            => task.Process;

        public IElement? SubProcess
            => task.SubProcess;

        public IElement? Lane
            => task.Lane;

        public string ID
            => task.ID;

        public ImmutableArray<XmlNode> SubNodes
            => task.SubNodes;

        public IExtensionsElement? ExtensionElement
            => task.ExtensionElement;

        public IBaseElement? Parent
            => task.Parent;

        public XmlElement Element
            => task.Element;
        #endregion

        private async ValueTask<bool> ProcessEvent(Func<ValueTask<bool>> invocation)
        {
            var isAborted = await invocation();
            Aborted=Aborted||isAborted;
            return isAborted;
        }

        public ValueTask<bool> EmitErrorAsync(Exception error)
            => ProcessEvent(() => businessProcess.EmitTaskErrorAsync(this, error));

        public ValueTask<bool> EmitMessageAsync(string message)
            => ProcessEvent(() => businessProcess.EmitTaskMessageAsync(this, message));


        public ValueTask<bool> EscalateAsync()
            => ProcessEvent(() => businessProcess.EscalateTaskAsync(this));

        public  ValueTask<bool> SignalAsync(string signal)
            => ProcessEvent(() => businessProcess.EmitTaskSignalAsync(this, signal));
    }
}
