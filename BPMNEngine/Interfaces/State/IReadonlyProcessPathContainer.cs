using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Interfaces.State
{
    internal interface IReadonlyProcessPathContainer : IReadOnlyStateContainer
    {
        IEnumerable<string> ActiveSteps { get; }
    }
}
