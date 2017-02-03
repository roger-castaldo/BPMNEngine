using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    public static class Log
    {
        public static void Debug(string message,object[] args)
        {
            Log.Debug(string.Format(message, args));
        }

        public static void Debug(string message)
        {
            _LogLine(LogLevels.Debug, message);
        }

        public static void Info(string message,object[] args)
        {
            Info(string.Format(message, args));
        }

        public static void Info(string message)
        {
            _LogLine(LogLevels.Info, message);
        }

        public static void Error(string message,object[] args)
        {
            Error(string.Format(message, args));
        }

        public static void Error(string message)
        {
            _LogLine(LogLevels.Error, message);
        }

        public static void Fatal(string message,object[] args)
        {
            Fatal(string.Format(message, args));
        }

        public static void Fatal(string message)
        {
            _LogLine(LogLevels.Fatal, message);
        }

        private static void _LogLine(LogLevels level, string message)
        {
            BusinessProcess.Current.WriteLogLine(level, new StackFrame(3, true), DateTime.Now, message);
        }

        internal static Exception _Exception(Exception exception)
        {
            BusinessProcess.Current.WriteLogException(new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }

        public static void Exception(Exception exception)
        {
            BusinessProcess.Current.WriteLogException(new StackFrame(2, true), DateTime.Now, exception);
        }
    }
}
