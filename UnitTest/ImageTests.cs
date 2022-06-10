using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTest
{
    [TestClass]
    public class ImageTests
    {

        [TestMethod]
        public void TestPngImageGeneration()
        {
            BusinessProcess bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/all_icons.bpmn"));
            Exception ex = null;
            byte[] data = null;
            try
            {
                data = bp.Diagram(ImageOuputTypes.Png);
            }catch(Exception e)
            {
                ex=e;
                data=null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
        }

        [TestMethod]
        public void TestJpegImageGeneration()
        {
            BusinessProcess bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/all_icons.bpmn"));
            Exception ex = null;
            byte[] data = null;
            try
            {
                data = bp.Diagram(ImageOuputTypes.Jpeg);
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
            BusinessProcess bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/vacation_request.bpmn"));
            IProcessInstance instance = bp.LoadState(Utility.LoadResourceDocument("ImageTests/vacation_state.xml"));
            Assert.IsNotNull(instance);
            Exception ex = null;
            byte[] data = null;
            byte[] nonState = null;
            try
            {
                data = instance.Diagram(true, ImageOuputTypes.Png);
                nonState = bp.Diagram(ImageOuputTypes.Png);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
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
            BusinessProcess bp = new BusinessProcess(Utility.LoadResourceDocument("ImageTests/vacation_request.bpmn"));
            IProcessInstance instance = bp.LoadState(Utility.LoadResourceDocument("ImageTests/vacation_state.xml"));
            Assert.IsNotNull(instance);
            Exception ex = null;
            byte[] data = null;
            byte[] nonState = null;
            try
            {
                data = instance.Animate(true);
                nonState = instance.Animate(false);
            }
            catch (Exception e)
            {
                ex=e;
                data=null;
            }
            Assert.IsNull(ex);
            Assert.IsNotNull(data);
            Assert.IsNotNull(nonState);
            Assert.IsTrue(data.Length > 0);
            Assert.AreNotEqual(Convert.ToBase64String(data), Convert.ToBase64String(nonState));
        }
    }
}
