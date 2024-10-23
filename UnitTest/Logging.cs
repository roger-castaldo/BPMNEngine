using BPMNEngine;
using BPMNEngine.Interfaces;
using BPMNEngine.Interfaces.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Xml;
using UnitTest.Helpers;

namespace UnitTest
{
    [TestClass]
    public class Logging
    {
        private const string _LOG_LINE = "Test Log Line";
        private const string _LOG_FORMAT_LINE = "Test Log Line {Number}";
        private static readonly object _FORMAT_INPUT = 1234567890;
        private static readonly Exception _EXCEPTION = new(_LOG_LINE);

        [TestMethod]
        public async System.Threading.Tasks.Task TestLoggingFromUserTask()
        {
            (var loggerFactory, var mockLogger) = LoggingHelper.CreateMockLogFactory();
            var process = new BusinessProcess(Utility.LoadResourceDocument("UserTasks/single_user_task.bpmn"),
                loggerFactory: loggerFactory,
                tasks: new BPMNEngine.DelegateContainers.ProcessTasks()
                {
                    BeginUserTask=new StartUserTask(StartUserTask)
                }
            );

            Assert.IsNotNull(process);

            IProcessInstance instance = await process.BeginProcessAsync(new Dictionary<string, object>() { }, stateLogLevel: LogLevel.Debug);
            Assert.IsNotNull(instance);
            Assert.IsTrue(Utility.WaitForCompletion(instance));

            mockLogger.VerifyLog(l => l.LogInformation(_LOG_LINE), Times.Once());
            mockLogger.VerifyLog(l => l.LogInformation(_LOG_FORMAT_LINE,_FORMAT_INPUT), Times.Once());

            mockLogger.VerifyLog(l => l.LogDebug(_LOG_LINE), Times.Once());
            mockLogger.VerifyLog(l => l.LogDebug(_LOG_FORMAT_LINE, _FORMAT_INPUT), Times.Once());

            mockLogger.VerifyLog(l => l.LogError(_LOG_LINE), Times.Once());
            mockLogger.VerifyLog(l => l.LogError(_LOG_FORMAT_LINE, _FORMAT_INPUT), Times.Once());

            mockLogger.VerifyLog(l => l.LogCritical(_LOG_LINE), Times.Once());
            mockLogger.VerifyLog(l => l.LogCritical(_LOG_FORMAT_LINE, _FORMAT_INPUT), Times.Once());

            mockLogger.VerifyLog(l => l.LogError(_EXCEPTION, "Error occured"), Times.Once());

            XmlDocument doc = new();
            doc.LoadXml(instance.CurrentState.AsXMLDocument);

            var nodes = doc.GetElementsByTagName("ProcessLog");

            Assert.AreEqual(1, nodes.Count);

            var logs = nodes[0].InnerText.Trim();

            Assert.IsTrue(logs.Contains("|Information|"));
            Assert.IsTrue(logs.Contains("|Debug|"));
            Assert.IsTrue(logs.Contains("|Error|"));
            Assert.IsTrue(logs.Contains("|Critical|"));
            Assert.IsTrue(logs.Contains("Error occured"));
            Assert.IsTrue(logs.Contains("|ElementID["));
            Assert.IsFalse(logs.Contains("|ProcessInstance["));
        }

        private void StartUserTask(IUserTask task)
        {
            task.Logger.LogInformation(_LOG_LINE);
            task.Logger.LogInformation(_LOG_FORMAT_LINE,_FORMAT_INPUT);
            task.Logger.LogDebug(_LOG_LINE);
            task.Logger.LogDebug(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Logger.LogError(_LOG_LINE);
            task.Logger.LogError(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Logger.LogCritical(_LOG_LINE);
            task.Logger.LogCritical(_LOG_FORMAT_LINE, _FORMAT_INPUT);
            task.Logger.LogError(_EXCEPTION,"Error occured");
            task.MarkComplete();
        }
    }
}
