using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Helpers
{
    internal static class LoggingHelper
    {

        public static (ILoggerFactory loggerFactory,Mock<ILogger> mockLogger) CreateMockLogFactory()
        {
            // Create a mock ILogger
            var mockLogger = new Mock<ILogger>();
            mockLogger.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            var mockLogFactory = new Mock<ILoggerFactory>();

            // Setup the factory to return the mocked logger
            mockLogFactory
                .Setup(factory => factory.CreateLogger(It.IsAny<string>()))
                .Returns(mockLogger.Object);

            return (mockLogFactory.Object,mockLogger);

        }
    }
}
