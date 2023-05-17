using BpmEngine.Elements.Processes.Tasks;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Tasks
{
    internal class ManualTask : ExternalTask, IManualTask
    {
        public ManualTask(ATask task, ProcessVariablesContainer variables, ProcessInstance process) :
            base(task, variables, process)
        { }

        public void MarkComplete()
        {
            _businessProcess.CompleteTask(this);
        }
    }
}
