using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPMNEngine;
using BPMNEngine.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class ImageTests
    {
        [TestInitialize] public void Init() {
            System.Diagnostics.Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        private static void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }

        private static void Trace(string message, object[] pars)
        {
            Trace(String.Format(message, pars));
        }

        [TestMethod]
        public void TestPngImageGeneration()
        {
            var bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/all_icons.bpmn"));
            Exception ex = null;
            byte[] data;
            try
            {
                data = bp.Diagram(Microsoft.Maui.Graphics.ImageFormat.Png);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
            string tmpFile = Path.GetTempFileName();
            Trace("Writing png with all icons to {0}",[tmpFile]);
            var bw = new BinaryWriter(new FileStream(tmpFile, FileMode.Create, FileAccess.Write, FileShare.None));
            bw.Write(data);
            bw.Flush();
            bw.Close();
        }

        [TestMethod]
        public void TestJpegImageGeneration()
        {
            var bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/all_icons.bpmn"));
            Exception ex = null;
            byte[] data;
            try
            {
                data = bp.Diagram(Microsoft.Maui.Graphics.ImageFormat.Jpeg);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
        }


        [TestMethod]
        public void TestVariableOutput()
        {
            var bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/vacation_request.bpmn"));
            IProcessInstance instance = bp.LoadState(Utility.LoadResourceDocument("ImageTests/vacation_state.xml"));
            Assert.IsNotNull(instance);
            Exception ex = null;
            byte[] data;
            byte[] nonState;
            try
            {
                data = instance.Diagram(true, Microsoft.Maui.Graphics.ImageFormat.Png);
                nonState = bp.Diagram(Microsoft.Maui.Graphics.ImageFormat.Png);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
                nonState=null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsNotNull(nonState);
            Assert.IsTrue(data.Length > 0);
            Assert.AreNotEqual(Convert.ToBase64String(data), Convert.ToBase64String(nonState));
        }

        [TestMethod()]
        public void TestAnimateProcess()
        {
            var bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/vacation_request.bpmn"));
            IProcessInstance instance = bp.LoadState(Utility.LoadResourceDocument("ImageTests/vacation_state.xml"));
            Assert.IsNotNull(instance);
            Exception ex = null;
            byte[] data;
            byte[] nonState;
            try
            {
                data = instance.Animate(true);
                nonState = instance.Animate(false);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
                nonState = null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsNotNull(nonState);
            Assert.IsTrue(data.Length > 0);
            Assert.AreNotEqual(Convert.ToBase64String(data), Convert.ToBase64String(nonState));
        }
    }
}
