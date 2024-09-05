using BPMNEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest.Extensions
{
    [TestClass]
    public class SubProcessConditions
    {
        private const string SUB_PROCESS_1_ID = "Activity_1rfycz6";
        private const string SUB_PROCESS_2_ID = "Activity_1u54apq";

        private static BusinessProcess _process;

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/sub_process_start_conditions.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private static void RunProcess(int subID, string completedID, string notRunID)
        {
            var instance = _process.BeginProcess(new Dictionary<string, object>() { { "sub_process_id", subID } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var state = instance.CurrentState;
            Assert.IsNotNull(state);
            Assert.IsTrue(Utility.StepCompleted(state, completedID));
            Assert.IsFalse(Utility.StepCompleted(state, notRunID));
        }

        [TestMethod]
        public void TestSubProcessStartCondition()
        {
            RunProcess(1, SUB_PROCESS_1_ID, SUB_PROCESS_2_ID);
            RunProcess(2, SUB_PROCESS_2_ID, SUB_PROCESS_1_ID);
        }
    }
}
