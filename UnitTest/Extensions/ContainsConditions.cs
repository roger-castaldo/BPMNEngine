using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace UnitTest.Extensions
{
    [TestClass]
    public class ContainsConditions
    {

        private static BusinessProcess _process;
        private static readonly string[] FLOWS =
        [
            "Flow_0124fzt",//array
            "Flow_0obucim",//dictionary keys
            "Flow_06n50fk",//dictionary values
            "Flow_0ydpcz9", //sub variable
            "Flow_0ydpca9"//string variable
        ];

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/contains_conditions.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private void RunPathProcess(Dictionary<string,object> variables,string successPath)
        {
            IProcessInstance processInstance = _process.BeginProcess(variables);
            Assert.IsNotNull(processInstance);
            Assert.IsTrue(Utility.WaitForCompletion(processInstance));
            var state = processInstance.CurrentState;
            Assert.IsNotNull(state);
            Assert.IsTrue(Utility.StepCompleted(state, successPath));
            foreach(var flow in FLOWS.Where(f=>f!=successPath))
                Assert.IsFalse(Utility.StepCompleted(state,flow));
        }

        [TestMethod]
        public void TestArrayValue()
        {
            RunPathProcess(new()
            {
                {"array_value",new string[]{"test_value","test_value_1" } }
            }, "Flow_0124fzt");
        }

        [TestMethod]
        public void TestDictionaryKeysValue()
        {
            RunPathProcess(new()
            {
                {"dictionary_keys_value",new Hashtable(){
                    {"test_value","test_value_1" },
                    {"test_value_2","test_value_3" }
                } }
            }, "Flow_0obucim");
        }

        [TestMethod]
        public void TestDictionaryValuesValue()
        {
            RunPathProcess(new()
            {
                {"dictionary_values_value",new Hashtable(){
                    {"test_value_1","test_value" },
                    {"test_value_3","test_value_2" }
                } }
            }, "Flow_06n50fk");
        }

        [TestMethod]
        public void TestArraySubVariable()
        {
            RunPathProcess(new()
            {
                {
                    "array_sub_value",
                    new Hashtable[]{
                        new(){
                            {"name","test_value" },
                            {"other","test_value_1" }
                        },
                        new(){
                            {"name","test_value_2" },
                            {"other","test_value_3" }
                        }
                    } 
                }
            }, "Flow_0ydpcz9");
        }

        [TestMethod]
        public void TestStringContainsValue()
        {
            RunPathProcess(new()
            {
                {"string_value","this string is a test"}
            }, "Flow_0ydpca9");
        }
    }
}
