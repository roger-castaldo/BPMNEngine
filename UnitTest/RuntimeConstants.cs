using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class RuntimeConstants
    {
        private const string VARIABLE_NAME = "TestVariable";
        private const string RESULT_VARIABLE_NAME = "OtherTestVariable";
        private const int VARIABLE_VALUE = 123456;

        [TestMethod]
        public void TestInstanceAccessToConstants()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"), constants: new SProcessRuntimeConstant[]
            {
                new SProcessRuntimeConstant(){
                    Name=VARIABLE_NAME,
                    Value=VARIABLE_VALUE
                }
            });
            var instance = process.BeginProcess(new Dictionary<string, object>() { });

            Assert.IsNotNull(instance);
            Thread.Sleep(5*1000);
            IUserTask task = instance.GetUserTask("UserTask_15dj2au");
            Assert.IsNotNull(task);
            Assert.AreEqual(1, task.Variables.FullKeys.Count());
            Assert.IsTrue(task.Variables.FullKeys.Contains(VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, task.Variables[VARIABLE_NAME]);

            task.Variables[RESULT_VARIABLE_NAME] = task.Variables[VARIABLE_NAME];
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(RESULT_VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[RESULT_VARIABLE_NAME]);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            variables = BusinessProcess.ExtractProcessVariablesFromStateDocument(doc);
            Assert.IsNotNull(variables);
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(RESULT_VARIABLE_NAME));
            Assert.AreEqual(VARIABLE_VALUE, variables[RESULT_VARIABLE_NAME]);
        }
    }
}
