using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp.bpmn"), logging: new BPMNEngine.DelegateContainers.ProcessLogging()
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
            catch (Exception e)
            {
                instance=null;
                Assert.Fail(e.Message);
            }
            Scripts.CheckResults(instance);
        }

        [TestMethod]
        public void TestCSharpException()
        {
            var cache = new ConcurrentQueue<string>();
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_exception.bpmn"), logging: new BPMNEngine.DelegateContainers.ProcessLogging()
            {
                LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                {
                    var ex = exception;
                    while (ex!=null)
                    {
                        cache.Enqueue(ex.Message);
                        ex= ex.InnerException;
                    }
                }
            });
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
            Assert.IsFalse(instance.WaitForCompletion(TimeSpan.FromSeconds(2)));
            Assert.IsTrue(cache.Any(str => str.Contains("ERROR!!!")));
        }

        [TestMethod]
        public void TestCSharpCompilationException()
        {
            var err = Assert.ThrowsException<InvalidProcessDefinitionException>(() => new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_compilation_error.bpmn")));
            Assert.IsTrue(err.ProcessExceptions.Any(ex => ex.Message.Contains("Unable to compile script Code.  Errors:")));
        }

        [TestMethod]
        public void TestActiveStepsException()
        {
            var proc = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Scripts/c_sharp_delayed_active_step.bpmn"), logging: new BPMNEngine.DelegateContainers.ProcessLogging()
            {
                LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                {
                    System.Diagnostics.Debug.WriteLine(exception.Message);
                }
            });
            IProcessInstance instance = proc.BeginProcess(new Dictionary<string, object> { { _varName, _varValue } });
            Assert.IsNotNull(instance);
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Assert.ThrowsException<ActiveStepsException>(() => instance.Dispose());
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
