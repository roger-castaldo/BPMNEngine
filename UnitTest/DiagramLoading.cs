using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Reflection;
using System.Xml;
using BPMNEngine.Interfaces.Elements;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class DiagramLoading
    {
        [TestMethod]
        public void LoadWithNoDefinition()
        {
            var log = new StringBuilder();

            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/no_definition.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new(doc,
                    logging: new BPMNEngine.DelegateContainers.ProcessLogging()
                    {
                        LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                        {
                            log.AppendLine($"{callingElement?.ID}|{exception.Message}");
                        }
                    }
                );
                loaded=true;
            }
            catch (Exception)
            {
            }
            Assert.IsFalse(loaded);

            var res = log.ToString();

            Assert.IsTrue(res.Contains("Unable to load a bussiness process from the supplied document.  No instance of bpmn:definitions was located."));
        }

        [TestMethod]
        public void LoadEmptyDocument()
        {
            var log = new StringBuilder();

            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/no_elements.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new(doc,
                    logging: new BPMNEngine.DelegateContainers.ProcessLogging()
                    {
                        LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                        {
                            log.AppendLine($"{callingElement?.ID}|{exception.Message}");
                        }
                    }
                );
                loaded=true;
            }
            catch (Exception)
            {
            }
            Assert.IsFalse(loaded);

            var res = log.ToString();

            Assert.IsTrue(res.Contains("Unable to load a bussiness process from the supplied document.  No bpmn elements were located."));
        }

        [TestMethod]
        public void LoadInvalidDiagram()
        {
            var log = new StringBuilder();

            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/invalid.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new(doc,
                    logging: new BPMNEngine.DelegateContainers.ProcessLogging() {
                        LogException=(IElement callingElement, AssemblyName assembly, string fileName, int lineNumber, DateTime timestamp, Exception exception) =>
                        {
                            log.AppendLine($"{callingElement?.ID}|{exception.Message}");
                        }
                    }
                );
                loaded=true;
            }catch(Exception)
            {
            }
            Assert.IsFalse(loaded);

            var res = log.ToString();

            Assert.IsTrue(res.Contains("Start Events must have an outgoing path"));
            Assert.IsTrue(res.Contains("Intermediate Catch Events must have an outgoing path."));
            Assert.IsTrue(res.Contains("Intermediate Catch Events must have only 1 outgoing path."));
            Assert.IsTrue(res.Contains("No Date String Specified"));
            Assert.IsTrue(res.Contains("dc:Bounds[0] is missing a value for the attribute x"));
            Assert.IsTrue(res.Contains("dc:Bounds[0] has an invalid value for the attribute y, expected ^-?\\d+(\\.\\d+)?$"));
            Assert.IsTrue(res.Contains("Boundary Events must have an outgoing path."));
            Assert.IsTrue(res.Contains("Boundary Events cannot have an incoming path."));
            Assert.IsTrue(res.Contains("Boundary Events can only have one outgoing path."));
            Assert.IsTrue(res.Contains("At least 2 points are required."));
            Assert.IsTrue(res.Contains("[@id='BPMNDiagram_2'] has the following error(s):\nNo child elements found."));
            Assert.IsTrue(res.Contains("A ExclusiveGateway must have at least 1 incoming path."));
            Assert.IsTrue(res.Contains("A ExclusiveGateway must have at least 1 outgoing path."));
            Assert.IsTrue(res.Contains("Too many children found."));
            Assert.IsTrue(res.Contains("An Error Definition cannot have the type of *, this is reserved"));
            Assert.IsTrue(res.Contains("An Error Definition for a Throw Event must have a Type defined"));
            Assert.IsTrue(res.Contains("A Message Definition cannot have the name of *, this is reserved"));
            Assert.IsTrue(res.Contains("A Message Definition for a Throw Event must have a Name defined"));
            Assert.IsTrue(res.Contains("A Signal Definition cannot have the type of *, this is reserved"));
            Assert.IsTrue(res.Contains("A Signal Definition for a Throw Event must have a Type defined"));
        }

        [TestMethod]
        public void LoadStartToEndDiagram()
        {
            XmlDocument doc = Utility.LoadResourceDocument("DiagramLoading/start_to_stop.bpmn");
            bool loaded = false;
            try
            {
                BusinessProcess proc = new(doc);
                loaded=true;
            }
            catch (Exception)
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
            catch (Exception){}
            Assert.IsTrue(loaded);
            if (loaded)
            {
                IProcessInstance inst = proc.BeginProcess(null);
                Assert.IsNotNull(inst);
                Assert.IsTrue(Utility.WaitForCompletion(inst));
                Assert.AreNotEqual("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", inst.CurrentState.AsXMLDocument);
            }
        }
    }
}