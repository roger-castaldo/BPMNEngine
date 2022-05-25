using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class DiagramLoading
    {
        [TestMethod]
        public void LoadInvalidDiagram()
        {
            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/invalid.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new BusinessProcess(doc);
                loaded=true;
            }catch(Exception e)
            {
            }
            Assert.IsFalse(loaded);
        }

        [TestMethod]
        public void LoadStartToEndDiagram()
        {
            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/start_to_stop.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new BusinessProcess(doc);
                loaded=true;
            }
            catch (Exception e)
            {
            }
            Assert.IsTrue(loaded);
        }

        [TestMethod]
        public void RunStartToEndDiagram()
        {
            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/start_to_stop.bpmn");
            bool loaded = false;
            BusinessProcess proc = null;
            try
            {
                proc = new BusinessProcess(doc);
                loaded=true;
            }
            catch (Exception e)
            {
            }
            Assert.IsTrue(loaded);
            if (loaded)
            {
                IProcessInstance inst = proc.BeginProcess(null);
                Assert.IsNotNull(inst);
                Assert.IsTrue(inst.WaitForCompletion());
                Assert.AreNotEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", inst.CurrentState.OuterXml);
            }
        }
    }
}