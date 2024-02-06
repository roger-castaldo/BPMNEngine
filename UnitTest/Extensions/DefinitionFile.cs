﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using BPMNEngine.Interfaces.Tasks;

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

            Assert.IsTrue(instance.WaitForUserTask(TimeSpan.FromSeconds(5), "UserTask_15dj2au", out IUserTask task));
            
            Assert.IsNotNull(task);
            Assert.IsTrue(task.Variables.FullKeys.Contains(_FILE_NAME));
            var file = (SFile)task.Variables[_FILE_NAME];
            task.Variables[_RESULT_VARIABLE_NAME] = System.Text.ASCIIEncoding.ASCII.GetString(file.Content);
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));
            
            var variables = instance.CurrentVariables;
            Assert.AreEqual(1,variables.Count);
            Assert.IsFalse(variables.ContainsKey(_FILE_NAME));
            Assert.IsTrue(variables.ContainsKey(_RESULT_VARIABLE_NAME));
            Assert.AreEqual(_FILE_CONTENT, variables[_RESULT_VARIABLE_NAME]);
        }
    }
}
