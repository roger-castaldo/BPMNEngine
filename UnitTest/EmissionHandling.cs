using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class EmissionHandling
    {
        private static BusinessProcess _messageProcess;
        private static BusinessProcess _signalProcess;
        private static BusinessProcess _escalateProcess;
        private static BusinessProcess _errorProcess;
        private static BusinessProcess _throwCatchProcess;

        private const string _ACTION_ID = "Action";
        private const string _VALUE_ID = "ActionValue";


        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _messageProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/messages.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask=new ProcessTask(ProcessTask) });
            _signalProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/signals.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask = new ProcessTask(ProcessTask) });
            _escalateProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/escalations.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask = new ProcessTask(ProcessTask) });
            _errorProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/errors.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask = new ProcessTask(ProcessTask) });
            _throwCatchProcess = new BusinessProcess(Utility.LoadResourceDocument("EmissionHandling/throw_catching.bpmn"));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _messageProcess.Dispose();
            _signalProcess.Dispose();
            _escalateProcess.Dispose();
            _errorProcess.Dispose();
            _throwCatchProcess.Dispose();
        }

        private static void ProcessTask(ITask task)
        {
            if (task.Variables[_ACTION_ID]!=null)
            {
                switch ((string)task.Variables[_ACTION_ID])
                {
                    case "Message":
                        task.EmitMessageAsync((string)task.Variables[_VALUE_ID]);
                        break;
                    case "Signal":
                        task.SignalAsync((string)task.Variables[_VALUE_ID]);
                        break;
                    case "Escalate":
                        if (task.ID == (string)task.Variables[_VALUE_ID])
                            task.EscalateAsync();
                        break;
                    case "Error":
                        task.EmitErrorAsync(new System.Exception((string)task.Variables[_VALUE_ID]));
                        break;
                }
            }
        }

        [TestMethod]
        public async ValueTask TestIntermediateCatchMessage()
        {
            IProcessInstance instance = await _messageProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Message" },
                {_VALUE_ID,"external_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "IntermediateCatchEvent_1rztezm"));
        }

        [TestMethod]
        public async ValueTask TestNonInteruptingBoundaryMessage()
        {
            IProcessInstance instance = await _messageProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Message" },
                {_VALUE_ID,"non_interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_0cezzjh"));
        }

        [TestMethod]
        public async ValueTask TestInteruptingBoundaryMessage()
        {
            IProcessInstance instance = await _messageProcess.BeginProcessAsync(new Dictionary<string, object>()
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
        public async ValueTask TestIntermediateCatchSignal()
        {
            IProcessInstance instance = await _signalProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Signal" },
                {_VALUE_ID,"external_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "IntermediateCatchEvent_0ms7d2m"));
        }

        [TestMethod]
        public async ValueTask TestNonInteruptingBoundarySignal()
        {
            IProcessInstance instance = await _signalProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Signal" },
                {_VALUE_ID,"non_interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1sdut64"));
        }

        [TestMethod]
        public async ValueTask TestInteruptingBoundarySignal()
        {
            IProcessInstance instance = await _signalProcess.BeginProcessAsync(new Dictionary<string, object>()
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
        public async ValueTask TestNonInteruptingBoundaryEscalation()
        {
            IProcessInstance instance = await _escalateProcess.BeginProcessAsync(new Dictionary<string, object>()
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
        public async ValueTask TestInteruptingBoundaryEscalation()
        {
            IProcessInstance instance = await _escalateProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Escalate" },
                {_VALUE_ID,"Task_0peqa8k" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1sr23zw"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
        }

        [TestMethod]
        public async ValueTask TestInteruptingBoundaryError()
        {
            IProcessInstance instance = await _errorProcess.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_ACTION_ID,"Error" },
                {_VALUE_ID,"interupting_catch" }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_0cezzjh"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
        }

        private static readonly string[] ThrowCatchSteps = ["Event_0ms842d", "Event_03kuv0p", "Event_1p2py1q", "Event_176mo0n", "Event_0juephw", "Event_1p6l3ai"];

        [TestMethod]
        public async ValueTask TestThrowCatchInternal()
        {
            IProcessInstance instance = await _throwCatchProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            foreach (var step in ThrowCatchSteps)
                Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, step));
        }
    }
}
