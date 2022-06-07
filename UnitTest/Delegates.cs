using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class Delegates
    {
        private static ConcurrentQueue<string> _cache;
        private static BusinessProcess _startCompleteProcess;
        private static BusinessProcess _pathChecksProcess;
        private static BusinessProcess _taskCallsProcess;
        private const string _TEST_ID_NAME = "TestID";
        private const string _VALID_PATHS_NAME = "ValidPaths";
        private const string _PROCESS_BLOCKED_NAME = "IsProcessBlocked";
        private const string _TASK_LIST_VARIABLE = "TasksExecuted";
        private const string _TASK_USER_ID = "MyUser";

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _cache = new ConcurrentQueue<string>();
            _startCompleteProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/start_complete_triggers.bpmn"),
                onEventStarted:new OnElementEvent(_elementStarted),
                onEventCompleted:new OnElementEvent(_elementCompleted),
                onGatewayStarted:new OnElementEvent(_elementStarted),
                onGatewayCompleted:new OnElementEvent(_elementCompleted),
                onSequenceFlowCompleted:new OnFlowComplete(_flowCompleted),
                onMessageFlowCompleted: new OnFlowComplete(_flowCompleted),
                onSubProcessCompleted:new OnElementEvent(_elementCompleted),
                onSubProcessStarted:new OnElementEvent(_elementStarted),
                onTaskCompleted:new OnElementEvent(_elementCompleted),
                onTaskStarted:new OnElementEvent(_elementStarted)
            );
            _pathChecksProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/path_valid_checks.bpmn"),
                onEventStarted: new OnElementEvent(_elementStarted),
                onEventCompleted: new OnElementEvent(_elementCompleted),
                onGatewayStarted: new OnElementEvent(_elementStarted),
                onGatewayCompleted: new OnElementEvent(_elementCompleted),
                onSequenceFlowCompleted: new OnFlowComplete(_flowCompleted),
                onMessageFlowCompleted: new OnFlowComplete(_flowCompleted),
                onSubProcessCompleted: new OnElementEvent(_elementCompleted),
                onSubProcessStarted: new OnElementEvent(_elementStarted),
                onTaskCompleted: new OnElementEvent(_elementCompleted),
                onTaskStarted: new OnElementEvent(_elementStarted),
                isFlowValid: new IsFlowValid(_isFlowValid),
                isProcessStartValid:new IsProcessStartValid(_isProcessStartValid),
                isEventStartValid:new IsEventStartValid(_isEventStartValid)
            );
            _taskCallsProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/task_checks.bpmn"),
                onEventStarted: new OnElementEvent(_elementStarted),
                onEventCompleted: new OnElementEvent(_elementCompleted),
                onGatewayStarted: new OnElementEvent(_elementStarted),
                onGatewayCompleted: new OnElementEvent(_elementCompleted),
                onSequenceFlowCompleted: new OnFlowComplete(_flowCompleted),
                onMessageFlowCompleted: new OnFlowComplete(_flowCompleted),
                onSubProcessCompleted: new OnElementEvent(_elementCompleted),
                onSubProcessStarted: new OnElementEvent(_elementStarted),
                onTaskCompleted: new OnElementEvent(_elementCompleted),
                onTaskStarted: new OnElementEvent(_elementStarted),
                processBusinessRuleTask:new ProcessTask(_processTask),
                processRecieveTask:new ProcessTask(_processTask),
                processScriptTask: new ProcessTask(_processTask),
                processSendTask:new ProcessTask(_processTask),
                processServiceTask:new ProcessTask(_processTask),
                processTask:new ProcessTask(_processTask),
                beginManualTask:new StartManualTask(_startManualTask),
                beginUserTask:new StartUserTask(_startUserTask)
            );
        }

        private static void _startUserTask(IUserTask task)
        {
            if (task.Variables[_TASK_LIST_VARIABLE]==null)
                task.Variables[_TASK_LIST_VARIABLE] = new string[] { };
            List<string> tmp = new List<string>((string[])task.Variables[_TASK_LIST_VARIABLE]);
            tmp.Add(task.id);
            task.Variables[_TASK_LIST_VARIABLE] = tmp.ToArray();
            task.UserID = _TASK_USER_ID;
            task.MarkComplete();
        }

        private static void _startManualTask(IManualTask task)
        {
            if (task.Variables[_TASK_LIST_VARIABLE]==null)
                task.Variables[_TASK_LIST_VARIABLE] = new string[] { };
            List<string> tmp = new List<string>((string[])task.Variables[_TASK_LIST_VARIABLE]);
            tmp.Add(task.id);
            task.Variables[_TASK_LIST_VARIABLE] = tmp.ToArray();
            task.MarkComplete();
        }

        private static void _processTask(ITask task)
        {
            if (task.Variables[_TASK_LIST_VARIABLE]==null)
                task.Variables[_TASK_LIST_VARIABLE] = new string[] { };
            List<string> tmp = new List<string>((string[])task.Variables[_TASK_LIST_VARIABLE]);
            tmp.Add(task.id);
            task.Variables[_TASK_LIST_VARIABLE] = tmp.ToArray();
        }

        private static bool _isEventStartValid(IStepElement Event, IReadonlyVariables variables)
        {
            return new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(Event.id);
        }

        private static bool _isProcessStartValid(IElement process, IReadonlyVariables variables)
        {
            return (variables[_PROCESS_BLOCKED_NAME]==null ? true : (bool)variables[_PROCESS_BLOCKED_NAME]);
        }

        private static bool _isFlowValid(ISequenceFlow flow, IReadonlyVariables variables)
        {
            return new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(flow.id);
        }

        private static void _flowCompleted(IElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void _elementStarted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Started", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void _elementCompleted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _startCompleteProcess.Dispose();
            _pathChecksProcess.Dispose();
            _cache = null;
        }

        private bool _EventOccured(Guid instanceID,string name,string evnt)
        {
            foreach (string str in _cache)
            {
                if (str==string.Format("{0}_{1}_{2}", new object[] { instanceID, name, evnt }))
                    return true;
            }
            return false;
        }

        [TestMethod]
        public void TestDelegatesTriggered()
        {
            Guid guid = new Guid("8782dc9a-915a-40af-ac39-b7d808fda926");
            IProcessInstance instance = _startCompleteProcess.BeginProcess(new Dictionary<string, object> { { _TEST_ID_NAME, guid } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));
            System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(_EventOccured(guid, "StartEvent_1", "Started"));
            Assert.IsTrue(_EventOccured(guid, "StartEvent_1", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "EndEvent_1d1a99g", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "EndEvent_1d1a99g", "Started"));
            Assert.IsTrue(_EventOccured(guid, "ServiceTask_19kcbag", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "ServiceTask_19kcbag", "Started"));
            Assert.IsTrue(_EventOccured(guid, "ParallelGateway_197wuek", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "ParallelGateway_197wuek", "Started"));
            Assert.IsTrue(_EventOccured(guid, "ParallelGateway_1ud7d8q", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "ParallelGateway_1ud7d8q", "Started"));
            Assert.IsTrue(_EventOccured(guid, "ScriptTask_0a8en2y", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "ScriptTask_0a8en2y", "Started"));
            Assert.IsTrue(_EventOccured(guid, "SubProcess_1fk97di", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SubProcess_1fk97di", "Started"));
            Assert.IsTrue(_EventOccured(guid, "StartEvent_1sttpuv", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "StartEvent_1sttpuv", "Started"));
            Assert.IsTrue(_EventOccured(guid, "EndEvent_0exopsv", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "EndEvent_0exopsv", "Started"));
            Assert.IsTrue(_EventOccured(guid, "Task_12seef8", "Started"));
            Assert.IsTrue(_EventOccured(guid, "Task_12seef8", "Completed"));

            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1fnfz4x", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1qrw9p3", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1e88oob", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1g5hpce", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_143qney", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1yaim57", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_09hc5op", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1w3bfnx", "Completed"));
            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_0zrlx9l", "Completed"));

            int cnt = 0;
            foreach (string str in _cache)
            {
                if (str.StartsWith(guid.ToString()+"_"))
                    cnt++;
            }
            Assert.AreEqual(29, cnt);
        }

        [TestMethod]
        public void TestPathIsValid()
        {
            Guid guid = new Guid("b420ba32-fd0b-4a67-ba7f-2606b8cd994d");
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                { _TEST_ID_NAME,guid },
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }}
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));
            System.Threading.Thread.Sleep(5000);

            Assert.IsTrue(_EventOccured(guid, "StartEvent_1", "Started"));
            Assert.IsTrue(_EventOccured(guid, "StartEvent_1", "Completed"));
            Assert.IsFalse(_EventOccured(guid, "StartEvent_0fbfgne", "Started"));
            Assert.IsFalse(_EventOccured(guid, "StartEvent_0fbfgne", "Completed"));

            Assert.IsTrue(_EventOccured(guid, "SequenceFlow_1sl9l6m", "Completed"));
            Assert.IsFalse(_EventOccured(guid, "SequenceFlow_0ijuqxx", "Completed"));
            
            guid = new Guid("da2a2dd4-c299-417a-896b-eefa15ad9b8f");
            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                { _TEST_ID_NAME,guid },
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
                {_PROCESS_BLOCKED_NAME,false }
            });
            Assert.IsNull(instance);
        }

        [TestMethod()]
        public void TestTaskCallbacks()
        {
            Guid guid = new Guid("b799bd4c-19ea-48e5-87ee-c028a948c460");
            IProcessInstance instance = _taskCallsProcess.BeginProcess(new Dictionary<string, object>()
            {
                { _TEST_ID_NAME,guid }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            Dictionary<string, object> results = instance.CurrentVariables;
            Assert.IsNotNull(results);
            Assert.IsTrue(results.ContainsKey(_TASK_LIST_VARIABLE));
            Assert.IsInstanceOfType(results[_TASK_LIST_VARIABLE], typeof(string[]));
            List<string> tmp = new List<string>((string[])results[_TASK_LIST_VARIABLE]);
            Assert.AreEqual(7, tmp.Count);
            Assert.IsTrue(tmp.Contains("Task_1koadgj"));
            Assert.IsTrue(tmp.Contains("SendTask_1i9s13s"));
            Assert.IsTrue(tmp.Contains("ReceiveTask_0xcb37w"));
            Assert.IsTrue(tmp.Contains("UserTask_1997n3l"));
            Assert.IsTrue(tmp.Contains("ManualTask_15lp0xy"));
            Assert.IsTrue(tmp.Contains("BusinessRuleTask_14b2ep0"));
            Assert.IsTrue(tmp.Contains("ServiceTask_1w2aowp"));
            Assert.IsNotNull(instance.CurrentState.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='UserTask_1997n3l'][@status='Succeeded'][@CompletedByID='{0}']", _TASK_USER_ID)));
        }
    }
}
