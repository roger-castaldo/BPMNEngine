using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    public interface IManualTask : ITask
    {
        void MarkComplete();
    }
}
