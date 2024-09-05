using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class Timers
    {
        private static BusinessProcess _process;

        private const string _DELAY_ID = "TaskDelayMS";
        private const string DATETIME_FORMAT = "yyyyMMddHHmmssfff";


        [ClassInitialize()]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Initialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("Timers/timing.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
            {
                BeginUserTask= new StartUserTask(BeginUserTask)
            });
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private static void BeginUserTask(IUserTask task)
        {
            if (task.Variables[_DELAY_ID]!=null)
            {
                Task.Delay((int)task.Variables[_DELAY_ID]).ContinueWith(_ =>
                {
                    task.MarkComplete();
                });
            }
            else
                task.MarkComplete();
        }

        private static TimeSpan? GetStepDuration(IState state, string name)
        {
            XmlDocument xml = new();
            xml.LoadXml(state.AsXMLDocument);
            XmlNode node = xml.SelectSingleNode(string.Format("/ProcessState/ProcessPath/StateStep[@elementID='{0}'][@status='Succeeded']", name));
            if (node!=null)
            {
                return DateTime.ParseExact(node.Attributes["endTime"].Value, DATETIME_FORMAT, CultureInfo.InvariantCulture).Subtract((((DateTime?)null)??DateTime.ParseExact(node.Attributes["startTime"].Value, DATETIME_FORMAT, CultureInfo.InvariantCulture)));
            }
            return null;
        }

        [TestMethod]
        public void TestCatchTimerDefinition()
        {
            IProcessInstance instance = _process.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "IntermediateCatchEvent_1tm2fi4");
            Assert.IsNotNull(ts);
            Assert.AreEqual(60, (int)Math.Floor(ts.Value.TotalSeconds));
        }

        [TestMethod]
        public void TestCatchTimerDefinitionWithSuspension()
        {
            var stateChangeMock = new Mock<OnStateChange>();
            IProcessInstance instance = _process.BeginProcess(null, events: new()
            {
                OnStateChange=stateChangeMock.Object
            });
            Assert.IsNotNull(instance);
            Task.Delay(1000).Wait();
            stateChangeMock.Reset();
            instance.Suspend();
            Task.Delay(5000).Wait();
            var doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _process.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "IntermediateCatchEvent_1tm2fi4");
            Assert.IsNotNull(ts);
            Assert.AreEqual(60, (int)Math.Floor(ts.Value.TotalSeconds));

            stateChangeMock.Verify(x => x.Invoke(It.IsAny<IState>()), Times.Once());
        }

        [TestMethod]
        public void TestCatchTimerDefinitionWithResumeAfterEnd()
        {
            IProcessInstance instance = _process.BeginProcess(null);
            Assert.IsNotNull(instance);
            Task.Delay(1000).Wait();
            instance.Suspend();
            Task.Delay(5000).Wait();
            var doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _process.LoadState(doc);
            Assert.IsNotNull(instance);
            Task.Delay(60000).Wait();
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "IntermediateCatchEvent_1tm2fi4");
            Assert.IsNotNull(ts);
            Assert.IsTrue((int)Math.Ceiling(ts.Value.TotalSeconds)>=60);
        }


        [TestMethod]
        public void TestNonInteruptingTimerDefinition()
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>(){
                { _DELAY_ID,(int)31*1000}
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "BoundaryEvent_1s99zkc");
            Assert.IsNotNull(ts);
            Assert.AreEqual(30, (int)Math.Floor(ts.Value.TotalSeconds));
        }

        [TestMethod]
        public void TestInteruptingTimerDefinition()
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>(){
                { _DELAY_ID,(int)46*1000}
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "BoundaryEvent_14c0tw1");
            Assert.IsNotNull(ts);
            Assert.AreEqual(45, (int)Math.Floor(ts.Value.TotalSeconds));
        }

        [TestMethod]
        public void TestDelayStartEventWithSuspension()
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>(){
                { _DELAY_ID,(int)46*1000}
            });
            Assert.IsNotNull(instance);
            Task.Delay(1000).Wait();
            instance.Suspend();
            Task.Delay(5000).Wait();
            var doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _process.LoadState(doc);
            Assert.IsNotNull(instance);
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "BoundaryEvent_14c0tw1");
            Assert.IsNotNull(ts);
            Assert.AreEqual(45, (int)Math.Floor(ts.Value.TotalSeconds));
        }

        [TestMethod]
        public void TestDelayStartEventWithResumeAfterStart()
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>(){
                { _DELAY_ID,(int)30*1000}
            });
            Assert.IsNotNull(instance);
            Task.Delay(1000).Wait();
            instance.Suspend();
            Task.Delay(5000).Wait();
            var doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            instance.Dispose();
            instance = _process.LoadState(doc);
            Assert.IsNotNull(instance);
            Task.Delay(30000).Wait();
            instance.Resume();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "BoundaryEvent_14c0tw1");
            Assert.IsNotNull(ts);
            Assert.IsTrue((int)Math.Ceiling(ts.Value.TotalSeconds)>=30);
        }
    }
}
