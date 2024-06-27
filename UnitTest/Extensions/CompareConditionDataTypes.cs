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

        private void RunPathProcess(Dictionary<string, object> variables, string successPath)
        {
            if (!variables.ContainsKey("null_value_1"))
                variables.Add("null_value_1", "test");
            if (!variables.ContainsKey("comparable_value_1"))
                variables.Add("comparable_value_1", "test");
            IProcessInstance processInstance = _process.BeginProcess(variables);
            Assert.IsNotNull(processInstance);
            Assert.IsTrue(Utility.WaitForCompletion(processInstance));
            var state = processInstance.CurrentState;
            Assert.IsNotNull(state);
            Assert.IsTrue(Utility.StepCompleted(state, successPath));
            foreach (var flow in FLOWS.Where(f => f!=successPath))
                Assert.IsFalse(Utility.StepCompleted(state, flow));
        }

        [TestMethod]
        public void TestByteArray()
        {
            RunPathProcess(new()
            {
                {"byte_value",Convert.FromBase64String("VGhlIHF1aWNrIGJyb3duIGZveCBqdW1wcyBvdmVyIHRoZSBsYXp5IGRvZw==") }
            }, "Flow_1svot8c");
        }

        [TestMethod]
        public void TestBoolean()
        {
            RunPathProcess(new()
            {
                {"boolean_value",true }
            }, "Flow_0u1l2e3");
        }

        [TestMethod]
        public void TestChar()
        {
            RunPathProcess(new()
            {
                {"char_value",'V' }
            }, "Flow_11fz5zo");
        }

        [TestMethod]
        public void TestDateTime()
        {
            RunPathProcess(new()
            {
                {"date_value",DateTime.Parse("2024-01-01 23:59:59") }
            }, "Flow_0vd6gpf");
        }

        [TestMethod]
        public void TestDecimal()
        {
            RunPathProcess(new()
            {
                {"decimal_value",decimal.Parse("123.456") }
            }, "Flow_1oag4um");
        }

        [TestMethod]
        public void TestDouble()
        {
            RunPathProcess(new()
            {
                {"double_value",double.Parse("123.456") }
            }, "Flow_0t1hecb");
        }

        [TestMethod]
        public void TestFloat()
        {
            RunPathProcess(new()
            {
                {"single_value",float.Parse("0.123456") }
            }, "Flow_0hbl5l3");
        }

        [TestMethod]
        public void TestInt()
        {
            RunPathProcess(new()
            {
                {"int_value",int.Parse("123456") }
            }, "Flow_0l6mmju");
        }

        [TestMethod]
        public void TestLong()
        {
            RunPathProcess(new()
            {
                {"long_value",long.Parse("123456") }
            }, "Flow_1qx5lzc");
        }

        [TestMethod]
        public void TestShort()
        {
            RunPathProcess(new()
            {
                {"short_value",short.Parse("1234") }
            }, "Flow_0ebyg31");
        }

        [TestMethod]
        public void TestNull()
        {
            RunPathProcess(new()
            {
                {"null_value_1",null },
                {"null_value_2",null }
            }, "Flow_0ebyg32");
        }

        [TestMethod]
        public void TestChainedVariable()
        {
            RunPathProcess(new()
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
        public void TestIComparableValue()
        {
            RunPathProcess(new Dictionary<string, object>()
            {
                {"comparable_value_1",new TestComparable("testing123") },
                {"comparable_value_2",new TestComparable("testing123") }
            }, "Flow_0ebyg34");
        }
    }
}
