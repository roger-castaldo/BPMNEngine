using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Interfaces.State
{
    internal interface IInternalState : IState
    {
        Dictionary<string, object> Variables { get; }
        IEnumerable<IStateStep> Steps {get;}
        string Log { get; }
    }
}
