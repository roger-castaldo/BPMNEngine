using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface IVariables
    {
        object this[string name] { get; set; }
        string[] Keys { get; }
        string[] FullKeys { get; }
    }
}
