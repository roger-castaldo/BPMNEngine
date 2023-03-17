using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace UnitTest.Extensions
{
    [TestClass]
    public class Custom
    {
        private const string _VARIABLE_NAME = "test_variable";
        private const int _VARIABLE_VALUE = 1234;
        private const int _RESULT_VARIABLE_VALUE = 5678;

        [TestMethod]
        public void TestCustomExtension()
        {
            BusinessProcess process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Custom/custom_extension.bpmn"), tasks:new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks() { ProcessTask= new ProcessTask(_ProcessTask) });
            Assert.IsNotNull(process);
            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>()
            {
                {_VARIABLE_NAME,_VARIABLE_VALUE }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            Dictionary<string, object> variables = instance.CurrentVariables;
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_VARIABLE_NAME));
            Assert.AreEqual(_RESULT_VARIABLE_VALUE, variables[_VARIABLE_NAME]);

            instance = process.BeginProcess(new Dictionary<string, object>(){});
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            variables = instance.CurrentVariables;
            Assert.AreEqual(0, variables.Count);
            Assert.IsFalse(variables.ContainsKey(_VARIABLE_NAME));
        }

        private void _ProcessTask(ITask task)
        {
            if (task.ExtensionElement!=null)
            {
                foreach (XmlNode n in task.ExtensionElement.SubNodes)
                {
                    if (n.NodeType==XmlNodeType.Element)
                    {
                        if (n.Name=="ChangeVariable")
                        {
                            if (task.Variables[n.Attributes["name"].Value]!=null)
                            {
                                if ((int)task.Variables[n.Attributes["name"].Value] == int.Parse(n.Attributes["current_value"].Value))
                                {
                                    task.Variables[n.Attributes["name"].Value] = int.Parse(n.Attributes["new_value"].Value);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
