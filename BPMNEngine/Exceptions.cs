﻿using BPMNEngine.Attributes;
using BPMNEngine.Elements;
using BPMNEngine.Elements.Processes.Gateways;
using System.Diagnostics.CodeAnalysis;

namespace BPMNEngine
{
    internal class ExceptionComparer : EqualityComparer<Exception>
    {
        public override bool Equals(Exception x, Exception y)
            =>x.Message.Equals(y.Message, StringComparison.InvariantCultureIgnoreCase);

        public override int GetHashCode([DisallowNull] Exception obj)
            =>obj.Message.ToLower().GetHashCode();
    }

    /// <summary>
    /// This Exception gets thrown on the loading of a Process Definition inside a BusinessProcess class when the definition is found to be invalid.
    /// </summary>
    public class InvalidProcessDefinitionException : Exception
    {
        /// <summary>
        /// The Exception(s) thrown during the validation process.
        /// </summary>
        public IEnumerable<Exception> ProcessExceptions { get; private init; }

        internal InvalidProcessDefinitionException(IEnumerable<Exception> children)
            : base("The process failed validation.  The property ProcessExceptions contains the list of details.")
        {
            ProcessExceptions = children.Distinct(new ExceptionComparer()).ToArray();
        }
    }

    /// <summary>
    /// This Exception is thrown when a required attribute is missing from an Element found within the definition
    /// </summary>
    public class MissingAttributeException : Exception
    {
        internal MissingAttributeException(Definition definition, XmlNode n, RequiredAttributeAttribute att)
            : base($"The element at {Utility.FindXPath(definition, n)} is missing a value for the attribute {att.Name}") { }

    }

    /// <summary>
    /// This Exception is thrown when an attribute value is not valid on an Element found within the definition
    /// </summary>
    public class InvalidAttributeValueException : Exception
    {
        internal InvalidAttributeValueException(Definition definition, XmlNode n, AttributeRegexAttribute ar)
            : base($"The element at {Utility.FindXPath(definition, n)} has an invalid value for the attribute {ar.Name}, expected {ar.Reg}") { }
    }

    /// <summary>
    /// This Exception is thrown when an Element found within the definition is not valid
    /// </summary>
    public class InvalidElementException : Exception
    {
        internal InvalidElementException(Definition definition, XmlNode n, IEnumerable<string> err)
            : base($"The element at {Utility.FindXPath(definition, n)} has the following error(s):\n{String.Join("\n\t", err.Distinct())}") { }
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
        internal MultipleOutgoingPathsException(ASinglePathGateway gateway)
            : base($"The {(gateway is ExclusiveGateway ? "Exclusive" : "Event")} Gateway {gateway.ID} has evaluated the outgoing paths and determine more than 1 result") { }
    }

    /// <summary>
    /// This Exception is thrown when an error occurs generating a Process Diagram Image
    /// </summary>
    public class DiagramException : Exception{
        internal DiagramException(string message)
            : base(message) { }
    }

    /// <summary>
    /// This Exception is thrown when a Process Instance is being disposed but still has active steps
    /// </summary>
    public class ActiveStepsException : Exception
    {
        internal ActiveStepsException()
            : base("An attempt was made to dispose of a Process Instance with active steps") { }
    }

    /// <summary>
    /// This Exception is thrown by an Intermediate Throw Event when an Error Message is defined
    /// </summary>
    public class IntermediateProcessExcepion : Exception
    {
        /// <summary>
        /// Houses the error message string defined inside the ErrorMessage definition from within the process document
        /// </summary>
        public string ProcessMessage { get; private init; }
        internal IntermediateProcessExcepion(string message)
            : base($"An Intermediate Event threw the following error: {message}") {
            ProcessMessage=message;
        }
    }

    internal class StateLockTimeoutException : Exception
    {
        public StateLockTimeoutException(Guid? stateID, int currentReadCount,int waitingWriteCount)
            : base($"Locking of the state {stateID} for reading/writing has timed out. [CurrentReadCount: {currentReadCount},WaitingWriteCount: {waitingWriteCount}]") { }
    }

    /// <summary>
    /// Thrown when attempting to use Javascript without the Jint Assembly
    /// </summary>
    public class JintAssemblyMissingException() 
        : Exception("Unable to process Javascript because the Jint.dll was not located in the Assembly path.")
    {}
}
