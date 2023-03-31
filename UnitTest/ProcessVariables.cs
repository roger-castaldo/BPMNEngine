using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Threading;
using System.Linq;
using Esprima.Ast;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

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

        private Dictionary<string, object> _TestProcessVariable(string variableName, object variableValue)
        {
            IProcessInstance inst = _process.BeginProcess(new Dictionary<string, object>() { { variableName, variableValue } });

            int cnt = 0;
            IUserTask task = inst.GetUserTask("UserTask_15dj2au");
            while (cnt<5 && task==null)
            {
                Thread.Sleep(1000);
                task = inst.GetUserTask("UserTask_15dj2au");
                cnt++;
            }
            Assert.IsNotNull(task);
            if (variableValue==null)
                Assert.AreEqual(0, task.Variables.Keys.Count());
            else
            {
                Assert.AreEqual(1, task.Variables.Keys.Count());
                _CompareVariableValue(variableValue, task.Variables[variableName]);
            }
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(inst));
            return inst.CurrentVariables;
        }

        [TestMethod]
        public void TestDateTimeVariable()
        {
            var variableName = "TestDateTime";
            var variableValue = DateTime.Now;
            var variableArray = new DateTime[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestIntegerVariable()
        {
            var variableName = "TestInteger";
            var variableValue = int.MinValue;
            var variableArray = new int[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestShortVariable()
        {
            var variableName = "TestShort";
            var variableValue = short.MinValue;
            var variableArray = new short[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestLongVariable()
        {
            var variableName = "TestLong";
            var variableValue = long.MinValue;
            var variableArray = new long[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestUnsignedIntegerVariable()
        {
            var variableName = "TestUnsignedInteger";
            var variableValue = uint.MinValue;
            var variableArray = new uint[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestUnsignedShortVariable()
        {
            var variableName = "TestUnsignedShort";
            var variableValue = ushort.MinValue;
            var variableArray = new ushort[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestUnsignedLongVariable()
        {
            var variableName = "TestUnsignedLong";
            var variableValue = ulong.MaxValue;
            var variableArray = new ulong[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestDoubleVariable()
        {
            var variableName = "TestDouble";
            var variableValue = double.MinValue;
            var variableArray = new double[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestDecimalVariable()
        {
            var variableName = "TestDecimal";
            var variableValue = decimal.MinValue;
            var variableArray = new decimal[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestStringVariable()
        {
            var variableName = "TestString";
            var variableValue = "This is a test string";
            var variableArray = new string[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestCharVariable()
        {
            var variableName = "TestChar";
            var variableValue = 'c';
            var variableArray = new char[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestBooleanVariable()
        {
            var variableName = "TestBoolean";
            var variableValue = true;
            var variableArray = new bool[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestFloatVariable()
        {
            var variableName = "TestFloat";
            var variableValue = float.MinValue;
            var variableArray = new float[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestByteVariable()
        {
            var variableName = "TestByte";
            var variableValue = System.Text.ASCIIEncoding.ASCII.GetBytes("Testing 12345");
            var variableArray = new byte[][] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestNullVariable()
        {
            var variableName = "TestNull";
            object variableValue = null;
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsFalse(results.ContainsKey(variableName));
            Assert.AreEqual(0,results.Count);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestFileVariable()
        {
            var variableName = "TestFile";
            Stream str = Utility.LoadResource("DiagramLoading/start_to_stop.bpmn");
            byte[] data = new byte[str.Length];
            str.Read(data,0, data.Length);
            str.Close();
            var variableValue = new sFile("start_to_stop","bpmn","text/xml",data);
            var variableArray = new sFile[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
        }

        [TestMethod]
        public void TestGuidVariable()
        {
            var variableName = "TestGuid";
            var variableValue = new Guid("a4966f14-0108-48ba-a793-8be9982bc411");
            var variableArray = new Guid[] { variableValue, variableValue };
            Dictionary<string, object> results = _TestProcessVariable(variableName, variableValue);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableValue, (results.ContainsKey(variableName) ? results[variableName] : null));

            results = _TestProcessVariable(variableName, variableArray);
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(variableName));
            Assert.IsTrue(results.Count==1);
            _CompareVariableValue(variableArray, (results.ContainsKey(variableName) ? results[variableName] : null));
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

            var taskVariables = new Dictionary<string, object>();

            IProcessInstance inst = _process.BeginProcess(variables);
            Assert.IsNotNull(inst);

            variables.Remove("TestNull");

            Thread.Sleep(1000);
            IUserTask task = inst.GetUserTask("UserTask_15dj2au");
            Assert.IsNotNull(task);
            foreach (string key in task.Variables.Keys)
                taskVariables.Add(key, task.Variables[key]);
            task.MarkComplete();

            Assert.IsTrue(Utility.WaitForCompletion(inst));

            Assert.IsFalse(taskVariables.ContainsKey("TestNull"));

            _CompareVariableSets(variables, taskVariables);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(inst.CurrentState.AsXMLDocument);

            _CompareVariableSets(variables, BusinessProcess.ExtractProcessVariablesFromStateDocument(doc));

            inst = _process.LoadState(doc);
            Assert.IsNotNull(inst);
            _CompareVariableSets(variables, inst.CurrentVariables);
        }

        private void _CompareVariableSets(Dictionary<string, object> inputted, Dictionary<string, object> extracted)
        {
            Assert.AreEqual(inputted.Count(), extracted.Count);
            foreach (string str in inputted.Keys)
            {
                Assert.IsTrue(extracted.ContainsKey(str));
                _CompareVariableValue(inputted[str], extracted[str]);
            }
        }

        private void _CompareVariableValue(object inputted,object extracted)
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
                    _CompareVariableValue(inp.value, extGroups.FirstOrDefault(grp => grp.index==inp.index).value);
            }
            else if (inputted is DateTime)
            {
                Assert.IsInstanceOfType(extracted, typeof(DateTime));
                Assert.AreEqual(inputted.ToString(), extracted.ToString());
            }
            else if (inputted is byte[])
            {
                Assert.IsInstanceOfType(extracted, typeof(byte));
                Assert.AreEqual(Convert.ToBase64String((byte[])inputted), Convert.ToBase64String((byte[])extracted));
            }
            else if (inputted is sFile)
            {
                Assert.IsInstanceOfType(extracted, typeof(sFile));
                var isfile = (sFile)inputted;
                var extfile = (sFile)extracted;
                Assert.AreEqual(isfile.Name, extfile.Name);
                Assert.AreEqual(isfile.Extension, extfile.Extension);
                Assert.AreEqual(isfile.ContentType, extfile.ContentType);
                Assert.AreEqual(Convert.ToBase64String(isfile.Content), Convert.ToBase64String(extfile.Content));
            }
            else
                Assert.AreEqual(inputted, extracted);
        }
    }
}
