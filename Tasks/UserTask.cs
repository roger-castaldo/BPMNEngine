using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Tasks
{
    internal class UserTask : ManualTask, IUserTask
    {
        public UserTask(Elements.Processes.Tasks.ATask task, ProcessVariablesContainer variables, BusinessProcess process) :
            base(task, variables, process)
        {
        }

        public string UserID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
