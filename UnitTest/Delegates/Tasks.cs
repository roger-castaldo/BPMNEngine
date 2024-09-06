using BPMNEngine;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnitTest.Delegates
{
    [TestClass]
    public class Tasks
    {
        private const string VARIABLE_NAME = "TestManualVariable";
        private const string INSTANCE_VARIABLE_NAME = "TestInstanceManualVariable";
        private const string VARIABLE_VALUE = "ManualVariableValue";

        private static void StartManualTask(IManualTask task)
        {
            task.Variables[VARIABLE_NAME] = VARIABLE_VALUE;
            task.MarkComplete();
        }

        private static void StartInstanceManualTask(IManualTask task)
        {
            task.Variables[INSTANCE_VARIABLE_NAME] = VARIABLE_VALUE;
            task.MarkComplete();
        }

        [TestMethod]
        public async ValueTask TestBusinessRule()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessBusinessRuleTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestBusinessRuleInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessBusinessRuleTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessBusinessRuleTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestManualTask()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        [TestMethod]
        public async ValueTask TestManualTaskInstanceOverride()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { },
                tasks: new()
                {
                    BeginManualTask=new StartManualTask(StartInstanceManualTask)
                });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(INSTANCE_VARIABLE_NAME));
            Assert.IsFalse(variables.ContainsKey(VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[INSTANCE_VARIABLE_NAME]);
        }

        [TestMethod]
        public async ValueTask TestReceiveTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessReceiveTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestReceiveTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessReceiveTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessReceiveTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestScriptTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessScriptTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestScriptTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessScriptTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessScriptTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestSendTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessSendTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestSendTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessSendTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessSendTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestServiceTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessServiceTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestServiceTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessServiceTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessServiceTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestCallActivity()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                CallActivity = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public async ValueTask TestCallActivityInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                CallActivity = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, tasks: new()
            {
                CallActivity = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }
    }
}
