using BPMNEngine;
using BPMNEngine.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTest.Helpers;

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
        public async System.Threading.Tasks.Task TestCSharp()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp.bpmn"));
            IProcessInstance instance = null;
            try
            {
                instance = await proc.BeginProcessAsync(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestCSharpException()
        {
            (var loggerFactory, var mockLogger) = LoggingHelper.CreateMockLogFactory();
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_exception.bpmn"), loggerFactory: loggerFactory);
            IProcessInstance instance = null;
            try
            {
                instance = await proc.BeginProcessAsync(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Assert.IsFalse(instance.WaitForCompletion(TimeSpan.FromSeconds(2)));
            //Assert.IsTrue(cache.Any(str => str.Contains("ERROR!!!")));
        }

        [TestMethod]
        public void TestCSharpCompilationException()
        {
            var err = Assert.ThrowsException<InvalidProcessDefinitionException>(() => new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_compilation_error.bpmn")));
            Assert.IsTrue(err.ProcessExceptions.Any(ex => ex.Message.Contains("Unable to compile script Code.  Errors:")));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestActiveStepsException()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_delayed_active_step.bpmn"));
            IProcessInstance instance = await proc.BeginProcessAsync(new Dictionary<string, object> { { _varName, _varValue } });
            Assert.IsNotNull(instance);
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Assert.ThrowsException<ActiveStepsException>(() => instance.Dispose());
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestVBScript()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/visual_basic.bpmn"));
            IProcessInstance instance;
            try
            {
                instance = await proc.BeginProcessAsync(new Dictionary<string, object> { { _varName, _varValue } });
            }
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestJavascript()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/javascript.bpmn"));
            IProcessInstance instance;
            try
            {
                instance = await proc.BeginProcessAsync(new Dictionary<string, object> { { _varName, _varValue } });
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
            var variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(2, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_varName));
            Assert.IsTrue(variables.ContainsKey(_varNamePlus));
            Assert.AreEqual(_varValue, (int)variables[_varName]);
            Assert.AreEqual((double)_varValuePlus, double.Parse(variables[_varNamePlus].ToString()));
        }
    }
}
