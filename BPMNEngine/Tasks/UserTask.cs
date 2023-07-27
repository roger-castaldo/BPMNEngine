using BPMNEngine.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Tasks
{
    internal class UserTask : ManualTask, IUserTask
    {
        public UserTask(Elements.Processes.Tasks.ATask task, ProcessVariablesContainer variables, ProcessInstance process) :
            base(task, variables, process)
        {
        }

        private string _userID=null;
        public string UserID { get { return _userID; } set { _userID=value; } }
    }
}
