﻿using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Tasks
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