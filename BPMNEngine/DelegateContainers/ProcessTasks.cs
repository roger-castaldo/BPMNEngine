namespace BPMNEngine.DelegateContainers
{
    /// <summary>
    /// This class is used to house all the Tasks delegates for a business process. 
    /// This can be defined at either the BusinessProcess constructor level for defining it 
    /// against all instances or at the BeginProcess level to defining it against a 
    /// specific instance
    /// </summary>
    public record ProcessTasks
    {
        /// <summary>
        /// A delegate called to execute a Business Rule Task
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:businessRuleTask id="BusinessRule_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <Analysis outputVariable="averageCost" inputs="Cost" formula="Average"/>
        ///    <Analysis outputVariable="totalQuantity" inputs="Quantity" formula="Sum"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public void ProcessBusinessRuleTask(ITask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Analysis"){
        ///                     switch(xn.Attributes["formula"].Value){
        ///                         case "Average":
        ///                             decimal avgSum=0;
        ///                             decimal avgCount=0;
        ///                             foreach (Hashtable item in task["Items"]){
        ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
        ///                                     avgSum+=(decimal)item[xn.Attributes["inputs"].Value];
        ///                                     avgCount++;
        ///                                 }
        ///                             }
        ///                             task[xn.Attriubtes["outputVariable"].Value] = avgSum/avgCount;
        ///                             break;
        ///                         case "Sum":
        ///                             decimal sum=0;
        ///                             foreach (Hashtable item in task["Items"]){
        ///                                 if (item.ContainsKey(xn.Attributes["inputs"].Value)){
        ///                                     sum+=(decimal)item[xn.Attributes["inputs"].Value];
        ///                                 }
        ///                             }
        ///                             task[xn.Attriubtes["outputVariable"].Value] = sum;
        ///                             break;
        ///                     }
        ///                 }
        ///             }
        ///         }
        ///     }
        /// }
        /// </code>
        /// </summary>
        public ProcessTask ProcessBusinessRuleTask { get; init; }
        /// <summary>
        /// A delegate called to start a Manual Task
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:manualTask id="ManualTask_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public void BeginManualTask(IManualTask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Question"){
        ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
        ///                     task[xn.Attributes["answer_property"].Value] = Console.ReadLine();
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     task.MarkComplete();
        /// }
        /// </code>
        /// </summary>
        public StartManualTask BeginManualTask { get; init; }
        /// <summary>
        /// A delegate called to execute a Receive Task
        /// </summary>
        public ProcessTask ProcessReceiveTask { get; init; }
        /// <summary>
        /// A delegate called to execute a Script Task.  This is called after any internal script extension elements have been processed.
        /// </summary>
        public ProcessTask ProcessScriptTask { get; init; }
        /// <summary>
        /// A delegate called to exeucte a Send Task.
        /// </summary>
        public ProcessTask ProcessSendTask { get; init; }
        /// <summary>
        /// A delegate called to execute a Service Task
        /// </summary>
        public ProcessTask ProcessServiceTask { get; init; }
        /// <summary>
        /// A delegate called to execute a Task
        /// </summary>
        public ProcessTask ProcessTask { get; init; }
        /// <summary>
        /// A delegate called to execute a CallActivity
        /// </summary>
        public ProcessTask CallActivity { get; init; }
        /// <summary>
        /// A delegate called to start a User Task
        /// <code>
        /// <![CDATA[
        /// XML:
        /// <bpmn:userTask id="UserTask_0ikjhwl">
        ///  <bpmn:extensionElements>
        ///    <Question prompt="What is the answer to the life, universe and everything" answer_property="answer"/>
        ///  </bpmn:extensionElements>
        ///  <bpmn:outgoing>SequenceFlow_1kh3jxa</bpmn:outgoing>
        /// </bpmn:startEvent>
        /// ]]>
        /// 
        /// public void BeginUserTask(IUserTask task)
        ///     if (task.ExtensionElement != null){
        ///         foreach (XmlNode xn in task.ExtensionElement.SubNodes){
        ///             if (xn.NodeType == XmlNodeType.Element)
        ///             {
        ///                 if (xn.Name=="Question"){
        ///                     Console.WriteLine(string.format("{0}?",xn.Attributes["prompt"].Value));
        ///                     task[xn.Attributes["answer_property"].Value] = Console.ReadLine();
        ///                     Console.WriteLine("Who Are You?");
        ///                     task.UserID = Console.ReadLine();
        ///                 }
        ///             }
        ///         }
        ///     }
        ///     task.MarkComplete();
        /// }
        /// </code>
        /// </summary>
        public StartUserTask BeginUserTask { get; init; }

        internal static ProcessTasks Merge(ProcessTasks source, ProcessTasks append)
        {
            source??=new ProcessTasks();
            append??=new ProcessTasks();
            return new ProcessTasks()
            {
                ProcessBusinessRuleTask = append.ProcessBusinessRuleTask??source.ProcessBusinessRuleTask,
                BeginManualTask = append.BeginManualTask??source.BeginManualTask,
                ProcessReceiveTask = append.ProcessReceiveTask ?? source.ProcessReceiveTask,
                ProcessScriptTask = append.ProcessScriptTask ?? source.ProcessScriptTask,
                ProcessSendTask = append.ProcessSendTask ?? source.ProcessSendTask,
                ProcessServiceTask = append.ProcessServiceTask ?? source.ProcessServiceTask,
                ProcessTask = append.ProcessTask ?? source.ProcessTask,
                CallActivity = append.CallActivity ?? source.CallActivity,
                BeginUserTask = append.BeginUserTask ?? source.BeginUserTask
            };
        }
    }
}
