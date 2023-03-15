﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Org.Reddragonit.BpmEngine;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    [TestClass]
    public class Logging
    {
        private const string _LOG_LINE = "Test Log Line";
        private const string _LOG_FORMAT_LINE = "Test Log Line {0}";
        private static readonly object[] _FORMAT_INPUT = new object[] { 1234567890 };
        private static readonly Exception _EXCEPTION = new Exception(_LOG_LINE);

        [TestMethod]
        public void TestLoggingFromUserTask()
        {
            var logger = new Mock<LogLine>();
            var exceptionLogger = new Mock<LogException>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"),
                logging: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessLogging()
                {
                    LogLine=logger.Object,
                    LogException=exceptionLogger.Object
                },
                tasks: new Org.Reddragonit.BpmEngine.DelegateContainers.ProcessTasks()
                {
                    BeginUserTask=new StartUserTask(_StartUserTask)
                }
            );

            Assert.IsNotNull(process);

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { },stateLogLevel:LogLevels.Fatal);
            Assert.IsNotNull(instance);
            Assert.IsTrue(instance.WaitForCompletion(30*1000));

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Info, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Info, It.IsAny<DateTime>(), string.Format(_LOG_LINE,_FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Debug, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Debug, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Error, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Debug, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Fatal, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevels.Debug, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            exceptionLogger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), _EXCEPTION), Times.Once);

            var nodes = instance.CurrentState.GetElementsByTagName("ProcessLog");

            Assert.AreEqual(1, nodes.Count);

            Assert.IsTrue(nodes[0].InnerText.Split("\r\n").Length>2);
        }

        private void _StartUserTask(IUserTask task)
        {
            task.Info(_LOG_LINE);
            task.Info(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Debug(_LOG_LINE);
            task.Debug(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Error(_LOG_LINE);
            task.Error(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Fatal(_LOG_LINE);
            task.Fatal(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Exception(_EXCEPTION);
            task.MarkComplete();
        }
    }
}