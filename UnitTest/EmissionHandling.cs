using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class EmissionHandling
    {
        private static BusinessProcess _messageProcess;
        private static BusinessProcess _signalProcess;
        private static BusinessProcess _escalateProcess;

        private const string _ACTION_ID = "Action";
        private const string _VALUE_ID = "ActionValue";


        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _messageProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/messages.bpmn"), tasks:new BPMNEngine.DelegateContainers.ProcessTasks(){ProcessTask=new ProcessTask(_ProcessTask)});
            _signalProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/signals.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask = new ProcessTask(_ProcessTask) });
            _escalateProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/escalations.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask = new ProcessTask(_ProcessTask) });
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _messageProcess.Dispose();
            _signalProcess.Dispose();
            _escalateProcess.Dispose();
        }

        private static void _ProcessTask(ITask task)
        {
            if (task.Variables[_ACTION_ID]!=null)
            {
                bool aborted;
                switch ((string)task.Variables[_ACTION_ID])
                {
                    case "Message":
                        task.EmitMessage((string)task.Variables[_VALUE_ID], out aborted);
                        break;
                    case "Signal":
                        task.Signal((string)task.Variables[_VALUE_ID], out aborted);
                        break;
                    case "Escalate":
                        if (task.id == (string)task.Variables[_VALUE_ID])
                            task.Escalate(out aborted);
                        break;
                }
            }
        }

        [TestMethod]
        public void TestIntermediateCatchMessage()
        {
            IProcessInstance instance = _messageProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Message" },
                {_VALUE_ID,"external_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState,"IntermediateCatchEvent_1rztezm"));
        }

        [TestMethod]
        public void TestNonInteruptingBoundaryMessage()
        {
            IProcessInstance instance = _messageProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Message" },
                {_VALUE_ID,"non_interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_0cezzjh"));
        }

        [TestMethod]
        public void TestInteruptingBoundaryMessage()
        {
            IProcessInstance instance = _messageProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Message" },
                {_VALUE_ID,"interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1qem8ws"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
        }

        [TestMethod]
        public void TestIntermediateCatchSignal()
        {
            IProcessInstance instance = _signalProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Signal" },
                {_VALUE_ID,"external_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "IntermediateCatchEvent_0ms7d2m"));
        }

        [TestMethod]
        public void TestNonInteruptingBoundarySignal()
        {
            IProcessInstance instance = _signalProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Signal" },
                {_VALUE_ID,"non_interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1sdut64"));
        }

        [TestMethod]
        public void TestInteruptingBoundarySignal()
        {
            IProcessInstance instance = _signalProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Signal" },
                {_VALUE_ID,"interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1bbj59j"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
        }

        [TestMethod]
        public void TestNonInteruptingBoundaryEscalation()
        {
            IProcessInstance instance = _escalateProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Escalate" },
                {_VALUE_ID,"Task_1pr3o3s" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_1pr3o3s"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_0zk6tzw"));
        }

        [TestMethod]
        public void TestInteruptingBoundaryEscalation()
        {
            IProcessInstance instance = _escalateProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Escalate" },
                {_VALUE_ID,"Task_0peqa8k" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1sr23zw"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
        }
    }
}
