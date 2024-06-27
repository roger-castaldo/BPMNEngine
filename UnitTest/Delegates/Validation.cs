using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml;

namespace UnitTest.Delegates
{
    [TestClass]
    public class Validation
    {
        private static BusinessProcess _pathChecksProcess;
        private const string _VALID_PATHS_NAME = "ValidPaths";
        private const string _PROCESS_BLOCKED_NAME = "IsProcessBlocked";
        private const string _CONDITION_CHECK = "FlowConditionCheck";

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
            if (variables[_CONDITION_CHECK]!=null)
                return string.Equals(flow.ConditionExpression??string.Empty, variables[_CONDITION_CHECK].ToString());
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

        private static bool EventOccured(XmlDocument state, string id, StepStatuses status)
            => Utility.StepAchievedStatus(state, id, status);


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

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));

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
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_0fbfgne", "SequenceFlow_1sl9l6m","SequenceFlow_0hhf11n" }}
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsEventStartValid=new IsEventStartValid(InstanceIsEventStartValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));

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

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsFalse(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsTrue(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));
        }

        [TestMethod]
        public void TestOverrideFlowConditionExpression()
        {
            IProcessInstance instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1" }},
                {_CONDITION_CHECK,"inlineCondition" }
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsFlowValid=new IsFlowValid(InstanceIsFlowValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));

            instance = _pathChecksProcess.BeginProcess(new Dictionary<string, object>()
            {
                {_VALID_PATHS_NAME,new string[]{ "StartEvent_1" }},
                {_CONDITION_CHECK,"elementCondition" }
            },
            validations: new BPMNEngine.DelegateContainers.StepValidations()
            {
                IsFlowValid=new IsFlowValid(InstanceIsFlowValid)
            });
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsFalse(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsTrue(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));
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

            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Started));
            Assert.IsTrue(EventOccured(doc, "StartEvent_1", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Started));
            Assert.IsFalse(EventOccured(doc, "StartEvent_0fbfgne", StepStatuses.Succeeded));

            Assert.IsTrue(EventOccured(doc, "SequenceFlow_1sl9l6m", StepStatuses.Succeeded));
            Assert.IsFalse(EventOccured(doc, "SequenceFlow_0ijuqxx", StepStatuses.Succeeded));

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
