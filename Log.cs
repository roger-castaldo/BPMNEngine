using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
     /// <summary>
    /// This implements a logging class to make log calls to, these logs can be stored in multiple locations depending on the implementations supplied.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="args">The arguments to be fed into a string format call agains the message</param>
        public static void Debug(string message,object[] args)
        {
            Log.Debug(string.Format(message, args));
        }

        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string message</param>
        public static void Debug(string message)
        {
            _LogLine(LogLevels.Debug, message);
        }

        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="args">The arguments to be fed into a string format call agains the message</param>
        public static void Info(string message,object[] args)
        {
            Info(string.Format(message, args));
        }

        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string message</param>
        public static void Info(string message)
        {
            _LogLine(LogLevels.Info, message);
        }

        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="args">The arguments to be fed into a string format call agains the message</param>
        public static void Error(string message,object[] args)
        {
            Error(string.Format(message, args));
        }

        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string message</param>
        public static void Error(string message)
        {
            _LogLine(LogLevels.Error, message);
        }

        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="args">The arguments to be fed into a string format call agains the message</param>
        public static void Fatal(string message,object[] args)
        {
            Fatal(string.Format(message, args));
        }

        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string message</param>
        public static void Fatal(string message)
        {
            _LogLine(LogLevels.Fatal, message);
        }

        private static void _LogLine(LogLevels level, string message)
        {
            if (BusinessProcess.Current!=null)
                BusinessProcess.Current.WriteLogLine(level, new StackFrame(3, true), DateTime.Now, message);
        }

        internal static Exception _Exception(Exception exception)
        {
            if (BusinessProcess.Current != null)
                BusinessProcess.Current.WriteLogException(new StackFrame(2, true), DateTime.Now, exception);
            return exception;
        }

        /// <summary>
        /// Called to log an exception 
        /// </summary>
        /// <param name="exception">The Exception that occured</param>
        public static void Exception(Exception exception)
        {
            if (BusinessProcess.Current != null)
                BusinessProcess.Current.WriteLogException(new StackFrame(2, true), DateTime.Now, exception);
        }
    }
}
