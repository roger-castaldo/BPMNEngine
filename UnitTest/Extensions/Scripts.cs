using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using BPMNEngine.Interfaces.Elements;

namespace UnitTest.Extensions
{
    [TestClass]
    public class Scripts
    {
        private const string _varName = "var1";
        private const string _varNamePlus = "var1_plus1";
        private const int _varValue = 12;
        private const int _varValuePlus = 13;

        [TestMethod]
        public void TestCSharp()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp.bpmn"),logging:new BPMNEngine.DelegateContainers.ProcessLogging()
            {
                LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }
            });
            IProcessInstance instance = null;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e) {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        [TestMethod]
        public void TestVBScript()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/visual_basic.bpmn"));
            IProcessInstance instance;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        [TestMethod]
        public void TestJavascript()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/javascript.bpmn"));
            IProcessInstance instance;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        private static void CheckResults(IProcessInstance instance)
        {
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(2, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_varName));
            Assert.IsTrue(variables.ContainsKey(_varNamePlus));
            Assert.AreEqual(_varValue, (int)variables[_varName]);
            Assert.AreEqual((double)_varValuePlus, double.Parse(variables[_varNamePlus].ToString()));
        }
    }
}
