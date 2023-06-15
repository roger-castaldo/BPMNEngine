using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace BPMNEngine.Interfaces
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
        IEnumerable<XmlNode> SubNodes { get; }

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
    }
}
