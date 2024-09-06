using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace UnitTest.Extensions
{
    [TestClass]
    public class Conditions
    {

        private static BusinessProcess _pathProcess;
        private static Dictionary<string, object> _pathFailVariables;
        private static BusinessProcess _startProcess;
        private static BusinessProcess _eventProcess;

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _pathProcess = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/path_conditions.bpmn"));
            _pathFailVariables = new Dictionary<string, object>()
            {
                {"isnull",1 },
                {"isequal",1 },
                {"isgreater",11 },
                {"isgreaterequal",11 },
                {"isless",13 },
                {"islessequal",13 },
                {"contains",new int[]{1,2,3} },
                {"isnotequal",12 },
                {"andequal1",1 },
                {"andequal2",1 },
                {"orequal1",1 },
                {"orequal2",1 },
                {"cscript",1 },
                {"vbscript",1 },
                {"javascript",1 }
            };
            _startProcess = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/process_start_condition.bpmn"));
            _eventProcess = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/event_start_condition.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _pathProcess.Dispose();
            _startProcess.Dispose();
            _eventProcess.Dispose();
        }

        private async System.Threading.Tasks.Task<XmlDocument> RunPathProcess(Dictionary<string, object> variables)
        {
            Dictionary<string, object> vars = new();
            foreach (string key in _pathFailVariables.Keys)
            {
                if (!variables.ContainsKey(key))
                    vars.Add(key, _pathFailVariables[key]);
            }
            foreach (string key in variables.Keys)
                vars.Add(key, variables[key]);
            IProcessInstance processInstance = await _pathProcess.BeginProcessAsync(vars);
            Assert.IsNotNull(processInstance);
            Assert.IsTrue(Utility.WaitForCompletion(processInstance));
            XmlDocument doc = new();
            doc.LoadXml(processInstance.CurrentState.AsXMLDocument);
            return doc;
        }

        private bool _StepRan(BusinessProcess process, XmlDocument xmlDocument, string name)
        {
            XmlNode node = process.Document.SelectSingleNode(string.Format("/*[local-name()='definitions']/*[local-name()='process']/*[local-name()='sequenceFlow'][@name='{0}']", name));
            if (node==null)
                return false;
            return Utility.StepCompleted(xmlDocument, node.Attributes["id"].Value);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsNull()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isnull", null } }), "IsNull"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsEqual()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isequal", 12 } }), "Equals"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsGreater()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isgreater", 13 } }), "GreaterThan"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsGreaterOrEqual()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isgreaterequal", 12 } }), "GreaterThan OrEqual"));
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isgreaterequal", 13 } }), "GreaterThan OrEqual"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsLess()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isless", 11 } }), "LessThan"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsLessOrEqual()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "islessequal", 12 } }), "LessThan OrEqual"));
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "islessequal", 11 } }), "LessThan OrEqual"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task Contains()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "contains", new int[] { 1, 2, 12, 13, 14 } } }), "Contains"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIsNotEqual()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "isnotequal", 10 } }), "Negated"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestAnd()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "andequal1", 12 }, { "andequal2", 12 } }), "And"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestOr()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "orequal1", 12 }, { "orequal2", 12 } }), "Or"));
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "orequal1", 12 } }), "Or"));
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "orequal2", 12 } }), "Or"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestCSharpScript()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "cscript", 12 } }), "C# Script"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestVBScript()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "vbscript", 12 } }), "VB Script"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestJavascript()
        {
            Assert.IsTrue(_StepRan(_pathProcess, await RunPathProcess(new Dictionary<string, object>() { { "javascript", 12 } }), "JavaScript"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestProcessStartCondition()
        {
            IProcessInstance instance = await _startProcess.BeginProcessAsync(new Dictionary<string, object>() { { "canstart", true } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            instance = await _startProcess.BeginProcessAsync(new Dictionary<string, object>() { { "canstart", false } });
            Assert.IsNull(instance);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestEventStartCondition()
        {
            IProcessInstance instance = await _eventProcess.BeginProcessAsync(new Dictionary<string, object>() { { "canstart", true } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            Assert.IsTrue(_StepRan(_eventProcess, doc, "Can Start"));
            Assert.IsFalse(_StepRan(_eventProcess, doc, "Default"));
            instance = await _eventProcess.BeginProcessAsync(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            doc.LoadXml(instance.CurrentState.AsXMLDocument);
            Assert.IsFalse(_StepRan(_eventProcess, doc, "Can Start"));
            Assert.IsTrue(_StepRan(_eventProcess, doc, "Default"));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestConditionException()
        {
            var errorMessage = "This is a script condition error";
            var cache = new ConcurrentQueue<string>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/path_conditions.bpmn"), logging: new BPMNEngine.DelegateContainers.ProcessLogging()
            {
                LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                {
                    var ex = exception;
                    while (ex!=null)
                    {
                        cache.Enqueue(ex.Message);
                        ex= ex.InnerException;
                    }
                },
                LogLine=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, LogLevel level, DateTime timestamp, string message) =>
                {

                }
            });
            IProcessInstance instance = null;
            try
            {
                instance = await process.BeginProcessAsync(new Dictionary<string, object> { { "cscript", new Exception(errorMessage) } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Assert.IsFalse(instance.WaitForCompletion(2*1000));
            Assert.IsTrue(cache.Any(str => str.Contains(errorMessage)));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestConditionalBoundary()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/conditional_boundary.bpmn"));

            var instance = await process.BeginProcessAsync(new()
            {
                {"canUseCondition",true }
            });

            Assert.IsTrue(Utility.WaitForCompletion(instance));

            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Event_0bzc3s9"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "Flow_1itqwbt"));
        }
    }
}
