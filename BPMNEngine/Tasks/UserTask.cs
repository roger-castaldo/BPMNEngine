using BPMNEngine.Interfaces.Tasks;

namespace BPMNEngine.Tasks
{
    internal record UserTask : ManualTask, IUserTask
    {
        public UserTask(Elements.Processes.Tasks.ATask task, ProcessVariablesContainer variables, ProcessInstance process) 
            : base(task, variables, process) { }

        public string UserID { get; set; }
    }
}
