using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;

namespace BPMNEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the validation delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public class StepValidations
    {
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
        public IsEventStartValid IsEventStartValid { get; init; } = null;
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
        public IsProcessStartValid IsProcessStartValid { get; init; } = null;
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
        public IsFlowValid IsFlowValid { get; init; } = null;

        private static readonly Func<IElement, IReadonlyVariables,bool> DEFAULT_ACTION = new((elem, variables) => true);
        internal static StepValidations Merge(StepValidations source, StepValidations append)
        {
            source??=new();
            append??=new();
            return new StepValidations()
            {
                IsEventStartValid = append.IsEventStartValid??source.IsEventStartValid??new(DEFAULT_ACTION),
                IsProcessStartValid = append.IsProcessStartValid??source.IsProcessStartValid??new(DEFAULT_ACTION),
                IsFlowValid = append.IsFlowValid??source.IsFlowValid??new(DEFAULT_ACTION)
            };
        }
    }
}
