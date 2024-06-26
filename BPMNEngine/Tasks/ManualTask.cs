using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Tasks;

namespace BPMNEngine.Tasks
{
    internal record ManualTask : ExternalTask, IManualTask
    {
        public ManualTask(ATask task, ProcessVariablesContainer variables, ProcessInstance process) 
            : base(task, variables, process) { }

        public void MarkComplete()
            => businessProcess.CompleteTask(this);
    }
}
