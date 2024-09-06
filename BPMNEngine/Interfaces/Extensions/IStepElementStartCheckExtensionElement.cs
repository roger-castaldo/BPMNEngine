using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.Interfaces.Extensions
{
    /// <summary>
    /// This interface is implemented to provide checks to ensure that a particular element should start from the element extensions.
    /// It can be contained within a Process, SubProcess, StartEvent or Flows
    /// </summary>
    public interface IStepElementStartCheckExtensionElement
    {
        /// <summary>
        /// Called to check and see if this element should start
        /// </summary>
        /// <param name="variables">The readonly current variable state of the process instance</param>
        /// <param name="owningElement">The element that this particular extension element was located within</param>
        /// <returns>True if the element can start</returns>
        ValueTask<bool> IsElementStartValid(IReadonlyVariables variables, IElement owningElement);
    }
}
