﻿using BPMNEngine;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

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
        public void TestBusinessRule()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessBusinessRuleTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestBusinessRuleInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessBusinessRuleTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessBusinessRuleTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestManualTask()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        [TestMethod]
        public void TestManualTaskInstanceOverride()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { },
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
        public void TestReceiveTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessReceiveTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestReceiveTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessReceiveTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessReceiveTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestScriptTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessScriptTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestScriptTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessScriptTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessScriptTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestSendTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessSendTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestSendTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessSendTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessSendTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestServiceTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessServiceTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestServiceTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessServiceTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessServiceTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestTask()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessTask = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestTaskInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                ProcessTask = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
            {
                ProcessTask = taskInstanceDelegateMock.Object
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskProcessDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Never());
            taskInstanceDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestCallActivity()
        {
            var taskDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                CallActivity = taskDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestCallActivityInstanceOverride()
        {
            var taskProcessDelegateMock = new Mock<ProcessTask>();
            var taskInstanceDelegateMock = new Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                CallActivity = taskProcessDelegateMock.Object,
                BeginManualTask = new StartManualTask(StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { }, tasks: new()
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
