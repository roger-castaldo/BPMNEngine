using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Reflection;
using System.Xml;
using BPMNEngine.Interfaces.Elements;
using System.Text;
using System.Linq;

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
            var err = Assert.ThrowsException<InvalidProcessDefinitionException>(() => new BusinessProcess(Utility.LoadResourceDocument("DiagramLoading/invalid.bpmn")));

            var res = String.Join('\n', err.ProcessExceptions.Select(e => e.Message));

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
            Assert.IsTrue(res.Contains("Intermediate Throw Events must have only 1 incoming path."));
            Assert.IsTrue(res.Contains("A throw event can only have one signal to be thrown."));
            Assert.IsTrue(res.Contains("A throw event can only have one message to be thrown."));
            Assert.IsTrue(res.Contains("A throw event can only have one error to be thrown."));
            Assert.IsTrue(res.Contains("A throw event cannot signal with a wildcard signal."));
            Assert.IsTrue(res.Contains("A throw event cannot error with a wildcard error."));
            Assert.IsTrue(res.Contains("A throw event cannot message with a wildcard message."));
            Assert.IsTrue(res.Contains("A throw must have a signal to throw."));
            Assert.IsTrue(res.Contains("A throw must have a error to throw."));
            Assert.IsTrue(res.Contains("A throw must have a message to throw."));
            Assert.IsTrue(res.Contains("No child elements to render."));
            Assert.IsTrue(res.Contains("No child elements found in Process."));
            Assert.IsTrue(res.Contains("No content for the text annotation was specified."));
            Assert.IsTrue(res.Contains("Not enough child elements found for an And Condition"));
            Assert.IsTrue(res.Contains("No child elements found within a condition set."));
            Assert.IsTrue(res.Contains("Not enough child elements found for an Or Condition"));
            Assert.IsTrue(res.Contains("No bounds specified for the shape."));
            Assert.IsTrue(res.Contains("End Events cannot have an outgoing path."));
            Assert.IsTrue(res.Contains("End Events must have an incoming path."));
            Assert.IsTrue(res.Contains("No child elements found in the definition."));
            Assert.IsTrue(res.Contains("Collaboration requires at least 1 child element."));
            Assert.IsTrue(res.Contains("Right value specified more than once."));
            Assert.IsTrue(res.Contains("Left value specified more than once."));
            Assert.IsTrue(res.Contains("Right and Left value missing."));
            Assert.IsTrue(res.Contains("Left value missing."));
            Assert.IsTrue(res.Contains("A Sub Process Must have a StartEvent or valid IntermediateCatchEvent"));
            Assert.IsTrue(res.Contains("A Sub Process Must have a valid Incoming path, achieved through an incoming flow or IntermediateCatchEvent"));
            Assert.IsTrue(res.Contains("A Sub Process Must have an EndEvent"));
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