using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Interfaces
{
    /// <summary>
    /// This interface is the parent interface for ALL process elements (which are XML nodes)
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// The unique ID of the element from the process
        /// </summary>
        string id { get; }

        /// <summary>
        /// The child XMLNodes from the process element
        /// </summary>
        XmlNode[] SubNodes { get; }

        /// <summary>
        /// This is called to get an attribute value from the process element
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns></returns>
        string this[string attributeName]{ get; }

        /// <summary>
        /// The extensions element if it exists.  This element is what is used in BPMN 2.0 to house additional components outside of the definition that 
        /// woudl allow you to extend the definition beyond the business process diagraming and into more of a realm for building it.  Such as Script and Condition 
        /// elements that this library implements.
        /// </summary>
        IElement ExtensionElement { get; }

        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Debug(string message);
        /// <summary>
        /// Called to log a debug level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Debug(string message,object[] pars);
        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Info(string message);
        /// <summary>
        /// Called to log an info level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Info(string message, object[] pars);
        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Error(string message);
        /// <summary>
        /// Called to log an error level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Error(string message, object[] pars);
        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string message</param>
        void Fatal(string message);
        /// <summary>
        /// Called to log a fatal level message
        /// </summary>
        /// <param name="message">The string formatted message</param>
        /// <param name="pars">The arguments to be fed into a string format call agains the message</param>
        void Fatal(string message, object[] pars);
        /// <summary>
        /// Called to log an exception 
        /// </summary>
        /// <param name="exception">The Exception that occured</param>
        /// <returns>The exception that was passed as an arguement to allow for throwing</returns>
        Exception Exception(Exception exception);
    }
}
