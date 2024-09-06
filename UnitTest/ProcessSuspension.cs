using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class ProcessSuspension
    {
        private static BusinessProcess _userProcess;
        private static BusinessProcess _timerProcess;


        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _userProcess = new BusinessProcess(Utility.LoadResourceDocument("ProcessSuspension/user_task.bpmn"));
            _timerProcess = new BusinessProcess(Utility.LoadResourceDocument("ProcessSuspension/timer.bpmn"));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _userProcess.Dispose();
            _timerProcess.Dispose();
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestUserTaskSuspension()
        {
            IProcessInstance instance = await _userProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _userProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            IUserTask task = instance.GetUserTask("UserTask_07o8pvs");
            Assert.IsNotNull(task);
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestSuspensionWithActiveSteps()
        {
            IProcessInstance instance;
            XmlDocument doc = new();
            for (int cnt = 0; cnt<5; cnt++)
            {
                instance = await _userProcess.BeginProcessAsync(null);
                Assert.IsNotNull(instance);
                instance.Suspend();
                System.Threading.Thread.Sleep(1000);
                doc.LoadXml(instance.CurrentState.AsXMLDocument);
                instance.Dispose();

                if (doc.SelectSingleNode("/ProcessState/ProcessPath/StateStep[@status='Suspended']")!=null)
                    break;
            }

            Assert.IsNotNull(doc.SelectSingleNode("/ProcessState/ProcessPath/StateStep[@status='Suspended']"));

            instance = _userProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            Assert.IsTrue(instance.WaitForUserTask(TimeSpan.FromSeconds(5), "UserTask_07o8pvs", out IUserTask task));
            Assert.IsNotNull(task);
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestTimerSuspension()
        {
            IProcessInstance instance = await _timerProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestTimerSuspensionDelayedResume()
        {
            IProcessInstance instance = await _timerProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(30*1000);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestNotSuspended()
        {
            IProcessInstance instance = await _timerProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            Exception exception = null;
            instance.Resume();
            try
            {
                instance.Resume();
            }
            catch (Exception e)
            {
                exception=e;
            }
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(NotSuspendedException));
        }
    }
}
