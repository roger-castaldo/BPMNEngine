﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using BPMNEngine.Interfaces.Variables;

namespace UnitTest
{
    [TestClass]
    public class ErrorHandling
    {
        private static readonly TimeSpan PROCESS_TIMEOUT = TimeSpan.FromSeconds(2);
        private static ConcurrentQueue<string> _cache;
        private static BusinessProcess _noErrorHandlingProcess;
        private static BusinessProcess _errorHandlingProcess;
        private const string _TEST_ID_NAME = "TestID";
        private const string _INVALID_ELEMENTS_ID = "InvalidElements";
        private const string _ERROR_MESSAGE_NAME = "ErrorMessage";
        private const string _DEFINED_ERROR_MESSAGE = "Test Error 1";

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _cache = new ConcurrentQueue<string>();
            _noErrorHandlingProcess = new BusinessProcess(Utility.LoadResourceDocument("ErrorHandling/no_error_handling.bpmn"),
                events:new BPMNEngine.DelegateContainers.ProcessEvents()
                {
                    Events=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Gateways=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Tasks=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    SubProcesses=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Processes=new()
                    {
                        Error= new OnProcessErrorEvent(ProcessError)
                    }
                },
                validations:new BPMNEngine.DelegateContainers.StepValidations()
                {
                    IsEventStartValid=new IsEventStartValid(IsEventStartValid),
                    IsFlowValid=new IsFlowValid(IsFlowValid),
                },
                tasks:new BPMNEngine.DelegateContainers.ProcessTasks()
                {
                    ProcessTask=new ProcessTask(ProcessTask)
                }
            );
            _errorHandlingProcess = new BusinessProcess(Utility.LoadResourceDocument("ErrorHandling/process_error_handling.bpmn"),
                events: new BPMNEngine.DelegateContainers.ProcessEvents()
                {
                    Events=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Gateways=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Tasks=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    SubProcesses=new ()
                    {
                        Error=new OnElementEvent(ErrorEvent)
                    },
                    Processes=new()
                    {
                        Error= new OnProcessErrorEvent(ProcessError)
                    }
                },
                validations: new BPMNEngine.DelegateContainers.StepValidations()
                {
                    IsEventStartValid=new IsEventStartValid(IsEventStartValid),
                    IsFlowValid=new IsFlowValid(IsFlowValid),
                },
                tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
                {
                    ProcessTask=new ProcessTask(ProcessTask)
                }
            );
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _noErrorHandlingProcess.Dispose();
            _cache = null;
        }

        private static bool IsElementValid(string id,object list)
        {
            if (list!=null)
                return !new List<string>((string[])list).Contains(id);
            return true;
        }

        private static void ProcessTask(ITask task)
        {
            if (!IsElementValid(task.ID, task.Variables[_INVALID_ELEMENTS_ID]))
                throw new Exception((task.Variables[_ERROR_MESSAGE_NAME]!=null ? (string)task.Variables[_ERROR_MESSAGE_NAME] : "Invalid Task"));
        }

        private static bool IsFlowValid(ISequenceFlow flow, IReadonlyVariables variables)
        {
            return IsElementValid(flow.ID, variables[_INVALID_ELEMENTS_ID]);
        }

        private static bool IsEventStartValid(IStepElement Event, IReadonlyVariables variables)
        {
            return IsElementValid(Event.ID, variables[_INVALID_ELEMENTS_ID]);
        }

        private static void ProcessError(IElement process, IElement sourceElement, IReadonlyVariables variables)
        {
            _cache.Enqueue(string.Format("{0}-{1}-{2}", [variables[_TEST_ID_NAME], process.ID, sourceElement.ID]));
        }

        private static void ErrorEvent(IStepElement element, IReadonlyVariables variables)
        {
            _cache.Enqueue(string.Format("{0}-{1}",[variables[_TEST_ID_NAME], element.ID]));
        }

        private static bool EventOccured(Guid instanceID, string name)
        {
            foreach (string str in _cache)
            {
                if (str==string.Format("{0}-{1}", [instanceID, name]))
                    return true;
            }
            return false;
        }

        private static bool EventOccured(Guid instanceID, string name,string subevent)
        {
            foreach (string str in _cache)
            {
                if (str==string.Format("{0}-{1}-{2}", [instanceID, name,subevent]))
                    return true;
            }
            return false;
        }

        [TestMethod()]
        public void TestEventErrorDelegate()
        {
            Guid guid = new("ed19e851-a0b2-4233-8030-d1d09fdbd1bd");
            IProcessInstance instance = _noErrorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "IntermediateCatchEvent_036z13e" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsFalse(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Thread.Sleep(5*1000);
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "IntermediateCatchEvent_036z13e"));
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "Process_1", "IntermediateCatchEvent_036z13e"));
        }

        [TestMethod()]
        public void TestTaskErrorDelegate()
        {
            Guid guid = new("c3a8ba48-2b39-4b85-962d-503ffc84b2e4");
            IProcessInstance instance = _noErrorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "Task_1t5xv8f" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsFalse(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Thread.Sleep(5*1000);
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "Task_1t5xv8f"));
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "Process_1", "Task_1t5xv8f"));
        }

        [TestMethod()]
        public void TestSubProcessErrorDelegate()
        {
            Guid guid = new("c24afff5-820a-4b76-808c-2268c5c807be");
            IProcessInstance instance = _noErrorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "Task_0e0f0l0" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsFalse(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Thread.Sleep(5*1000);
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "SubProcess_1mqrot2"));
        }

        [TestMethod()]
        public void TestGatewayErrorDelegate()
        {
            Guid guid = new("67718f12-3139-4f90-afc1-75956092339e");
            IProcessInstance instance = _noErrorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "SequenceFlow_096t69k", "SequenceFlow_1io01r8" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsFalse(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
            Thread.Sleep(5*1000);
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "ExclusiveGateway_1nkgv9w"));
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "Process_1", "ExclusiveGateway_1nkgv9w"));
            guid = new Guid("63087f93-f389-43c9-b11d-5e8ede39f953");
            instance = _noErrorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid}
            });
            Assert.IsNotNull(instance);
            Assert.IsFalse(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Thread.Sleep(5*1000);
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "ExclusiveGateway_1nkgv9w"));
            Assert.IsTrue(ErrorHandling.EventOccured(guid, "Process_1", "ExclusiveGateway_1nkgv9w"));
        }

        [TestMethod()]
        public void TestAnyErrorCatchEvent()
        {
            Guid guid = new("c195b4be-d337-4465-be5e-5087663567d4");
            IProcessInstance instance = _errorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "Task_11szmyl" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "IntermediateCatchEvent_1as7z3k"));
        }

        [TestMethod()]
        public void TestSubProcessErrorCatchEvent()
        {
            Guid guid = new("b1b1bc80-c015-4c2e-b882-18b11335fd55");
            IProcessInstance instance = _errorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "Task_067gf16" } }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance,waitTime:PROCESS_TIMEOUT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "IntermediateCatchEvent_1r5p299"));
        }

        [TestMethod()]
        public void TestBoundaryErrorCatchEvent()
        {
            Guid guid = new("b1b1bc80-c015-4c2e-b882-18b11335fd55");
            IProcessInstance instance = _errorHandlingProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_TEST_ID_NAME, guid},
                {_INVALID_ELEMENTS_ID,new string[]{ "Task_11szmyl" } },
                {_ERROR_MESSAGE_NAME,_DEFINED_ERROR_MESSAGE }
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance, waitTime: PROCESS_TIMEOUT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "BoundaryEvent_0hxboq6"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "SequenceFlow_08tdtwz"));
        }

        [TestMethod()]
        public void TestIntermediateCatchEventsAttachedToTask()
        {
            var process = new BusinessProcess(Utility.LoadResourceDocument("ErrorHandling/catch_event_checking.bpmn"));
            var instance = process.BeginProcess(new Dictionary<string, object>() { });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            var state = instance.CurrentState;

            Assert.IsFalse(Utility.StepCompleted(state, "Event_131x4qh"));
            Assert.IsFalse(Utility.StepCompleted(state, "Event_1dtsax8"));
            Assert.IsFalse(Utility.StepCompleted(state, "Event_0crhvev"));
            Assert.IsFalse(Utility.StepCompleted(state, "Event_1got9d6"));
            Assert.IsTrue(Utility.StepCompleted(state, "Flow_1sww23a"));
        }
    }
}
