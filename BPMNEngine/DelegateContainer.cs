using BPMNEngine.DelegateContainers;

namespace BPMNEngine
{
    internal class DelegateContainer
    {
        public ProcessEvents Events { get; init; } = new ProcessEvents();
        public StepValidations Validations { get; init; } = new StepValidations();
        public ProcessTasks Tasks { get; init; } = new ProcessTasks();
        public ProcessLogging Logging { get; init; } = new ProcessLogging();
        public static DelegateContainer Merge(DelegateContainer original,DelegateContainer append)
        {
            if (original==null && append==null) return new DelegateContainer();
            if (original==null) return append;
            if (append==null) return original;
            return new DelegateContainer()
            {
                Logging = ProcessLogging.Merge(original.Logging,append.Logging)??new ProcessLogging(),
                Tasks = ProcessTasks.Merge(original.Tasks,append.Tasks)??new ProcessTasks(),
                Validations = StepValidations.Merge(original.Validations, append.Validations),
                Events = ProcessEvents.Merge(original.Events, append.Events) ?? new ProcessEvents()
            };
        }
    }
}
