using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace UnitTest.Extensions
{
    [TestClass]
    public class Conditions
    {

        private static BusinessProcess _pathProcess;
        private static Dictionary<string, object> _pathFailVariables;

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
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _pathProcess.Dispose();
        }

        private XmlDocument RunPathProcess(Dictionary<string,object> variables)
        {
            Dictionary<string, object> vars = new Dictionary<string, object>();
            foreach (string key in _pathFailVariables.Keys)
            {
                if (!variables.ContainsKey(key))
                    vars.Add(key, _pathFailVariables[key]);
            }
            foreach (string key in variables.Keys)
                vars.Add(key,variables[key]);
            IProcessInstance processInstance = _pathProcess.BeginProcess(vars);
            Assert.IsNotNull(processInstance);
            Assert.IsTrue(processInstance.WaitForCompletion(30*1000));
            return processInstance.CurrentState;
        }

        private bool _StepRan(XmlDocument xmlDocument, string name)
        {
            XmlNode node = _pathProcess.Document.SelectSingleNode(string.Format("/*[local-name()='definitions']/*[local-name()='process']/*[local-name()='sequenceFlow'][@name='{0}']", name));
            if (node==null)
                return false;
            return xmlDocument.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='Succeeded']",node.Attributes["id"].Value))!=null;
        }

        [TestMethod]
        public void TestIsNull()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isnull", null } }),"IsNull"));
        }

        [TestMethod]
        public void TestIsEqual()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isequal", 12} }), "Equals"));
        }

        [TestMethod]
        public void TestIsGreater()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isgreater", 13 } }), "GreaterThan"));
        }

        [TestMethod]
        public void TestIsGreaterOrEqual()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isgreaterequal", 12 } }), "GreaterThan OrEqual"));
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isgreaterequal", 13 } }), "GreaterThan OrEqual"));
        }

        [TestMethod]
        public void TestIsLess()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isless", 11 } }), "LessThan"));
        }

        [TestMethod]
        public void TestIsLessOrEqual()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "islessequal", 12 } }), "LessThan OrEqual"));
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "islessequal", 11 } }), "LessThan OrEqual"));
        }

        [TestMethod]
        public void Contains()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "contains", new int[] { 1, 2, 12, 13, 14 } } }), "Contains"));
        }

        [TestMethod]
        public void TestIsNotEqual()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "isnotequal", 10 } }), "Negated"));
        }

        [TestMethod]
        public void TestAnd()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "andequal1", 12 }, { "andequal2", 12 } }), "And"));
        }

        [TestMethod]
        public void TestOr()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "orequal1", 12 }, { "orequal2", 12 } }), "Or"));
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "orequal1", 12 } }), "Or"));
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "orequal2", 12 } }), "Or"));
        }

        [TestMethod]
        public void TestCSharpScript()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "cscript", 12 } }), "C# Script"));
        }

        [TestMethod]
        public void TestVBScript()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "vbscript", 12 } }), "VB Script"));
        }

        [TestMethod]
        public void TestJavascript()
        {
            Assert.IsTrue(_StepRan(RunPathProcess(new Dictionary<string, object>() { { "javascript", 12 } }), "JavaScript"));
        }
    }
}
