using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine
{
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
        internal InvalidElementException(XmlNode n, string err)
            : base(string.Format("The element at {0} has an error. {1}", Utility.FindXPath(n), err)) { }
    }
}
