﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace UnitTest
{
    [TestClass]
    public class ProcessVariables
    {
        private static BusinessProcess _process;

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("DiagramLoading/start_to_stop.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private Dictionary<string,object> _TestProcessVariable(string variableName,object variableValue)
        {
            try
            {
                IProcessInstance inst = _process.BeginProcess(new Dictionary<string, object>() { { variableName, variableValue } });
                inst.WaitForCompletion();
                return inst.CurrentVariables;
            }
            catch(Exception ex) { }
            return null;
        }

        [TestMethod]
        public void TestDateTimeVariable()
        {
            string variableName = "TestDateTime";
            object variableValue = DateTime.Now;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestIntegerVariable()
        {
            string variableName = "TestInteger";
            object variableValue = int.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestShortVariable()
        {
            string variableName = "TestShort";
            object variableValue = short.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestLongVariable()
        {
            string variableName = "TestLong";
            object variableValue = long.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestUnsignedIntegerVariable()
        {
            string variableName = "TestUnsignedInteger";
            object variableValue = uint.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestUnsignedShortVariable()
        {
            string variableName = "TestUnsignedShort";
            object variableValue = ushort.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestUnsignedLongVariable()
        {
            string variableName = "TestUnsignedLong";
            object variableValue = long.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestDoubleVariable()
        {
            string variableName = "TestDouble";
            object variableValue = double.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestDecimalVariable()
        {
            string variableName = "TestDecimal";
            object variableValue = decimal.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestStringVariable()
        {
            string variableName = "TestString";
            object variableValue = "This is a test string";
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestCharVariable()
        {
            string variableName = "TestChar";
            object variableValue = 'c';
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestBooleanVariable()
        {
            string variableName = "TestBoolean";
            object variableValue = true;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestFloatVariable()
        {
            string variableName = "TestFloat";
            object variableValue = float.MinValue;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue.ToString(), (results.ContainsKey(variableName) ? results[variableName].ToString() : null));
        }

        [TestMethod]
        public void TestByteVariable()
        {
            string variableName = "TestByte";
            object variableValue = System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 12345");
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(Convert.ToBase64String((byte[])variableValue), (results.ContainsKey(variableName) ? Convert.ToBase64String((byte[])results[variableName]) : null));
        }

        [TestMethod]
        public void TestNullVariable()
        {
            string variableName = "TestNull";
            object variableValue = null;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestFileVariable()
        {
            string variableName = "TestFile";
            Stream str = Utility.LoadResource("DiagramLoading/start_to_stop.bpmn");
            byte[] data = new byte[str.Length];
            str.Read(data,0, data.Length);
            str.Close();
            object variableValue = new sFile("start_to_stop","bpmn",data);
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestArrayVariable()
        {
            string variableName = "TestArray";
            object variableValue = "This is a test".ToCharArray();
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual(new string((char[])variableValue), (results.ContainsKey(variableName) ? new string((char[])results[variableName]) : null));
        }

        [TestMethod]
        public void TestGuidVariable()
        {
            string variableName = "TestGuid";
            object variableValue = new Guid("a4966f14-0108-48ba-a793-8be9982bc411");
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.AreEqual((Guid?)variableValue, (Guid?)(results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestStateXMLVariableStorage()
        {
            Stream str = Utility.LoadResource("DiagramLoading/start_to_stop.bpmn");
            byte[] data = new byte[str.Length];
            str.Read(data, 0, data.Length);
            str.Close();
            Dictionary<string, object> variables = new Dictionary<string, object>()
            {
                {"TestDateTime",DateTime.Now },
                {"TestInteger",int.MinValue },
                {"TestShort",short.MinValue },
                {"TestLong",long.MinValue },
                {"TestUnsignedInteger",uint.MinValue },
                {"TestUnsignedShort",ushort.MinValue },
                {"TestUnsignedLong",ulong.MinValue },
                {"TestDouble",double.MinValue },
                {"TestDecimal",decimal.MinValue },
                {"TestString","This is a test string"},
                {"TestChar",'c'},
                {"TestBoolean",true },
                {"TestFloat",float.MinValue},
                {"TestByte", System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 12345")},
                {"TestNull",null },
                { "TestFile",new sFile("start_to_stop","bpmn",data) },
                {"TestArray","This is a test".ToCharArray() },
                {"TestGuid",new Guid("a4966f14-0108-48ba-a793-8be9982bc411") }
            };
            IProcessInstance inst = _process.BeginProcess(variables);
            Assert.IsNotNull(inst);
            Assert.IsTrue(inst.WaitForCompletion(30*1000));
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(inst.CurrentState.InnerXml);

            _CompareVariableSets(variables, BusinessProcess.ExtractProcessVariablesFromStateDocument(doc));

            inst = _process.LoadState(doc);
            Assert.IsNotNull(inst);
            _CompareVariableSets(variables, inst.CurrentVariables);
        }

        private void _CompareVariableSets(Dictionary<string, object> inputted, Dictionary<string, object> extracted)
        {
            Assert.AreEqual(inputted.Count, extracted.Count);
            foreach (string str in inputted.Keys)
            {
                Assert.IsTrue(extracted.ContainsKey(str));
                if (inputted[str] is DateTime)
                    Assert.AreEqual(inputted[str].ToString(), extracted[str].ToString());
                else if (inputted[str] is byte[])
                    Assert.AreEqual(Convert.ToBase64String((byte[])inputted[str]), Convert.ToBase64String((byte[])extracted[str]));
                else if (inputted[str] is char[])
                    Assert.AreEqual(new String((char[])inputted[str]), new string((char[])extracted[str]));
                else
                    Assert.AreEqual(inputted[str], extracted[str]);
            }
        }
    }
}
