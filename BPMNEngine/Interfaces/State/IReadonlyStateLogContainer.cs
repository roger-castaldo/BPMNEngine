using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Interfaces.State
{
    internal interface IReadonlyStateLogContainer : IReadOnlyStateContainer
    {
        string Log { get; }
    }
}
