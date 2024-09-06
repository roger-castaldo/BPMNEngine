using BPMNEngine;
using BPMNEngine.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UnitTest.Extensions
{
    [TestClass]
    public class CompareConditionDataTypes
    {

        private static BusinessProcess _process;
        private static readonly string[] FLOWS =
        [
            "Flow_1svot8c",//byte
            "Flow_11fz5zo",//char
            "Flow_0vd6gpf",//datetime
            "Flow_1oag4um",//decimal
            "Flow_0t1hecb",//double
            "Flow_0hbl5l3",//single
            "Flow_0l6mmju",//int
            "Flow_1qx5lzc",//long
            "Flow_0ebyg31",//short
            "Flow_0u1l2e3",//boolean
            "Flow_0ebyg32",//null
            "Flow_0ebyg33",//chained variable
            "Flow_0ebyg34" //icomparable
        ];

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("Extensions/Conditions/compare_conditions.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private async System.Threading.Tasks.Task RunPathProcess(Dictionary<string, object> variables, string successPath)
        {
            if (!variables.ContainsKey("null_value_1"))
                variables.Add("null_value_1", "test");
            if (!variables.ContainsKey("comparable_value_1"))
                variables.Add("comparable_value_1", "test");
            IProcessInstance processInstance = await _process.BeginProcessAsync(variables);
            Assert.IsNotNull(processInstance);
            Assert.IsTrue(Utility.WaitForCompletion(processInstance));
            var state = processInstance.CurrentState;
            Assert.IsNotNull(state);
            Assert.IsTrue(Utility.StepCompleted(state, successPath));
            foreach (var flow in FLOWS.Where(f => f!=successPath))
                Assert.IsFalse(Utility.StepCompleted(state, flow));
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestByteArray()
        {
            await RunPathProcess(new()
            {
                {"byte_value",Convert.FromBase64String("VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==") }
            }, "Flow_1svot8c");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestBoolean()
        {
            await RunPathProcess(new()
            {
                {"boolean_value",true }
            }, "Flow_0u1l2e3");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestChar()
        {
            await RunPathProcess(new()
            {
                {"char_value",'V' }
            }, "Flow_11fz5zo");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestDateTime()
        {
            await RunPathProcess(new()
            {
                {"date_value",DateTime.Parse("2024-01-01 23:59:59") }
            }, "Flow_0vd6gpf");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestDecimal()
        {
            await RunPathProcess(new()
            {
                {"decimal_value",decimal.Parse("123.456") }
            }, "Flow_1oag4um");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestDouble()
        {
            await RunPathProcess(new()
            {
                {"double_value",double.Parse("123.456") }
            }, "Flow_0t1hecb");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestFloat()
        {
            await RunPathProcess(new()
            {
                {"single_value",float.Parse("0.123456") }
            }, "Flow_0hbl5l3");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestInt()
        {
            await RunPathProcess(new()
            {
                {"int_value",int.Parse("123456") }
            }, "Flow_0l6mmju");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestLong()
        {
            await RunPathProcess(new()
            {
                {"long_value",long.Parse("123456") }
            }, "Flow_1qx5lzc");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestShort()
        {
            await RunPathProcess(new()
            {
                {"short_value",short.Parse("1234") }
            }, "Flow_0ebyg31");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestNull()
        {
            await RunPathProcess(new()
            {
                {"null_value_1",null },
                {"null_value_2",null }
            }, "Flow_0ebyg32");
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestChainedVariable()
        {
            await RunPathProcess(new()
            {
                {
                    "hashtable",new Hashtable(){
                        {
                            "person",new Dictionary<string, string>()
                            {
                                {"firstname","bob" }
                            }
                        }
                    }
                }
            }, "Flow_0ebyg33");
        }

        private class TestComparable : IComparable
        {
            private readonly string value;
            public TestComparable(string value) { this.value=value; }

            public int CompareTo(object obj)
            {
                if (obj is TestComparable comparable)
                    return value.CompareTo(comparable.value);
                return -1;
            }
        }

        [TestMethod]
        public async System.Threading.Tasks.Task TestIComparableValue()
        {
            await RunPathProcess(new Dictionary<string, object>()
            {
                {"comparable_value_1",new TestComparable("testing123") },
                {"comparable_value_2",new TestComparable("testing123") }
            }, "Flow_0ebyg34");
        }
    }
}
