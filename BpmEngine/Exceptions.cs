using Org.Reddragonit.BpmEngine.Attributes;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.Elements.Processes.Gateways;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    /// <summary>
    /// This Exception gets thrown on the loading of a Process Definition inside a BusinessProcess class when the definition is found to be invalid.
    /// </summary>
    public class InvalidProcessDefinitionException : Exception
    {
        private Exception[] _processExceptions;
        /// <summary>
        /// The Exception(s) thrown during the validation process.
        /// </summary>
        public Exception[] ProcessExceptions { get { return _processExceptions; } }

        internal InvalidProcessDefinitionException(List<Exception> children)
            : base("The process failed validation.  The property ProcessExceptions contains the list of details.")
        {
            _processExceptions = children.ToArray();
        }
    }

    /// <summary>
    /// This Exception is thrown when a required attribute is missing from an Element found within the definition
    /// </summary>
    public class MissingAttributeException : Exception
    {
        internal MissingAttributeException(Definition definition, XmlNode n, RequiredAttribute att)
            : base(string.Format("The element at {0} is missing a value for the attribute {1}", Utility.FindXPath(definition, n), att.Name)) { }

    }

    /// <summary>
    /// This Exception is thrown when an attribute value is not valid on an Element found within the definition
    /// </summary>
    public class InvalidAttributeValueException : Exception
    {
        internal InvalidAttributeValueException(Definition definition, XmlNode n, AttributeRegex ar)
            : base(string.Format("The element at {0} has an invalid value for the attribute {1}, expected {2}", new string[]{
                Utility.FindXPath(definition,n),
                ar.Name,ar.Reg.ToString()
            })) { }
    }

    /// <summary>
    /// This Exception is thrown when an Element found within the definition is not valid
    /// </summary>
    public class InvalidElementException : Exception
    {
        internal InvalidElementException(Definition definition, XmlNode n, string[] err)
            : base(string.Format("The element at {0} has the following error(s):\n{1}", Utility.FindXPath(definition, n), String.Join("\n\t", err))) { }
    }

    /// <summary>
    /// This Exception is thrown when a Business Process is told to Resume but is not Suspended
    /// </summary>
    public class NotSuspendedException : Exception
    {
        internal NotSuspendedException()
            : base("Unable to resume a process that is not suspended.") { }
    }

    /// <summary>
    /// This Exception is thrown when an Exclusive Gateway evalutes to more than 1 outgoing path
    /// </summary>
    public class MultipleOutgoingPathsException : Exception
    {
        internal MultipleOutgoingPathsException(ExclusiveGateway gateway)
            : base(string.Format("The Exclusive Gateway {0} has evaluated the outgoing paths and determine more than 1 result", new object[] { gateway.id })) { }
    }

    /// <summary>
    /// This Exception is thrown when an error occurs generating a Process Diagram Image
    /// </summary>
    public class DiagramException : Exception{
        internal DiagramException(string message)
            : base(message) { }
    }
}
