﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Linq;
using BPMNEngine.Interfaces.Tasks;
using System.Text.Json;
using System.Collections.Immutable;

namespace UnitTest
{
    [TestClass]
    public class ProcessVariables
    {
        private static BusinessProcess _process;

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _process = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"));
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            _process.Dispose();
        }

        private static IImmutableDictionary<string, object> TestProcessVariable(string variableName, object variableValue,bool changeValue=false,object newValue=null)
        {
            IProcessInstance instance = _process.BeginProcess(new Dictionary<string, object>() { { variableName, variableValue } });

            Assert.IsTrue(instance.WaitForUserTask(TimeSpan.FromSeconds(10), "UserTask_15dj2au", out IUserTask task));
            Assert.IsNotNull(task);
            if (variableValue==null)
                Assert.AreEqual(0, task.Variables.Keys.Count());
            else
            {
                Assert.AreEqual(1, task.Variables.Keys.Count());
                CompareVariableValue(variableValue, task.Variables[variableName]);
            }

            if (changeValue)
                task.Variables[variableName]=newValue;

            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(instance));
            return instance.CurrentVariables;
        }

        [TestMethod]
        public void TestDateTimeVariable()
        {
            var variableName = "TestDateTime";
            var variableValue = DateTime.Now;
            var variableArray = new DateTime[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestIntegerVariable()
        {
            var variableName = "TestInteger";
            var variableValue = int.MinValue;
            var variableArray = new int[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestShortVariable()
        {
            var variableName = "TestShort";
            var variableValue = short.MinValue;
            var variableArray = new short[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestLongVariable()
        {
            var variableName = "TestLong";
            var variableValue = long.MinValue;
            var variableArray = new long[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestUnsignedIntegerVariable()
        {
            var variableName = "TestUnsignedInteger";
            var variableValue = uint.MinValue;
            var variableArray = new uint[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestUnsignedShortVariable()
        {
            var variableName = "TestUnsignedShort";
            var variableValue = ushort.MinValue;
            var variableArray = new ushort[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestUnsignedLongVariable()
        {
            var variableName = "TestUnsignedLong";
            var variableValue = ulong.MaxValue;
            var variableArray = new ulong[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestDoubleVariable()
        {
            var variableName = "TestDouble";
            var variableValue = double.MinValue;
            var variableArray = new double[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestDecimalVariable()
        {
            var variableName = "TestDecimal";
            var variableValue = decimal.MinValue;
            var variableArray = new decimal[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestStringVariable()
        {
            var variableName = "TestString";
            var variableValue = "This is a test string";
            var variableArray = new string[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestCharVariable()
        {
            var variableName = "TestChar";
            var variableValue = 'c';
            var variableArray = new char[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestBooleanVariable()
        {
            var variableName = "TestBoolean";
            var variableValue = true;
            var variableArray = new bool[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestFloatVariable()
        {
            var variableName = "TestFloat";
            var variableValue = float.MinValue;
            var variableArray = new float[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out  value) ? value : null));
        }

        [TestMethod]
        public void TestByteVariable()
        {
            var variableName = "TestByte";
            var variableValue = System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 12345");
            var variableArray = new byte[][] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestNullVariable()
        {
            var variableName = "TestNull";
            object variableValue = null;
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsFalse(results.ContainsKey(variableName));
            Assert.AreEqual(0,results.Count);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));
        }

        [TestMethod]
        public void TestFileVariable()
        {
            var variableName = "TestFile";
            Stream str = Utility.LoadResource("DiagramLoading/start_to_stop.bpmn");
            byte[] data = new byte[str.Length];
            str.Read(data,0, data.Length);
            str.Close();
            var variableValue = new SFile() { Name="start_to_stop", Extension="bpmn", ContentType="text/xml", Content=data };
            var variableArray = new SFile[] { variableValue, variableValue };

            Assert.IsTrue(variableArray[0].Equals(variableArray[1]));
            Assert.IsFalse(variableArray[0].Equals("testing"));
            Assert.AreEqual(variableArray[0].GetHashCode(), variableArray[1].GetHashCode());
            Assert.IsTrue(variableArray[0]==variableArray[1]);
            Assert.IsFalse(variableArray[0]!=variableArray[1]);

            Assert.IsFalse(variableArray[0].Equals(new SFile() { Name="stop_to_start", Extension="bpmn", ContentType="text/xml", Content=data }));
            Assert.IsFalse(variableArray[0].Equals(new SFile() { Name="start_to_stop", Extension="xml", ContentType="text/xml", Content=data }));
            Assert.IsFalse(variableArray[0].Equals(new SFile() { Name="start_to_stop", Extension="bpmn", ContentType="text/text", Content=data }));
            Assert.IsFalse(variableArray[0].Equals(new SFile() { Name="start_to_stop", Extension="bpmn", ContentType="text/xml", Content=new byte[] { 0, 0, 0, 0 } }));

            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestGuidVariable()
        {
            var variableName = "TestGuid";
            var variableValue = new Guid("a4966f14-0108-48ba-a793-8be9982bc411");
            var variableArray = new Guid[] { variableValue, variableValue };
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableArray, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestVariableChange()
        {
            var variableName = "TestDateTime";
            var variableValue = DateTime.Now;
            var altVariableValue = "Test Value";
            var results = ProcessVariables.TestProcessVariable(variableName, variableValue,changeValue:true);
            Assert.IsNotNull(results);
            Assert.IsTrue(!results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==0);

            results = ProcessVariables.TestProcessVariable(variableName, null, changeValue: true,newValue:variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(variableValue, (results.TryGetValue(variableName, out object value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, variableValue, changeValue: true, newValue: altVariableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(altVariableValue, (results.TryGetValue(variableName, out value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, null, changeValue: true);
            Assert.IsNotNull(results);
            Assert.IsTrue(!results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==0);

            results = ProcessVariables.TestProcessVariable(variableName, new DateTime[] {variableValue,variableValue}, changeValue: true, newValue: new DateTime[] { variableValue, variableValue });
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(new DateTime[] { variableValue, variableValue }, (results.TryGetValue(variableName, out value) ? value: null));

            results = ProcessVariables.TestProcessVariable(variableName, variableValue, changeValue: true, newValue: new DateTime[] { variableValue, variableValue });
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(new DateTime[] { variableValue, variableValue }, (results.TryGetValue(variableName, out value) ? value : null));

            results = ProcessVariables.TestProcessVariable(variableName, new DateTime[] { variableValue, variableValue,variableValue }, changeValue: true, newValue: new DateTime[] { variableValue, variableValue });
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            CompareVariableValue(new DateTime[] { variableValue, variableValue }, (results.TryGetValue(variableName, out value) ? value : null));
        }

        [TestMethod]
        public void TestStateVariableStorage()
        {
            Stream str = Utility.LoadResource("DiagramLoading/start_to_stop.bpmn");
            byte[] data = new byte[str.Length];
            str.Read(data, 0, data.Length);
            str.Close();
            var variables = new Dictionary<string, object>()
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
                { "TestFile",new SFile(){Name="start_to_stop",Extension="bpmn",Content=data } },
                {"TestGuid",new Guid("a4966f14-0108-48ba-a793-8be9982bc411") },

                {"TestDateTimeArray",new DateTime[]{DateTime.Now,DateTime.Now } },
                {"TestIntegerArray",new int[]{int.MinValue,int.MaxValue } },
                {"TestShortArray",new short[]{short.MinValue,short.MaxValue } },
                {"TestLongArray",new long[] { long.MinValue, long.MaxValue } },
                {"TestUnsignedIntegerArray",new uint[] { uint.MinValue, uint.MaxValue } },
                {"TestUnsignedShortArray",new ushort[] { ushort.MinValue, ushort.MaxValue } },
                {"TestUnsignedLongArray",new ulong[] { ulong.MinValue, ulong.MaxValue } },
                {"TestDoubleArray",new double[] { double.MinValue, double.MaxValue } },
                {"TestDecimalArray",new decimal[] { decimal.MinValue, decimal.MaxValue } },
                {"TestStringArray",new string[]{"This is a test string","This is also a test string" } },
                {"TestCharArray",new char[]{'c','d' } },
                {"TestBooleanArray",new bool[]{true,false } },
                {"TestFloatArray",new float[] { float.MinValue, float.MaxValue }},
                {"TestByteArray", new byte[][]{System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 12345"),System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 678910") } },
                { "TestFileArray",new SFile[]{new SFile(){Name="start_to_stop",Extension="bpmn",Content=data },new SFile(){Name="start_to_stop",Extension="bpmn",Content=data } } },
                {"TestGuidArray",new Guid[]{new Guid("a4966f14-0108-48ba-a793-8be9982bc411"),new Guid("a4966f14-0108-48ba-a793-8be9982bc412") } }
            };

            var taskVariables = new Dictionary<string, object>();

            IProcessInstance inst = _process.BeginProcess(variables);
            Assert.IsNotNull(inst);

            variables.Remove("TestNull");

            Assert.IsTrue(inst.WaitForUserTask(10*1000, "UserTask_15dj2au", out IUserTask task));
            Assert.IsNotNull(task);

            foreach (string key in task.Variables.Keys)
                taskVariables.Add(key, task.Variables[key]);
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(inst));

            Assert.IsFalse(taskVariables.ContainsKey("TestNull"));

            CompareVariableSets(variables, taskVariables.ToImmutableDictionary());

            var doc = new XmlDocument();
            doc.LoadXml(inst.CurrentState.AsXMLDocument);

            var bytes = System.Text.UTF8Encoding.UTF8.GetBytes(inst.CurrentState.AsJSONDocument);

            CompareVariableSets(variables, BusinessProcess.ExtractProcessVariablesFromStateDocument(doc));

            CompareVariableSets(variables, BusinessProcess.ExtractProcessVariablesFromStateDocument(new Utf8JsonReader(bytes)));

            inst = _process.LoadState(doc);
            Assert.IsNotNull(inst);
            CompareVariableSets(variables, inst.CurrentVariables);

            inst = _process.LoadState(new Utf8JsonReader(bytes));
            Assert.IsNotNull(inst);
            CompareVariableSets(variables, inst.CurrentVariables);
        }

        private static void CompareVariableSets(Dictionary<string, object> inputted, IImmutableDictionary<string, object> extracted)
        {
            Assert.AreEqual(inputted.Count, extracted.Count);
            foreach (string str in inputted.Keys)
            {
                Assert.IsTrue(extracted.ContainsKey(str));
                CompareVariableValue(inputted[str], extracted[str]);
            }
        }

        private static void CompareVariableValue(object inputted,object extracted)
        {
            if (inputted==null)
                Assert.IsNull(extracted);
            else if (inputted.GetType().IsArray)
            {
                Assert.IsTrue(extracted.GetType().IsArray);
                Assert.AreEqual(((IEnumerable)inputted).Cast<object>().Count(), ((IEnumerable)extracted).Cast<object>().Count());
                var inpGroups = ((IEnumerable)inputted).Cast<object>().Select((v, i) => new { value = v, index = i });
                var extGroups = ((IEnumerable)extracted).Cast<object>().Select((v, i) => new { value = v, index = i });
                foreach (var inp in inpGroups)
                    CompareVariableValue(inp.value, extGroups.FirstOrDefault(grp => grp.index==inp.index).value);
            }
            else if (inputted is DateTime)
            {
                Assert.IsInstanceOfType(extracted, typeof(DateTime));
                Assert.AreEqual(inputted.ToString(), extracted.ToString());
            }
            else if (inputted is byte[] v)
            {
                Assert.IsInstanceOfType(extracted, typeof(byte));
                Assert.AreEqual(Convert.ToBase64String(v), Convert.ToBase64String((byte[])extracted));
            }
            else if (inputted is SFile file)
            {
                Assert.IsInstanceOfType(extracted, typeof(SFile));
                var isfile = file;
                var extfile = (SFile)extracted;
                Assert.AreEqual(isfile.Name, extfile.Name);
                Assert.AreEqual(isfile.Extension, extfile.Extension);
                if (!(string.IsNullOrEmpty(isfile.ContentType)&&string.IsNullOrEmpty(extfile.ContentType)))
                    Assert.AreEqual(isfile.ContentType, extfile.ContentType);
                Assert.AreEqual(Convert.ToBase64String(isfile.Content), Convert.ToBase64String(extfile.Content));
            }
            else
                Assert.AreEqual(inputted, extracted);
        }
    }
}
