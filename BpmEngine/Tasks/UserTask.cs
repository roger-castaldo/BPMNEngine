using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Tasks
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
