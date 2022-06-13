using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("Timers/timing.bpmn"), processTask: new ProcessTask(_ProcessTask));
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private static void _ProcessTask(ITask task)
        {
            if (task.Variables[_DELAY_ID]!=null)
                System.Threading.Thread.Sleep((int)task.Variables[_DELAY_ID]);
        }

        private static TimeSpan? GetStepDuration(XmlDocument state,string name)
        {
            DateTime? start = null;
            XmlNode node = state.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='WaitingStart']", name));
            if (node!=null)
            {
                start = DateTime.ParseExact(node.Attributes["startTime"].Value, DATETIME_FORMAT, CultureInfo.InvariantCulture);
            }
            node = state.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='Succeeded']", name));
            if (node!=null)
            {
                return DateTime.ParseExact(node.Attributes["endTime"].Value, DATETIME_FORMAT, CultureInfo.InvariantCulture).Subtract((start.HasValue ? start.Value : DateTime.ParseExact(node.Attributes["startTime"].Value, DATETIME_FORMAT, CultureInfo.InvariantCulture)));
            }
            return null;
        }

        [TestMethod]
        public void TestCatchTimerDefinition()
        {
            IProcessInstance instance = _process.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(5*60*1000));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "IntermediateCatchEvent_1tm2fi4");
            Assert.IsNotNull(ts);
            Assert.AreEqual(60, (int)Math.Floor(ts.Value.TotalSeconds));
        }


        [TestMethod]
        public void TestNonInteruptingTimerDefinition()
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>(){
                { _DELAY_ID,(int)31*1000}
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(5*60*1000));
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
            Assert.IsTrue(instance.WaitForCompletion(5*60*1000));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_1s99zkc"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_14c0tw1"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "Task_0peqa8k"));
            TimeSpan? ts = GetStepDuration(instance.CurrentState, "BoundaryEvent_14c0tw1");
            Assert.IsNotNull(ts);
            Assert.AreEqual(45, (int)Math.Floor(ts.Value.TotalSeconds));
        }
    }
}
