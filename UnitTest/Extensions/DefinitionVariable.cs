using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest.Extensions
{
    [TestClass]
    public class DefinitionVariable
    {
        private const string _VARIABLE_NAME = "FullName";
        private const string _RESULT_VARIABLE_NAME = "Processed_FullName";
        private const string _RESULT_FORMAT = "Processed_{0}";

        [TestMethod]
        public void TestDefinitionVariableUsage()
        {
            BusinessProcess process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/definition_variable.bpmn"));
            Assert.IsNotNull(process);
            Assert.IsNotNull(process[_VARIABLE_NAME]);
            Assert.IsInstanceOfType(process[_VARIABLE_NAME], typeof(string));

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            var task = instance.GetUserTask("UserTask_15dj2au");
            Assert.IsNotNull(task);
            Assert.IsTrue(task.Variables.FullKeys.Contains(_VARIABLE_NAME));
            task.Variables[_RESULT_VARIABLE_NAME] = string.Format(_RESULT_FORMAT, task.Variables[_VARIABLE_NAME]);
            task.MarkComplete();

            Assert.IsTrue(instance.WaitForCompletion(30*1000));
            
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.AreEqual(1,variables.Count);
            Assert.IsFalse(variables.ContainsKey(_VARIABLE_NAME));
            Assert.IsTrue(variables.ContainsKey(_RESULT_VARIABLE_NAME));
            Assert.AreEqual(string.Format(_RESULT_FORMAT, process[_VARIABLE_NAME]), variables[_RESULT_VARIABLE_NAME]);
        }
    }
}
