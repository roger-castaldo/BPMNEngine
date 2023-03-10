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
    public class DefinitionFile
    {
        private const string _FILE_CONTENT = "A quick brown fox jumped over the lazy dog";
        private const string _FILE_NAME = "FullName.txt";
        private const string _RESULT_VARIABLE_NAME = "Processed_File";

        [TestMethod]
        public void TestDefinitionVariableUsage()
        {
            BusinessProcess process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/definition_file.bpmn"), beginUserTask: new StartUserTask(_StartUserTask));
            Assert.IsNotNull(process);
            Assert.IsNotNull(process[_FILE_NAME]);
            Assert.IsInstanceOfType(process[_FILE_NAME], typeof(sFile));

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));
            
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.AreEqual(1,variables.Count);
            Assert.IsFalse(variables.ContainsKey(_FILE_NAME));
            Assert.IsTrue(variables.ContainsKey(_RESULT_VARIABLE_NAME));
            Assert.AreEqual(_FILE_CONTENT, variables[_RESULT_VARIABLE_NAME]);
        }

        private void _StartUserTask(IUserTask task)
        {
            var file = (sFile)task.Variables[_FILE_NAME];
            task.Variables[_RESULT_VARIABLE_NAME] = System.Text.ASCIIEncoding.ASCII.GetString(file.Content);
            task.MarkComplete();
        }
    }
}
