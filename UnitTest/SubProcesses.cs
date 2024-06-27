using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class SubProcesses
    {
        private const string SIGNAL = "interupted";

        private const string SUB_PROCESS_ID = "Activity_01eulv2";
        private const string SUB_SUB_PROCESS_ID = "Activity_099a0io";

        private const string MAIN_SIGNAL_ID = "Event_167oagf";
        private const string SUB_SIGNAL_ID = "Event_1etf3o8";
        private const string SUB_SUB_SIGNAL_ID = "Event_0s8bx7i";

        private const string MAIN_TASK_ID = "Activity_1hle9w5";
        private const string SUB_TASK_ID = "Activity_1ifnuk7";
        private const string SUB_SUB_TASK_ID = "Activity_1e0arlx";

        private static BusinessProcess _process;

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("SubProcesses/subprocesses.bpmn"));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private static IProcessInstance StartProcess()
        {
            var instance = _process.BeginProcess();
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForManualTask(MAIN_TASK_ID, out _));
            Assert.IsTrue(instance.WaitForManualTask(SUB_TASK_ID, out _));
            Assert.IsTrue(instance.WaitForManualTask(SUB_SUB_TASK_ID, out _));
            return instance;
        }

        [TestMethod]
        public void TestSubProcessCompletion()
        {
            var instance = StartProcess();
            Assert.IsTrue(instance.WaitForManualTask(SUB_TASK_ID, out IManualTask task));
            Assert.IsNotNull(task);
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsTrue(Utility.StepCompleted(state, SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_SUB_TASK_ID));
        }

        [TestMethod]
        public void TestSubTaskSignal()
        {
            var instance = StartProcess();
            Assert.IsTrue(instance.WaitForManualTask(SUB_TASK_ID, out IManualTask task));
            Assert.IsNotNull(task);
            task.Signal(SIGNAL, out bool isAborted);
            Assert.IsTrue(isAborted);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsTrue(Utility.StepAborted(state, SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepCompleted(state, SUB_SIGNAL_ID));
            Assert.IsFalse(Utility.StepCompleted(state, SUB_SUB_SIGNAL_ID));
            Assert.IsFalse(Utility.StepCompleted(state, MAIN_SIGNAL_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_PROCESS_ID));
        }

        [TestMethod]
        public void TestSubSubProcessCompletion()
        {
            var instance = StartProcess();
            Assert.IsTrue(instance.WaitForManualTask(SUB_SUB_TASK_ID, out IManualTask task));
            Assert.IsNotNull(task);
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsTrue(Utility.StepCompleted(state, SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepCompleted(state, SUB_SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_TASK_ID));
        }

        [TestMethod]
        public void TestSubSubTaskSignal()
        {
            var instance = StartProcess();
            Assert.IsTrue(instance.WaitForManualTask(SUB_SUB_TASK_ID, out IManualTask task));
            Assert.IsNotNull(task);
            task.Signal(SIGNAL, out bool isAborted);
            Assert.IsTrue(isAborted);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsTrue(Utility.StepCompleted(state, SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepCompleted(state, SUB_SUB_SIGNAL_ID));
            Assert.IsFalse(Utility.StepCompleted(state, SUB_SIGNAL_ID));
            Assert.IsFalse(Utility.StepCompleted(state, MAIN_SIGNAL_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_SUB_PROCESS_ID));
            Assert.IsTrue(Utility.StepAborted(state, SUB_SUB_TASK_ID));
        }

        [TestMethod]
        public void TestMainTaskSignal()
        {
            var instance = StartProcess();
            Assert.IsTrue(instance.WaitForManualTask(MAIN_TASK_ID, out IManualTask task));
            Assert.IsNotNull(task);
            task.Signal(SIGNAL, out bool isAborted);
            Assert.IsFalse(isAborted);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsFalse(Utility.StepCompleted(state, SUB_PROCESS_ID));
            Assert.IsFalse(Utility.StepCompleted(state, SUB_SUB_SIGNAL_ID));
            Assert.IsFalse(Utility.StepCompleted(state, SUB_SIGNAL_ID));
            Assert.IsTrue(Utility.StepCompleted(state, MAIN_SIGNAL_ID));
        }
    }
}
