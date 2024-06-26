using System.Collections;
using System.Collections.Immutable;
using System.Text.Json;
using System.Windows.Markup;
using BPMNEngine.Drawing.Icons.IconParts;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.State.Objects;

namespace BPMNEngine.State
{
    internal class ProcessVariables(StateLock stateLock, int? stepIndex = null) : IStateContainer
    {
        internal record ReadOnlyProcessVariables : IReadOnlyStateVariablesContainer
        {
            private readonly int stepIndex;
            private readonly ProcessVariables processVariables;

            public ReadOnlyProcessVariables(ProcessVariables processVariables, int stepIndex)
            {
                this.processVariables = processVariables;
                this.stepIndex=stepIndex;
            }

            public object this[string name]
                => processVariables[name, stepIndex];

            public IImmutableList<string> Keys
                => processVariables[stepIndex];

            public IImmutableDictionary<string, object> AsExtract {
                get
                {
                    Dictionary<string, object> ret = new();
                    Keys.ForEach(key =>
                    {
                        ret.Add(key, this[key]);
                    });
                    return ret.ToImmutableDictionary();
                }
            }

            private ImmutableArray<ProcessVariable> Variables
            {
                get
                {
                    processVariables.stateLock.EnterReadLock();
                    var result = processVariables.variables
                        .Where(variable => variable.StepIndex==stepIndex)
                        .OrderBy(variable => variable.StepIndex)
                        .ToImmutableArray();
                    processVariables.stateLock.ExitReadLock();
                    return result;
                }
            }

            public void Append(XmlWriter writer)
                => Variables.ForEach(variable => variable.Write(writer));

            public void Append(Utf8JsonWriter writer)
            {
                writer.WriteStartArray();
                Variables.ForEach(variable => variable.Write(writer));
                writer.WriteEndArray();
            }
        }

        private readonly StateLock stateLock = stateLock;

        private readonly List<ProcessVariable> variables = [];
        private bool disposedValue;

        #region IStateContainer
        public XmlReader Load(XmlReader reader, Version version)
        {
            variables.Clear();
            reader.Read();
            while (reader.NodeType!=XmlNodeType.EndElement)
            {
                var variable = new ProcessVariable();
                if (!variable.CanRead(reader, version))
                    break;
                reader=variable.Read(reader, version);
                variables.Add(variable);
            }
            return reader;
        }

        public Utf8JsonReader Load(Utf8JsonReader reader, Version version)
        {
            variables.Clear();
            reader.Read();
            if (reader.TokenType==JsonTokenType.StartArray)
                reader.Read();
            while (reader.TokenType!=JsonTokenType.EndArray)
            {
                var variable = new ProcessVariable();
                if (!variable.CanRead(reader, version))
                    break;
                reader=variable.Read(reader, version);
                variables.Add(variable);
            }
            return reader;
        }

        public IReadOnlyStateContainer Clone()
            => new ReadOnlyProcessVariables(this, stepIndex??(variables.Any() ? variables.Max(var => var.StepIndex) : -1));

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    variables.Clear();
                }
                disposedValue=true;
            }
        }


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private IEnumerable<T> RunQuery<T>(Func<IEnumerable<ProcessVariable>, IEnumerable<T>> filter)
        {
            stateLock.EnterReadLock();
            var results = filter(variables).ToImmutableArray();
            stateLock.ExitReadLock();
            return results;
        }

        public object this[string variableName, int stepIndex]
        {
            get {
                var lst = RunQuery((IEnumerable<ProcessVariable> variables) =>
                {
                    return variables
                    .Where(var => var.Name==variableName && var.StepIndex<=stepIndex)
                    .OrderBy(var => var.StepIndex);
                });
                return (lst.Any() ? lst.Last().Value : null);
            }
            set
            {
                stateLock.EnterWriteLock();
                variables.Add(new ProcessVariable(variableName, stepIndex, value));
                stateLock.ExitWriteLock();
            }
        }

        public IImmutableList<string> this[int stepIndex]
            => RunQuery((IEnumerable<ProcessVariable> variables) => {
                return variables
                    .Where(variable => variable.StepIndex <= stepIndex)
                    .OrderBy(var => var.StepIndex)
                    .GroupBy(variable => variable.Name)
                    .Where(grp => grp.Last().Value != null)
                    .Select(grp => grp.Key);
            }).ToImmutableList();

        internal void MergeVariables(int stepIndex, IVariables vars)
        {
            stateLock.EnterWriteLock();
            var changes = new Queue<ProcessVariable>();
            vars.Keys.ForEach(key =>
            {
                object left = vars[key];
                var right = (variables.Exists(var => var.Name==key && var.StepIndex <= stepIndex) ?
                    variables
                        .OrderBy(var => var.StepIndex)
                        .LastOrDefault(var => var.Name==key
                            && var.StepIndex <= stepIndex)
                    : null);
                if (!AreVariablesEqual(left, (right?.Value)))
                    changes.Enqueue(new ProcessVariable(key, stepIndex, left));
            });
            while (changes.Count>0)
                variables.Add(changes.Dequeue());
            stateLock.ExitWriteLock();
        }

        internal static bool AreVariablesEqual(object left, object right)
            => (left, right) switch
            {
                (null, not null) => false,
                (not null, null) => false,
                (null, null) => true,
                (Array, not Array) => false,
                (not Array, Array) => false,
                (Array aleft, Array aright) => aleft.Length==aright.Length
                    && aleft.Cast<object>().Select((v, i) => new { val = v, index = i }).All(ival => AreVariablesEqual(ival.val, aright.GetValue(ival.index))),
                _ => object.Equals(left, right)
            };
    }
}
