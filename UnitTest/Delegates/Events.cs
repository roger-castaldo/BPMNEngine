using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace UnitTest.Delegates
{
    [TestClass]
    public class ElementProperties
    {
        private static ConcurrentQueue<string> _cache;
        private static BusinessProcess _startCompleteProcess;
        private const string _TEST_ID_NAME = "TestID";
        private const int STEP_COUNTS = 38;

        private static void FlowCompleted(IElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void ElementStarted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Started", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void ElementCompleted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void InstanceFlowCompleted(IElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("Instance_{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void InstanceElementStarted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("Instance_{0}_{1}_Started", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static void InstanceElementCompleted(IStepElement element, IReadonlyVariables variables)
        {
            Assert.IsNotNull(variables[_TEST_ID_NAME]);
            _cache.Enqueue(string.Format("Instance_{0}_{1}_Completed", new object[] { variables[_TEST_ID_NAME], element.id }));
        }

        private static bool IsProcessStartValid(IElement process, IReadonlyVariables variables)
        {
            if (process.id=="Process_1" && !EventOccured((Guid)variables[_TEST_ID_NAME], "StartEvent_1", "Started"))
                return true;
            else if ((process.id=="Process_1vk3guw"||process.id=="SubProcess_1fk97di") && EventOccured((Guid)variables[_TEST_ID_NAME], "StartEvent_1", "Started"))
                return true;
            return false;
        }

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _cache = new ConcurrentQueue<string>();
            _startCompleteProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/start_complete_triggers.bpmn"),
                validations: new BPMNEngine.DelegateContainers.StepValidations()
                {
                    IsProcessStartValid=new IsProcessStartValid(IsProcessStartValid)
                },
                events: new BPMNEngine.DelegateContainers.ProcessEvents()
                {
                    Events=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(ElementStarted),
                        Completed=new OnElementEvent(ElementCompleted)
                    },
                    Gateways=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(ElementStarted),
                        Completed=new OnElementEvent(ElementCompleted)
                    },
                    SubProcesses=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(ElementStarted),
                        Completed=new OnElementEvent(ElementCompleted)
                    },
                    Tasks=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(ElementStarted),
                        Completed=new OnElementEvent(ElementCompleted)
                    },
                    Flows=new BPMNEngine.DelegateContainers.ProcessEvents.FlowEvents()
                    {
                        SequenceFlow=new OnFlowComplete(FlowCompleted),
                        MessageFlow=new OnFlowComplete(FlowCompleted)
                    }
                }, logging: new BPMNEngine.DelegateContainers.ProcessLogging()
                {
                    LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                    {
                        System.Diagnostics.Debug.WriteLine(exception.Message);
                    }
                }
            );
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _startCompleteProcess.Dispose();
            _cache = null;
        }

        private static bool EventOccured(Guid instanceID, string name, string evnt,bool instanceVersion=false)
        {
            foreach (string str in _cache)
            {
                if (str==string.Format("{0}{1}_{2}_{3}", new object[] { (instanceVersion ? "Instance_" : ""),instanceID, name, evnt }))
                    return true;
            }
            return false;
        }

        [TestMethod]
        public void TestDelegatesTriggered()
        {
            Guid guid = Guid.NewGuid();
            IProcessInstance instance = _startCompleteProcess.BeginProcess(new Dictionary<string, object> { { _TEST_ID_NAME, guid } });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Started"));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Started"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Started"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Started"));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Started"));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Started"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Completed"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Started"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Started"));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Started"));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Started"));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Started"));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Started"));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Completed"));

            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1fnfz4x", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1qrw9p3", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1e88oob", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1g5hpce", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_143qney", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1yaim57", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_09hc5op", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1w3bfnx", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_0zrlx9l", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_10gw13c", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_06vbv76", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_0q597g1", "Completed"));
            

            int cnt = 0;
            foreach (string str in _cache)
            {
                if (str.StartsWith(guid.ToString()+"_"))
                    cnt++;
            }
            Assert.AreEqual(STEP_COUNTS, cnt);
        }

        [TestMethod]
        public void TestDelegatesTriggeredInstance()
        {
            Guid guid = Guid.NewGuid();
            IProcessInstance instance = _startCompleteProcess.BeginProcess(new Dictionary<string, object> { { _TEST_ID_NAME, guid } },
                events: new BPMNEngine.DelegateContainers.ProcessEvents()
                {
                    Events=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(InstanceElementStarted),
                        Completed=new OnElementEvent(InstanceElementCompleted)
                    },
                    Gateways=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(InstanceElementStarted),
                        Completed=new OnElementEvent(InstanceElementCompleted)
                    },
                    SubProcesses=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(InstanceElementStarted),
                        Completed=new OnElementEvent(InstanceElementCompleted)
                    },
                    Tasks=new BPMNEngine.DelegateContainers.ProcessEvents.BasicEvents()
                    {
                        Started=new OnElementEvent(InstanceElementStarted),
                        Completed=new OnElementEvent(InstanceElementCompleted)
                    },
                    Flows=new BPMNEngine.DelegateContainers.ProcessEvents.FlowEvents()
                    {
                        SequenceFlow=new OnFlowComplete(InstanceFlowCompleted),
                        MessageFlow=new OnFlowComplete(InstanceFlowCompleted)
                    }
                });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));
            System.Threading.Thread.Sleep(5000);
            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Started"));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Started"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Started"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Started"));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Completed"));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Started"));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Started"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Completed"));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Started"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Completed"));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Started"));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Started"));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Started"));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Started"));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Started"));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Completed"));

            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1fnfz4x", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1qrw9p3", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1e88oob", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1g5hpce", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_143qney", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1yaim57", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_09hc5op", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1w3bfnx", "Completed"));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_0zrlx9l", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_10gw13c", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_06vbv76", "Completed"));
            Assert.IsTrue(EventOccured(guid, "Flow_0q597g1", "Completed"));

            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Started",instanceVersion:true));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "EndEvent_1d1a99g", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ServiceTask_19kcbag", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_197wuek", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ParallelGateway_1ud7d8q", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Completed" , instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "ScriptTask_0a8en2y", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SubProcess_1fk97di", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "StartEvent_1sttpuv", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "EndEvent_0exopsv", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Task_12seef8", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Event_0gtdmdg", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Activity_1iw9xdd", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Started", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Event_1oltx11", "Completed", instanceVersion: true));

            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1fnfz4x", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1qrw9p3", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1e88oob", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1g5hpce", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_143qney", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1yaim57", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_09hc5op", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_1w3bfnx", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "SequenceFlow_0zrlx9l", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Flow_10gw13c", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Flow_06vbv76", "Completed", instanceVersion: true));
            Assert.IsTrue(EventOccured(guid, "Flow_0q597g1", "Completed", instanceVersion: true));

            int cnt = 0;
            foreach (string str in _cache)
            {
                if (str.StartsWith(guid.ToString()+"_"))
                    cnt++;
            }
            Assert.AreEqual(STEP_COUNTS, cnt);

            cnt = 0;
            foreach (string str in _cache)
            {
                if (str.StartsWith("Instance_"+guid.ToString()+"_"))
                    cnt++;
            }
            Assert.AreEqual(STEP_COUNTS, cnt);
        }
    }
}
