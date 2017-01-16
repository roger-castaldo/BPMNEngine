using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
    public class InvalidProcessDefinitionException : Exception
    {
        private Exception[] _processExceptions;
        public Exception[] ProcessExceptions { get { return _processExceptions; } }

        internal InvalidProcessDefinitionException(List<Exception> children)
            : base("The process failed validation.  The property ProcessExceptions contains the list of details.")
        {
            _processExceptions = children.ToArray();
        }
    }

    public class MissingAttributeException : Exception
    {
        internal MissingAttributeException(XmlNode n, RequiredAttribute att)
            : base(string.Format("The element at {0} is missing a value for the attribute {1}", Utility.FindXPath(n), att.Name)) { }

    }

    public class InvalidAttributeValueException : Exception
    {
        internal InvalidAttributeValueException(XmlNode n, AttributeRegex ar)
            : base(string.Format("The element at {0} has an invalid value for the attribute {1}, expected {2}", new string[]{
                Utility.FindXPath(n), 
                ar.Name,ar.Reg.ToString()
            })) { }
    }

    public class InvalidElementException : Exception
    {
        internal InvalidElementException(XmlNode n, string[] err)
            : base(string.Format("The element at {0} has the following error(s):\n{1}", Utility.FindXPath(n), String.Join("\n\t",err))) { }
    }

    public class NotSuspendedException : Exception
    {
        internal NotSuspendedException()
            : base("Unable to resume a process that is not suspended.") { }
    }
}
