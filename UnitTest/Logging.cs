﻿using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace UnitTest
{
    [TestClass]
    public class Logging
    {
        private const string _LOG_LINE = "Test Log Line";
        private const string _LOG_FORMAT_LINE = "Test Log Line {0}";
        private static readonly object[] _FORMAT_INPUT = [1234567890];
        private static readonly Exception _EXCEPTION = new(_LOG_LINE);

        [TestMethod]
        public void TestLoggingFromUserTask()
        {
            var logger = new Mock<LogLine>();
            var exceptionLogger = new Mock<LogException>();
            var process = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"),
                logging: new BPMNEngine.DelegateContainers.ProcessLogging()
                {
                    LogLine=logger.Object,
                    LogException=exceptionLogger.Object
                },
                tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
                {
                    BeginUserTask=new StartUserTask(StartUserTask)
                }
            );

            Assert.IsNotNull(process);

            IProcessInstance instance = process.BeginProcess(new Dictionary<string, object>() { }, stateLogLevel: LogLevel.Debug);
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Information, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Information, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Debug, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Debug, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Error, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Error, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Critical, It.IsAny<DateTime>(), _LOG_LINE), Times.Once);
            logger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), LogLevel.Critical, It.IsAny<DateTime>(), string.Format(_LOG_LINE, _FORMAT_INPUT)), Times.Once);

            exceptionLogger.Verify(l => l.Invoke(It.IsAny<IElement>(), It.IsAny<AssemblyName>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<DateTime>(), _EXCEPTION), Times.Once);

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            var nodes = doc.GetElementsByTagName("ProcessLog");

            Assert.AreEqual(1, nodes.Count);

            var logs = nodes[0].InnerText.Trim();

            Assert.IsTrue(logs.Contains("|Information|"));
            Assert.IsTrue(logs.Contains("|Debug|"));
            Assert.IsTrue(logs.Contains("|Error|"));
            Assert.IsTrue(logs.Contains("|Critical|"));
            Assert.IsTrue(logs.Contains("STACKTRACE:"));
        }

        private void StartUserTask(IUserTask task)
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
