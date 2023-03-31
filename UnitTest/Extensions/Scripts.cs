using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

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
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp.bpmn"));
            IProcessInstance instance = null;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e) {
                instance=null;
                Assert.Fail(e.Message);
            }
            _CheckResults(instance);
        }

        [TestMethod]
        public void TestVBScript()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/visual_basic.bpmn"));
            IProcessInstance instance = null;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            _CheckResults(instance);
        }

        [TestMethod]
        public void TestJavascript()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/javascript.bpmn"));
            IProcessInstance instance = null;
            try
            {
                instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            _CheckResults(instance);
        }

        private void _CheckResults(IProcessInstance instance)
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
