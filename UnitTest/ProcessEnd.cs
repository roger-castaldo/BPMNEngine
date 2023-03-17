﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class ProcessEnd
    {
        [TestMethod()]
        public void TestBasicProcessEnd()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("DiagramLoading/start_to_stop.bpmn"));
            IProcessInstance instance = proc.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "EndEvent_1d1a99g"));
            proc.Dispose();
        }

        [TestMethod()]
        public void TestSubProcessEnd()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("ProcessEnd/sub_process_end.bpmn"));
            IProcessInstance instance = proc.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "EndEvent_1oqe74x"));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "EndEvent_0c7kvxm"));
            proc.Dispose();
        }

        [TestMethod()]
        public void TestProcessTerminateEnd()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("ProcessEnd/sub_process_terminate.bpmn"));
            IProcessInstance instance = proc.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "EndEvent_0i74eau"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "EndEvent_0c7kvxm"));
            proc.Dispose();
        }

        [TestMethod()]
        public void TestProcessTerminateEndWithAborts()
        {
            BusinessProcess proc = new BusinessProcess(Utility.LoadResourceDocument("ProcessEnd/sub_process_terminate_abort.bpmn"));
            IProcessInstance instance = proc.BeginProcess(null);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(Constants.DEFAULT_PROCESS_WAIT));
            Assert.IsTrue(Utility.StepCompleted(instance.CurrentState, "EndEvent_0i74eau"));
            Assert.IsTrue(Utility.StepAborted(instance.CurrentState, "UserTask_0l8i663"));
            Assert.IsTrue(Utility.StepAborted(instance.CurrentState, "IntermediateCatchEvent_0gjhltt"));
            Assert.IsTrue(Utility.StepAborted(instance.CurrentState, "BoundaryEvent_05sy9qo"));
            Assert.IsFalse(Utility.StepCompleted(instance.CurrentState, "EndEvent_0c7kvxm"));
            proc.Dispose();
        }
    }
}
