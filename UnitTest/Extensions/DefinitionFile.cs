using Microsoft.VisualStudio.TestTools.UnitTesting;
using BpmEngine;
using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest.Extensions
{
    [TestClass]
    public class DefinitionFile
    {
        private const string _FILE_CONTENT = "A quick brown fox jumped over the lazy dog";
        private const string _FILE_NAME = "FullName.txt";
        private const string _RESULT_VARIABLE_NAME = "Processed_File";

        [TestMethod]
        public void TestDefinitionVariableUsage()
        {
            BusinessProcess process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/definition_file.bpmn"));
            Assert.IsNotNull(process);
            Assert.IsNotNull(process[_FILE_NAME]);
            Assert.IsInstanceOfType(process[_FILE_NAME], typeof(SFile));

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);

            Task.Delay(TimeSpan.FromSeconds(1)).Wait();
            var task = instance.GetUserTask("UserTask_15dj2au");
            Assert.IsNotNull(task);
            Assert.IsTrue(task.Variables.FullKeys.Contains(_FILE_NAME));
            var file = (SFile)task.Variables[_FILE_NAME];
            task.Variables[_RESULT_VARIABLE_NAME] = System.Text.ASCIIEncoding.ASCII.GetString(file.Content);
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));
            
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.AreEqual(1,variables.Count);
            Assert.IsFalse(variables.ContainsKey(_FILE_NAME));
            Assert.IsTrue(variables.ContainsKey(_RESULT_VARIABLE_NAME));
            Assert.AreEqual(_FILE_CONTENT, variables[_RESULT_VARIABLE_NAME]);
        }
    }
}
