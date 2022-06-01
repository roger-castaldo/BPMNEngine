using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class Delegates
    {
        private static Dictionary<Guid, List<string>> _cache;
        private static BusinessProcess _startCompleteProcess;
        private static BusinessProcess _pathChecksProcess;
        private const string _TEST_ID_NAME = "TestID";
        private const string _VALID_PATHS_NAME = "ValidPaths";
        private const string _PROCESS_BLOCKED_NAME = "IsProcessBlocked";

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _startCompleteProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/start_complete_triggers.bpmn"),
                onEventStarted:new OnElementEvent(_elementStartedCompleted),
                onEventCompleted:new OnElementEvent(_elementStartedCompleted),
                onGatewayStarted:new OnElementEvent(_elementStartedCompleted),
                onGatewayCompleted:new OnElementEvent(_elementStartedCompleted),
                onSequenceFlowCompleted:new OnFlowComplete(_flowCompleted),
                onMessageFlowCompleted: new OnFlowComplete(_flowCompleted),
                onSubProcessCompleted:new OnElementEvent(_elementStartedCompleted),
                onSubProcessStarted:new OnElementEvent(_elementStartedCompleted),
                onTaskCompleted:new OnElementEvent(_elementStartedCompleted),
                onTaskStarted:new OnElementEvent(_elementStartedCompleted)
            );
            _cache = new Dictionary<Guid, List<string>>();
            _pathChecksProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/path_valid_checks.bpmn"),
                onEventStarted: new OnElementEvent(_elementStartedCompleted),
                onEventCompleted: new OnElementEvent(_elementStartedCompleted),
                onGatewayStarted: new OnElementEvent(_elementStartedCompleted),
                onGatewayCompleted: new OnElementEvent(_elementStartedCompleted),
                onSequenceFlowCompleted: new OnFlowComplete(_flowCompleted),
                onMessageFlowCompleted: new OnFlowComplete(_flowCompleted),
                onSubProcessCompleted: new OnElementEvent(_elementStartedCompleted),
                onSubProcessStarted: new OnElementEvent(_elementStartedCompleted),
                onTaskCompleted: new OnElementEvent(_elementStartedCompleted),
                onTaskStarted: new OnElementEvent(_elementStartedCompleted),
                isFlowValid: new IsFlowValid(_isFlowValid),
                isProcessStartValid:new IsProcessStartValid(_isProcessStartValid),
                isEventStartValid:new IsEventStartValid(_isEventStartValid)
            );
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
            lock (_cache)
            {
                List<string> tmp = new List<string>();
                if (_cache.ContainsKey((Guid)variables[_TEST_ID_NAME]))
                {
                    tmp = _cache[(Guid)variables[_TEST_ID_NAME]];
                    _cache.Remove((Guid)variables[_TEST_ID_NAME]);
                }
                tmp.Add(element.id);
                _cache.Add((Guid)variables[_TEST_ID_NAME], tmp);
            }
        }

        private static void _elementStartedCompleted(IStepElement element, IReadonlyVariables variables)
        {
            lock (_cache)
            {
                List<string> tmp = new List<string>();
                if (_cache.ContainsKey((Guid)variables[_TEST_ID_NAME]))
                {
                    tmp = _cache[(Guid)variables[_TEST_ID_NAME]];
                    _cache.Remove((Guid)variables[_TEST_ID_NAME]);
                }
                tmp.Add(element.id);
                _cache.Add((Guid)variables[_TEST_ID_NAME], tmp);
            }
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _startCompleteProcess.Dispose();
            _pathChecksProcess.Dispose();
            _cache = null;
        }

        private int _CountCacheOccurences(Guid instanceID,string name)
        {
            int ret = 0;
            lock (_cache)
            {
                if (_cache.ContainsKey(instanceID))
                {
                    foreach (string str in _cache[instanceID])
                        ret+=(str==name ? 1 : 0);
                }
            }
            return ret;
        }

        private static readonly Dictionary<string,int> _entryCounts = new Dictionary<string, int>(){
            { "StartEvent_1",2 },
            {"EndEvent_1d1a99g",2},
            {"ServiceTask_19kcbag",2},
            {"ParallelGateway_197wuek",2},
            {"ParallelGateway_1ud7d8q",2},
            {"ScriptTask_0a8en2y",2},
            {"SubProcess_1fk97di",2},
            {"StartEvent_1sttpuv",2},
            {"EndEvent_0exopsv",2},
            {"Task_12seef8",2},
            {"SequenceFlow_1fnfz4x",1 },
            {"SequenceFlow_1qrw9p3",1 },
            {"SequenceFlow_1e88oob",1 },
            {"SequenceFlow_1g5hpce",1 },
            {"SequenceFlow_143qney",1 },
            {"SequenceFlow_1yaim57",1 },
            {"SequenceFlow_09hc5op",1 },
            {"SequenceFlow_1w3bfnx",1 },
            {"SequenceFlow_0zrlx9l",1 }
        };

        [TestMethod]
        public void TestDelegatesTriggered()
        {
            Guid guid = new Guid("8782dc9a-915a-40af-ac39-b7d808fda926");
            IProcessInstance instance = _startCompleteProcess.BeginProcess(new Dictionary<string, object> { { _TEST_ID_NAME, guid } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));
            int total = 0;
            foreach (string key in _entryCounts.Keys)
            {
                total+=_entryCounts[key];
                Assert.AreEqual(_entryCounts[key], _CountCacheOccurences(guid, key));
            }
            lock (_cache)
            {
                Assert.AreEqual(total, _cache[guid].Count);
            }
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
            Assert.AreEqual(2, _CountCacheOccurences(guid, "StartEvent_1"));
            Assert.AreEqual(0, _CountCacheOccurences(guid, "StartEvent_0fbfgne"));
            Assert.AreEqual(1, _CountCacheOccurences(guid, "SequenceFlow_1sl9l6m"));
            Assert.AreEqual(0, _CountCacheOccurences(guid, "SequenceFlow_0ijuqxx"));

            guid = new Guid("da2a2dd4-c299-417a-896b-eefa15ad9b8f");
            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                { _TEST_ID_NAME,guid },
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
                {_PROCESS_BLOCKED_NAME,false }
            });
            Assert.IsNull(instance);
        }

    }
}
