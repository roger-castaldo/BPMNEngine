using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface ITask : IVariables, IStepElement
    {
        void Signal(string signal);
        void Escalate();
        void EmitMessage(string message);

        void EmitError(Exception error);
    }
}
