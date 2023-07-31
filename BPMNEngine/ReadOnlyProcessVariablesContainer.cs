using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine
{
    internal class ReadOnlyProcessVariablesContainer : IReadonlyVariables
    {
        private readonly IVariables variables;

        public object this[string name] { get => variables[name]; set => throw new Exception("Unable to change variable values in readonly process variables container."); }

        public IEnumerable<string> Keys => variables.Keys;

        public IEnumerable<string> FullKeys => variables.FullKeys;

        public Exception Error { get; private init; }

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessInstance instance)
            : this(elementID, instance,null) { }

        internal ReadOnlyProcessVariablesContainer(string elementID, ProcessInstance instance,Exception error)
        {
            variables = new ProcessVariablesContainer(elementID, instance);
            Error = error;
        }

        internal ReadOnlyProcessVariablesContainer(IVariables variables)
        {
            this.variables = variables;
        }
    }
}
