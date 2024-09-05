using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var readonlyKeysValid = false;
            BusinessProcess process = new(Utility.LoadResourceDocument("Extensions/definition_variable.bpmn"));
            Assert.IsNotNull(process);
            Assert.IsNotNull(process[_VARIABLE_NAME]);
            Assert.IsInstanceOfType(process[_VARIABLE_NAME], typeof(string));

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { }, events: new BPMNEngine.DelegateContainers.ProcessEvents()
            {
                Tasks = new()
                {
                    Completed=(element, variables) =>
                    {
                        readonlyKeysValid = variables.FullKeys.Contains(_VARIABLE_NAME)
                        && variables.Keys.Contains(_RESULT_VARIABLE_NAME);
                    }
                }
            });
            Assert.IsNotNull(instance);

            Assert.IsTrue(instance.WaitForUserTask(TimeSpan.FromSeconds(5), "UserTask_15dj2au", out IUserTask task));

            Assert.IsNotNull(task);
            Assert.IsTrue(task.Variables.FullKeys.Contains(_VARIABLE_NAME));
            task.Variables[_RESULT_VARIABLE_NAME] = string.Format(_RESULT_FORMAT, task.Variables[_VARIABLE_NAME]);
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));

            var variables = instance.CurrentVariables;
            Assert.AreEqual(1, variables.Count);
            Assert.IsFalse(variables.ContainsKey(_VARIABLE_NAME));
            Assert.IsTrue(variables.ContainsKey(_RESULT_VARIABLE_NAME));
            Assert.AreEqual(string.Format(_RESULT_FORMAT, process[_VARIABLE_NAME]), variables[_RESULT_VARIABLE_NAME]);
            Task.Delay(TimeSpan.FromSeconds(5)).Wait();
            Assert.IsTrue(readonlyKeysValid);
        }
    }
}
