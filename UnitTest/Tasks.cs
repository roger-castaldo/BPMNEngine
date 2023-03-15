using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class Tasks
    {
        private const string VARIABLE_NAME = "TestManualVariable";
        private const string VARIABLE_VALUE = "ManualVariableValue";

        private static void _StartManualTask(IManualTask task)
        {
            task.Variables[VARIABLE_NAME] = VARIABLE_VALUE;
            task.MarkComplete();
        }

        [TestMethod]
        public void TestBusinessRule()
        {
            var taskDelegateMock =new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessBusinessRuleTask=taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestManualTask()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count());
            Assert.IsTrue(variables.ContainsKey(VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[VARIABLE_NAME]);
        }

        [TestMethod]
        public void TestRecieveTask()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessRecieveTask =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestScriptTask()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessScriptTask =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestSendTask()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessSendTask =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestServiceTask()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessServiceTask =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestTask()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                ProcessTask =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }

        [TestMethod]
        public void TestCallActivity()
        {
            var taskDelegateMock = new Moq.Mock<ProcessTask>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Tasks/all_tasks.bpmn"), tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
            {
                CallActivity =taskDelegateMock.Object,
                BeginManualTask=new StartManualTask(_StartManualTask)
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(TimeSpan.FromSeconds(30)));
            taskDelegateMock.Verify(x => x.Invoke(It.IsAny<ITask>()), Times.Once());
        }
    }
}
