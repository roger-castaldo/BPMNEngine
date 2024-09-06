using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
        public async System.Threading.Tasks.Task TestCustomExtension()
        {
            BusinessProcess process = new(Utility.LoadResourceDocument("Extensions/Custom/custom_extension.bpmn"), tasks: new BPMNEngine.DelegateContainers.ProcessTasks() { ProcessTask= new ProcessTask(ProcessTask) });
            Assert.IsNotNull(process);
            IProcessInstance instance = await process.BeginProcessAsync(new Dictionary<string, object>()
            {
                {_VARIABLE_NAME,_VARIABLE_VALUE }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            var variables = instance.CurrentVariables;
            Assert.AreEqual(1, variables.Count);
            Assert.IsTrue(variables.ContainsKey(_VARIABLE_NAME));
            Assert.AreEqual(_RESULT_VARIABLE_VALUE, variables[_VARIABLE_NAME]);

            instance = await process.BeginProcessAsync(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            variables = instance.CurrentVariables;
            Assert.AreEqual(0, variables.Count);
            Assert.IsFalse(variables.ContainsKey(_VARIABLE_NAME));
        }

        private void ProcessTask(ITask task)
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
