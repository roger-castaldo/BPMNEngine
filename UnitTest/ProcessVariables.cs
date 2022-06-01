using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void TestHashtableVariable()
        {
            string variableName = "TestHashtable";
            object variableValue = new Hashtable()
            {
                {"part1",1234 },
                {"part2",5678 }
            };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            Assert.IsTrue(Utility.AreHashtablesEqual((Hashtable)variableValue, (Hashtable)(results.ContainsKey(variableName) ? results[variableName] : null)));
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
    }
}
