using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System.Collections.Generic;
using System.Xml;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace UnitTest.Delegates
{
    [TestClass]
    public class Validation
    {
        private static BusinessProcess _pathChecksProcess;
        private const string _VALID_PATHS_NAME = "ValidPaths";
        private const string _PROCESS_BLOCKED_NAME = "IsProcessBlocked";

        private static bool IsEventStartValid(IStepElement Event, IReadonlyVariables variables)
        {
            return new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(Event.ID);
        }

        private static bool IsProcessStartValid(IElement process, IReadonlyVariables variables)
        {
            return (variables[_PROCESS_BLOCKED_NAME]==null ||(bool)variables[_PROCESS_BLOCKED_NAME]);
        }

        private static bool IsFlowValid(ISequenceFlow flow, IReadonlyVariables variables)
        {
            return new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(flow.ID);
        }

        private static bool InstanceIsEventStartValid(IStepElement Event, IReadonlyVariables variables)
        {
            return !new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(Event.ID);
        }

        private static bool InstanceIsProcessStartValid(IElement process, IReadonlyVariables variables)
        {
            return !(variables[_PROCESS_BLOCKED_NAME]==null ||(bool)variables[_PROCESS_BLOCKED_NAME]);
        }

        private static bool InstanceIsFlowValid(ISequenceFlow flow, IReadonlyVariables variables)
        {
            return !new List<string>((string[])variables[_VALID_PATHS_NAME]).Contains(flow.ID);
        }

        [ClassInitialize()]
        public static void Initialize(TestContext testContext)
        {
            _pathChecksProcess = new BusinessProcess(Utility.LoadResourceDocument("Delegates/path_valid_checks.bpmn"),
                validations: new BPMNEngine.DelegateContainers.StepValidations()
                {
                    IsFlowValid= new IsFlowValid(IsFlowValid),
                    IsProcessStartValid=new IsProcessStartValid(IsProcessStartValid),
                    IsEventStartValid=new IsEventStartValid(IsEventStartValid)
                }
            );
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            _pathChecksProcess.Dispose();
        }

        private static bool EventOccured(XmlDocument state,string id,string status)
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
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Started"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", "Succeeded"));

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
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsEventStartValid=new IsEventStartValid(InstanceIsEventStartValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Started"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", "Succeeded"));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_0fbfgne", "StartEvent_1", "SequenceFlow_1sl9l6m" }}
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsEventStartValid=new IsEventStartValid(InstanceIsEventStartValid)
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
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsFlowValid=new IsFlowValid(InstanceIsFlowValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Started"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsFalse(EventOccured(doc, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsTrue(EventOccured(doc, "SequenceFlow_0ijuqxx", "Succeeded"));
        }

        [TestMethod]
        public void TestOverrideIsProcessStartValid()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
                {_PROCESS_BLOCKED_NAME,false }
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsProcessStartValid=new IsProcessStartValid(InstanceIsProcessStartValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Started"));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Started"));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", "Succeeded"));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", "Succeeded"));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", "Succeeded"));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1", "SequenceFlow_1sl9l6m" }},
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsProcessStartValid=new IsProcessStartValid(InstanceIsProcessStartValid)
            });
            Assert.IsNull(instance);
        }
    }
}
