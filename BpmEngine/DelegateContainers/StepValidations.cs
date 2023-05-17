using BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpmEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the validation delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public class StepValidations
    {
        private static readonly StepValidations _DEFAULT_VALIDATOR = new StepValidations()
        {
            IsEventStartValid=new IsEventStartValid(_DefaultEventStartValid),
            IsProcessStartValid= new IsProcessStartValid(_DefaultProcessStartValid),
            IsFlowValid= new IsFlowValid(_DefaultFlowValid)
        };

        /// <summary>
        /// A delegate called to validate if an event can start
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:startEvent id="StartEvent_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public bool IsEventStartValid(IStepElement Event, IVariables variables){
        ///     if (Event.ExtensionElement != null){
        ///         foreach (XmlNode xn in Event.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="DateRange"){
        ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     return true;
        /// }
        /// </code>
        /// </summary>
        public IsEventStartValid IsEventStartValid { get; init; }
        /// <summary>
        /// A delegate called to validate if a process is valid to start
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:process id="Process_1" isExecutable="false">
        ///  ...
        ///  <bpmn:extensionElements>
        ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
        ///  </bpmn:extensionElements>
        /// </bpmn:process>
        /// ]]>
        /// 
        /// public bool IsProcessStartValid(IElement process, IVariables variables){
        ///     if (process.ExtensionElement != null){
        ///         foreach (XmlNode xn in process.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="DateRange"){
        ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     return true;
        /// }
        /// </code>
        /// </summary>
        public IsProcessStartValid IsProcessStartValid { get; init; }
        /// <summary>
        /// A delegate called to validate if a flow is valid to run
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:outgoing>SequenceFlow_1jma3bu
        ///  <bpmn:extensionElements>
        ///    <DateRange start="2020-01-01 00:00:00" end="2020-12-31 11:59:59"/>
        ///  </bpmn:extensionElements>
        /// </bpmn:outgoing>
        /// ]]>
        /// 
        /// public bool IsFlowValid(IElement flow, IVariables variables){
        ///     if (flow.ExtensionElement != null){
        ///         foreach (XmlNode xn in flow.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="DateRange"){
        ///                     return DateTime.Now.Ticks &gt;= DateTime.Parse(xn.Attributes["start"].Value) &amp;&amp; DateTime.Now.Ticks &lt;= DateTime.Parse(xn.Attributes["end"].Value);
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     return true;
        /// }
        /// </code>
        /// </summary>
        public IsFlowValid IsFlowValid {get; init; }

        private static bool _DefaultEventStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private static bool _DefaultProcessStartValid(IElement Event, IReadonlyVariables variables) { return true; }
        private static bool _DefaultFlowValid(IElement flow, IReadonlyVariables variables) { return true; }

        internal static StepValidations Merge(StepValidations source, StepValidations append)
        {
            if (source==null&&append==null) return Merge(_DEFAULT_VALIDATOR, null);
            if (source==null) return Merge(_DEFAULT_VALIDATOR,append);
            if (append==null) return Merge(_DEFAULT_VALIDATOR,source);
            return new StepValidations()
            {
                IsEventStartValid = append.IsEventStartValid??(source.IsEventStartValid??new IsEventStartValid(_DefaultEventStartValid)),
                IsProcessStartValid = append.IsProcessStartValid??(source.IsProcessStartValid??new IsProcessStartValid(_DefaultProcessStartValid)),
                IsFlowValid = append.IsFlowValid??(source.IsFlowValid ?? new IsFlowValid(_DefaultFlowValid))
            };
        }
    }
}
