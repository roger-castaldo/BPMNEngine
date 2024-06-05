using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using BPMNEngine.Interfaces.Tasks;
using System;

namespace UnitTest
{
    [TestClass]
    public class ManualTasks
    {
        private static BusinessProcess _singleTaskProcess;
        private static BusinessProcess _multiTaskProcess;
        private const string _TEST_VARIABLE_NAME = "TestValue";
        private const string _TEST_VARIABLE_VALUE = "This is a test";
        private readonly static string[] _TEST_VARIABLE_VALUES = new string[]
        {
            "This is another test",
            "This is also a test",
            "Yup another test here"
        };
        
        [ClassInitialize]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required to run properly")]
        public static void Initialize(TestContext testContext) {
            _singleTaskProcess = new BusinessProcess(Utility.LoadResourceDocument("ManualTasks/single_manual_task.bpmn"));
            _multiTaskProcess = new BusinessProcess(Utility.LoadResourceDocument("ManualTasks/multiple_manual_tasks.bpmn"));
        }

        [ClassCleanup]
        public static void Cleanup() {
            _singleTaskProcess.Dispose();
            _multiTaskProcess.Dispose();
        }

        [TestMethod()]
        public void TestManualTaskAccess()
        {
            IProcessInstance instance = _singleTaskProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_VARIABLE_NAME, _TEST_VARIABLE_VALUE}
            });
            Assert.IsNotNull(instance);

            Assert.IsTrue(instance.WaitForManualTask(TimeSpan.FromSeconds(5), "ManualTask_15dj2au", out IManualTask task));
            Assert.IsNotNull(task);

            Assert.AreEqual(1, task.Variables.Keys.Length);
            Assert.AreEqual(_TEST_VARIABLE_VALUE, task.Variables[_TEST_VARIABLE_NAME]);
            task.Variables[_TEST_VARIABLE_NAME] = _TEST_VARIABLE_VALUES[0];
            task.MarkComplete();
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_TEST_VARIABLE_NAME));
            Assert.AreEqual(_TEST_VARIABLE_VALUES[0], variables[_TEST_VARIABLE_NAME]);
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "ManualTask_15dj2au"));
        }

        private static readonly string[] _TaskNames = new string[]
        {
            "ManualTask_15dj2au",
            "ManualTask_06twx0q",
            "ManualTask_1qxmpii"
        };

        [TestMethod]
        public void TestManualTaskSeperation()
        {
            int idx = 0;
            IProcessInstance instance = _multiTaskProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_VARIABLE_NAME, _TEST_VARIABLE_VALUE}
            });
            Assert.IsNotNull(instance);
            Thread.Sleep(6*1000);
            while (idx<3)
            {
#pragma warning disable IDE0018 // Inline variable declaration
                IManualTask task;
#pragma warning restore IDE0018 // Inline variable declaration
                Assert.IsTrue(idx switch
                {
                    0 => instance.WaitForManualTask(TimeSpan.FromSeconds(5), _TaskNames[idx], out task),
                    1 => instance.WaitForManualTask(5*1000, _TaskNames[idx], out task),
                    _ => instance.WaitForManualTask(_TaskNames[idx], out task)
                });
                Assert.IsNotNull(task);
                Assert.IsNull(instance.GetManualTask(_TaskNames[idx+(idx==0 ? 1 : -1)]));
                Assert.AreEqual(1, task.Variables.Keys.Length);
                Assert.AreEqual((idx==0 ? _TEST_VARIABLE_VALUE : _TEST_VARIABLE_VALUES[idx-1]), task.Variables[_TEST_VARIABLE_NAME]);
                task.Variables[_TEST_VARIABLE_NAME] = _TEST_VARIABLE_VALUES[idx];
                task.MarkComplete();
                idx++;
            }
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_TEST_VARIABLE_NAME));
            Assert.AreEqual(_TEST_VARIABLE_VALUES[2], variables[_TEST_VARIABLE_NAME]);
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            Assert.IsTrue(Utility.StepCompleted(doc, _TaskNames[0]));
            Assert.IsTrue(Utility.StepCompleted(doc, _TaskNames[1]));
            Assert.IsTrue(Utility.StepCompleted(doc, _TaskNames[2]));
        }

        [TestMethod()]
        public void TestManualTaskElementPropertiesAccess()
        {
            IProcessInstance instance = _singleTaskProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_VARIABLE_NAME, _TEST_VARIABLE_VALUE}
            });
            Assert.IsNotNull(instance);

            Assert.IsTrue(instance.WaitForManualTask(TimeSpan.FromSeconds(5), "ManualTask_15dj2au", out IManualTask task));
            Assert.IsNotNull(task);
            
            Assert.AreEqual("ManualTask_15dj2au", task.ID);
            Assert.AreEqual(task.ID, task["id"]);

            Assert.IsTrue(task.SubNodes.Any());
            Assert.AreEqual(2, task.SubNodes.Length);

            Assert.IsNull(task.ExtensionElement);
            Assert.IsNull(task.SubProcess);
            
            Assert.IsNotNull(task.Lane);
            Assert.AreEqual("Lane_0z6k1d4", task.Lane.ID);
            
            Assert.IsNotNull(task.Process);
            Assert.AreEqual("Process_1", task.Process.ID);

            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));
        }
    }
}
