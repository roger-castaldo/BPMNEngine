using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface IReadonlyVariables : IVariables
    {
        
        new object this[string name] { get; } 
        
        Exception Error { get; }

        void WriteLogLine(LogLevels level, string message);
    }
}
