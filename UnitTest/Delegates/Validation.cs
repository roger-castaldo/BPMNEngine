using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTest.Delegates
{
    [TestClass]
    public class Validation
    {
        private static BusinessProcess _pathChecksProcess;
        private const string _VALID_PATHS_NAME = "ValidPaths";
        private const string _PROCESS_BLOCKED_NAME = "IsProcessBlocked";

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

        private static bool _instanceIsEventStartValid(IStepElement Event, IReadonlyVariables variables)
        {
            return !new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(Event.id);
        }

        private static bool _instanceIsProcessStartValid(IElement process, IReadonlyVariables variables)
        {
            return !(variables[_PROCESS_BLOCKED_NAME]==null ? true : (bool)variables[_PROCESS_BLOCKED_NAME]);
        }

        private static bool _instanceIsFlowValid(ISequenceFlow flow, IReadonlyVariables variables)
        {
            return !new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(flow.id);
        }

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _pathChecksProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/path_valid_checks.bpmn"),
                validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
                {
                    IsFlowValid= new IsFlowValid(_isFlowValid),
                    IsProcessStartValid=new IsProcessStartValid(_isProcessStartValid),
                    IsEventStartValid=new IsEventStartValid(_isEventStartValid)
                }
            );
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _pathChecksProcess.Dispose();
        }

        private static bool _EventOccured(XmlDocument state,string id,string status)
        {
            return state.SelectSingleNode(string.Format("/ProcessState/ProcessPath/sPathEntry[@elementID='{0}'][@status='{1}']", id,status))!=null;
        }
        

        [TestMethod]
        public void TestPathIsValid()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }}
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Waiting"));
            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Waiting"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "SequenceFlow_0ijuqxx", "Succeeded"));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
                {_PROCESS_BLOCKED_NAME,false }
            });
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TestOverrideIsEventStartValid()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_0fbfgne","SequenceFlow_1sl9l6m","SequenceFlow_0hhf11n" }}
            },
            validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
            {
                IsEventStartValid=new IsEventStartValid(_instanceIsEventStartValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Waiting"));
            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Waiting"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "SequenceFlow_0ijuqxx", "Succeeded"));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_0fbfgne", "StartEvent_1", "SequenceFlow_1sl9l6m" }}
            },
            validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
            {
                IsEventStartValid=new IsEventStartValid(_instanceIsEventStartValid)
            });
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void TestOverrideIsFlowValid()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }}
            },
            validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
            {
                IsFlowValid=new IsFlowValid(_instanceIsFlowValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Waiting"));
            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Waiting"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsFalse(_EventOccured(instance.CurrentState, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsTrue(_EventOccured(instance.CurrentState, "SequenceFlow_0ijuqxx", "Succeeded"));
        }

        [TestMethod]
        public void TestOverrideIsProcessStartValid()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
                {_PROCESS_BLOCKED_NAME,false }
            },
            validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
            {
                IsProcessStartValid=new IsProcessStartValid(_instanceIsProcessStartValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Waiting"));
            Assert.IsTrue(_EventOccured(instance.CurrentState, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Waiting"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(_EventOccured(instance.CurrentState, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(_EventOccured(instance.CurrentState, "SequenceFlow_0ijuqxx", "Succeeded"));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
            },
            validations: new Org.Reddragonit.BpmEngine.DelegateContainers.StepValidations()
            {
                IsProcessStartValid=new IsProcessStartValid(_instanceIsProcessStartValid)
            });
            Assert.IsNull(instance);
        }
    }
}
