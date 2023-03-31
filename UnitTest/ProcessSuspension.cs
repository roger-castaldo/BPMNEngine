using Esprima;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
        public void TestUserTaskSuspension()
        {
            IProcessInstance instance = _userProcess.BeginProcess(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new XmlDocument();
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
        public void TestSuspensionWithActiveSteps()
        {
            IProcessInstance instance = null;
            XmlDocument doc = new XmlDocument();
            for (int cnt = 0; cnt<5; cnt++)
            {
                instance = _userProcess.BeginProcess(null);
                Assert.IsNotNull(instance);
                instance.Suspend();
                System.Threading.Thread.Sleep(1000);
                doc.LoadXml(instance.CurrentState.AsXMLDocument);
                instance.Dispose();

                if (doc.SelectSingleNode("/ProcessState/ProcessPath/sPathEntry[@status='Suspended']")!=null)
                    break;
            }

            Assert.IsNotNull(doc.SelectSingleNode("/ProcessState/ProcessPath/sPathEntry[@status='Suspended']"));

            instance = _userProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            System.Threading.Thread.Sleep(4*1000);
            IUserTask task = instance.GetUserTask("UserTask_07o8pvs");
            Assert.IsNotNull(task);
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public void TestTimerSuspension()
        {
            IProcessInstance instance = _timerProcess.BeginProcess(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public void TestTimerSuspensionDelayedResume()
        {
            IProcessInstance instance = _timerProcess.BeginProcess(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(30*1000);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }

        [TestMethod]
        public void TestNotSuspended()
        {
            IProcessInstance instance = _timerProcess.BeginProcess(null);
            Assert.IsNotNull(instance);
            System.Threading.Thread.Sleep(5*1000);
            instance.Suspend();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _timerProcess.LoadState(doc);
            Assert.IsNotNull(instance);
            Exception exception = null;
            instance.Resume();
            try
            {
                instance.Resume();
            }catch(Exception e)
            {
                exception=e;
            }
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsNotNull(exception);
            Assert.IsInstanceOfType(exception, typeof(NotSuspendedException));
        }
    }
}
