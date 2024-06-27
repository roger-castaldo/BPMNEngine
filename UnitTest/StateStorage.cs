using BPMNEngine;
using BPMNEngine.Interfaces.State;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class StateStorage
    {
        private static BusinessProcess _singleTaskProcess;
        private const string _TEST_VARIABLE_NAME = "TestValue";
        private const string _TEST_VARIABLE_VALUE = "This is a test";
        private const string _USER_ID = "User1";
        private const string _TEST_LOG_LINE = "Test Log Line";
        private const string _TEST_FILE_VARIABLE = "TestFile";
        private static readonly SFile[] _TEST_FILES =
        [
            new(){Name="Test1",Extension="txt",Content=[],ContentType="text/text"},
            new(){Name="Test2",Extension="txt",Content=[],ContentType="text/text"}
        ];

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Initialize(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            _singleTaskProcess = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"),
                tasks: new()
                {
                    BeginUserTask=(IUserTask task) =>
                    {
                        Task.Delay(TimeSpan.FromSeconds(2)).Wait();
                        task.Debug(_TEST_LOG_LINE);
                        task.Variables[_TEST_VARIABLE_NAME] = _TEST_VARIABLE_VALUE;
                        task.Variables[_TEST_FILE_VARIABLE]=_TEST_FILES;
                        task.UserID = _USER_ID;
                        task.MarkComplete();
                    }
                });
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _singleTaskProcess.Dispose();
        }

        private static void TestVariables(IImmutableDictionary<string, object> variables)
        {
            Assert.IsNotNull(variables);
            Assert.IsTrue(variables.ContainsKey(_TEST_VARIABLE_NAME));
            Assert.AreEqual(_TEST_VARIABLE_VALUE, variables[_TEST_VARIABLE_NAME]);
            Assert.IsTrue(variables.ContainsKey(_TEST_FILE_VARIABLE));
            Assert.IsInstanceOfType(variables[_TEST_FILE_VARIABLE], typeof(SFile[]));
            Assert.AreEqual(_TEST_FILES.Length, ((SFile[])variables[_TEST_FILE_VARIABLE]).Length);
            for (var x = 0; x<_TEST_FILES.Length; x++)
                Assert.AreEqual(_TEST_FILES[x], ((SFile[])variables[_TEST_FILE_VARIABLE])[x]);
        }

        private static void TestEmptyVariables(IImmutableDictionary<string, object> variables)
        {
            Assert.IsNotNull(variables);
            Assert.AreEqual(0, variables.Count);
        }

        private static void TestSteps(IEnumerable<IStateStep> steps)
        {
            Assert.IsNotNull(steps);
            Assert.IsTrue(steps.Any(s => s.ElementID=="UserTask_15dj2au" && s.Status==StepStatuses.Waiting));
            Assert.IsTrue(steps.Any(s => s.ElementID=="UserTask_15dj2au" && s.Status==StepStatuses.Succeeded && s.CompletedBy==_USER_ID && s.OutgoingID.Contains("SequenceFlow_1nn72ou")));
            Assert.IsTrue(steps.Any(s => s.ElementID=="EndEvent_181ulmj" && s.Status==StepStatuses.Succeeded && (s.OutgoingID==null || !s.OutgoingID.Any())));
        }

        private static void TestLog(string log)
        {
            Assert.IsTrue(log.Contains(_TEST_LOG_LINE));
            Assert.IsTrue(log.Contains("Debug"));
        }

        private static void TestCurrentState(IState currentState)
        {
            Assert.IsTrue(!currentState.ActiveElements.Any());
            Assert.IsTrue(currentState.Keys.Any(k => k==_TEST_VARIABLE_NAME));
            Assert.AreEqual(_TEST_VARIABLE_VALUE, currentState[_TEST_VARIABLE_NAME]);
        }

        [TestMethod]
        [Timeout(300000)]
        public void TestXMLStorage()
        {
            var inst = _singleTaskProcess.BeginProcess(null, stateLogLevel: Microsoft.Extensions.Logging.LogLevel.Debug);
            Assert.IsTrue(inst.WaitForUserTask("UserTask_15dj2au", out _));
            Assert.IsTrue(inst.CurrentState.ActiveElements.Any());
            Assert.IsTrue(inst.WaitForCompletion());

            TestCurrentState(inst.CurrentState);

            var doc = new XmlDocument();
            doc.LoadXml(inst.CurrentState.AsXMLDocument);

            TestVariables(BusinessProcess.ExtractProcessVariablesFromStateDocument(doc));
            TestEmptyVariables(BusinessProcess.ExtractProcessVariablesFromStateDocument(doc, 2));
            TestSteps(BusinessProcess.ExtractProcessSteps(doc));
            TestLog(BusinessProcess.ExtractProcessLog(doc));
        }

        [TestMethod]
        [Timeout(300000)]
        public void TestJsonStorage()
        {
            var inst = _singleTaskProcess.BeginProcess(null, stateLogLevel: Microsoft.Extensions.Logging.LogLevel.Debug);
            Assert.IsTrue(inst.WaitForUserTask("UserTask_15dj2au", out _));
            Assert.IsTrue(inst.CurrentState.ActiveElements.Any());
            Assert.IsTrue(inst.WaitForCompletion());

            TestCurrentState(inst.CurrentState);

            var data = System.Text.UTF8Encoding.UTF8.GetBytes(inst.CurrentState.AsJSONDocument);

            TestVariables(BusinessProcess.ExtractProcessVariablesFromStateDocument(new Utf8JsonReader(data)));
            TestEmptyVariables(BusinessProcess.ExtractProcessVariablesFromStateDocument(new Utf8JsonReader(data), 2));
            TestSteps(BusinessProcess.ExtractProcessSteps(new Utf8JsonReader(data)));
            TestLog(BusinessProcess.ExtractProcessLog(new Utf8JsonReader(data)));
        }

        [TestMethod]
        public void TestLoadingInvalidState()
        {
            Assert.ThrowsException<NullReferenceException>(() => BusinessProcess.ExtractProcessLog(new XmlDocument()));
            Assert.ThrowsException<NullReferenceException>(() => BusinessProcess.ExtractProcessLog(new Utf8JsonReader(UTF8Encoding.UTF8.GetBytes("{}"))));
        }
    }
}
