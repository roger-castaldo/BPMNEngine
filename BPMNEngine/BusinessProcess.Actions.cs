using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements;
using BPMNEngine.Scheduling;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Elements.Processes.Gateways;

namespace BPMNEngine
{
    public sealed partial class BusinessProcess
    {
        private static void TriggerDelegateAsync(Delegate dgate, params object?[]? pars)
        {
            if (dgate!=null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    dgate.DynamicInvoke(pars);
                });
            }
        }
        private IEnumerable<AHandlingEvent> GetEventHandlers(EventSubTypes type, object data, AFlowNode source, IReadonlyVariables variables)
        {
            var handlerGroup = eventHandlers
                .GroupBy(handler => handler.EventCost(type, data, source, variables))
                .OrderBy(grp => grp.Key)
                .FirstOrDefault();

            switch (type)
            {
                case EventSubTypes.Timer:
                    if (handlerGroup!=null && handlerGroup.Key>0)
                        return Array.Empty<AHandlingEvent>();
                    break;
            }

            return (handlerGroup==null || handlerGroup.Key==int.MaxValue ? Array.Empty<AHandlingEvent>() : handlerGroup.ToList());
        }

        internal void ProcessStepComplete(ProcessInstance instance, string sourceID, string outgoingID)
        {
            if (sourceID!=null)
            {
                IElement elem = GetElement(sourceID);
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer vars = new(sourceID, instance);
                    GetEventHandlers(EventSubTypes.Timer, null, node, vars).ForEach(ahe =>
                    {
                        if (instance.State.Path.GetStatus(ahe.ID)==StepStatuses.WaitingStart)
                        {
                            StepScheduler.Instance.AbortDelayedEvent(instance, (BoundaryEvent)ahe, sourceID);
                            AbortStep(instance, sourceID, ahe, vars);
                        }
                    });
                }
                if (elem is SubProcess subProcess)
                {
                    ReadOnlyProcessVariablesContainer vars = new(sourceID, instance);
                    subProcess.Children
                        .Where(child=>instance.State.Path.AbortableSteps.Contains(child.ID))
                        .ForEach(child=>AbortStep(instance, sourceID, child, vars));
                }
            }
            WriteLogLine(sourceID, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Process Step[{0}] has been completed", sourceID));
            if (outgoingID != null)
            {
                IElement elem = GetElement(outgoingID);
                if (elem != null)
                    ProcessElement(instance, sourceID, elem);
            }
        }

        internal void ProcessStepError(ProcessInstance instance, IElement step, Exception ex)
        {
            instance.WriteLogLine(step, LogLevel.Information, new StackFrame(1, true), DateTime.Now, "Process Step Error occured, checking for valid Intermediate Catch Event");
            bool success = false;
            if (step is AFlowNode node)
            {
                var events = GetEventHandlers(EventSubTypes.Error, ex, node, new ReadOnlyProcessVariablesContainer(step.ID, instance, ex));
                if (events.Any())
                {
                    success=true;
                    events.ForEach(ahe =>
                    {
                        instance.WriteLogLine(step, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Valid Error handle located at {0}", ahe.ID));
                        ProcessElement(instance, step.ID, ahe);
                    });
                }
            }
            if (!success)
            {
                if (((IStepElement)step).SubProcess!=null)
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.SubProcesses.Error,
                        (IStepElement)((IStepElement)step).SubProcess, 
                        new ReadOnlyProcessVariablesContainer(step.ID, instance, ex)
                    );
                else
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Processes.Error, 
                        ((IStepElement)step).Process, 
                        step, 
                        new ReadOnlyProcessVariablesContainer(step.ID, instance, ex)
                    );
            }
        }

        private void ProcessElement(ProcessInstance instance, string sourceID, IElement elem)
        {
            if (instance.IsSuspended)
            {
                instance.State.Path.SuspendElement(sourceID, elem);
                instance.MreSuspend.Set();
            }
            else
            {
                instance.WriteLogLine(sourceID, LogLevel.Debug, new StackFrame(1, true), DateTime.Now, string.Format("Processing Element {0} from source {1}", new object[] { elem.ID, sourceID }));
                bool abort = false;
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer ropvc = new(sourceID, instance);
                    var evnts = GetEventHandlers(EventSubTypes.Conditional, null, node, ropvc);
                    evnts.ForEach(ahe =>
                    {
                        ProcessEvent(instance, elem.ID, ahe);
                        abort|=(ahe is BoundaryEvent @event &&@event.CancelActivity);
                    });
                    if (!abort)
                    {
                        GetEventHandlers(EventSubTypes.Timer, null, node, ropvc).ForEach(ahe =>
                        {
                            TimeSpan? ts = ahe.GetTimeout(ropvc);
                            if (ts.HasValue)
                            {
                                instance.State.Path.DelayEventStart(ahe, elem.ID, ts.Value);
                                StepScheduler.Instance.DelayStart(ts.Value, instance, (BoundaryEvent)ahe, elem.ID);
                            }
                        });
                    }
                }
                if (elem is IFlowElement flowElement)
                    BusinessProcess.ProcessFlowElement(instance, flowElement);
                else if (elem is AGateway aGateway)
                    ProcessGateway(instance, sourceID, aGateway);
                else if (elem is AEvent aEvent)
                    ProcessEvent(instance, sourceID, aEvent);
                else if (elem is ATask aTask)
                    BusinessProcess.ProcessTask(instance, sourceID, aTask);
                else if (elem is SubProcess subProcess)
                    BusinessProcess.ProcessSubProcess(instance, sourceID, subProcess);
            }
        }

        private static void ProcessSubProcess(ProcessInstance instance, string sourceID, SubProcess esp)
        {
            ReadOnlyProcessVariablesContainer variables = new(new ProcessVariablesContainer(esp.ID, instance));
            if (esp.IsStartValid(variables, instance.Delegates.Validations.IsProcessStartValid))
            {
                var startEvent = esp.StartEvents.FirstOrDefault(se => se.IsEventStartValid(variables, instance.Delegates.Validations.IsEventStartValid));
                if (startEvent!=null)
                {
                    instance.WriteLogLine(startEvent, LogLevel.Information, new StackFrame(1, true), DateTime.Now, string.Format("Valid Sub Process Start[{0}] located, beginning process", startEvent.ID));
                    instance.State.Path.StartFlowNode(esp, sourceID);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.SubProcesses.Started, 
                        esp, 
                        variables
                    );
                    instance.State.Path.StartFlowNode(startEvent, null);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Events.Started,
                        startEvent, 
                        variables
                    );
                    instance.State.Path.SucceedFlowNode(startEvent);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Events.Completed, 
                        startEvent, 
                        variables
                    );
                }
            }
        }

        private static void ProcessTask(ProcessInstance instance, string sourceID, ATask tsk)
        {
            instance.State.Path.StartFlowNode(tsk, sourceID);
            BusinessProcess.TriggerDelegateAsync(
                instance.Delegates.Events.Tasks.Started, 
                tsk, 
                new ReadOnlyProcessVariablesContainer(tsk.ID, instance)
            );
            try
            {
                ProcessVariablesContainer variables = new(tsk.ID, instance);
                Tasks.ExternalTask task = null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                    case "ReceiveTask":
                    case "SendTask":
                    case "ServiceTask":
                    case "Task":
                    case "ScriptTask":
                    case "CallActivity":
                        task = new Tasks.ExternalTask(tsk, variables, instance);
                        break;
                }
                ProcessTask delTask = null;
                switch (tsk.GetType().Name)
                {
                    case "BusinessRuleTask":
                        delTask = instance.Delegates.Tasks.ProcessBusinessRuleTask;
                        break;
                    case "ManualTask":
                        BusinessProcess.TriggerDelegateAsync(
                            instance.Delegates.Tasks.BeginManualTask, 
                            new Tasks.ManualTask(tsk, variables, instance)
                        );
                        break;
                    case "ReceiveTask":
                        delTask = instance.Delegates.Tasks.ProcessReceiveTask;
                        break;
                    case "ScriptTask":
                        ((ScriptTask)tsk).ProcessTask(task, instance.Delegates.Tasks.ProcessScriptTask);
                        break;
                    case "SendTask":
                        delTask = instance.Delegates.Tasks.ProcessSendTask;
                        break;
                    case "ServiceTask":
                        delTask = instance.Delegates.Tasks.ProcessServiceTask;
                        break;
                    case "Task":
                        delTask = instance.Delegates.Tasks.ProcessTask;
                        break;
                    case "CallActivity":
                        delTask = instance.Delegates.Tasks.CallActivity;
                        break;
                    case "UserTask":
                        BusinessProcess.TriggerDelegateAsync(
                            instance.Delegates.Tasks.BeginUserTask,
                            new Tasks.UserTask(tsk, variables, instance)
                        );
                        break;
                }
                delTask?.Invoke(task);
                if (task!=null && !task.Aborted)
                    instance.MergeVariables(task);
            }
            catch (Exception e)
            {
                instance.WriteLogException(tsk, new StackFrame(1, true), DateTime.Now, e);
                BusinessProcess.TriggerDelegateAsync(
                    instance.Delegates.Events.Tasks.Error, 
                    tsk, 
                    new ReadOnlyProcessVariablesContainer(tsk.ID, instance, e) 
                );
                instance.State.Path.FailFlowNode(tsk, error: e);
            }
        }

        internal void ProcessEvent(ProcessInstance instance, string sourceID, AEvent evnt)
        {
            if (evnt is IntermediateCatchEvent)
            {
                SubProcess sp = (SubProcess)evnt.SubProcess;
                if (sp != null)
                    instance.State.Path.StartFlowNode(sp, sourceID);
            }
            instance.State.Path.StartFlowNode(evnt, sourceID);
            BusinessProcess.TriggerDelegateAsync(
                instance.Delegates.Events.Events.Started,
                evnt, 
                new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
            );
            if (evnt is BoundaryEvent @event && @event.CancelActivity)
                AbortStep(instance, sourceID, GetElement(@event.AttachedToID), new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
            bool success = true;
            TimeSpan? ts = null;
            if (evnt is IntermediateCatchEvent || evnt is IntermediateThrowEvent)
                ts = evnt.GetTimeout(new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
            if (ts.HasValue)
            {
                instance.State.SuspendStep(sourceID, evnt.ID, ts.Value);
                if (ts.Value.TotalMilliseconds > 0)
                {
                    StepScheduler.Instance.Sleep(ts.Value, instance, evnt);
                    return;
                }
                else
                    success = true;
            }
            else if (evnt is IntermediateThrowEvent event1)
            {
                if (evnt.SubType.HasValue)
                    GetEventHandlers(evnt.SubType.Value, event1.Message, evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance))
                        .ForEach(tsk => { ProcessEvent(instance, evnt.ID, tsk); });
            }
            else if (instance.Delegates.Validations.IsEventStartValid != null && (evnt is IntermediateCatchEvent || evnt is StartEvent))
            {
                try
                {
                    success = instance.Delegates.Validations.IsEventStartValid(evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(evnt, new StackFrame(1, true), DateTime.Now, e);
                    success = false;
                }
            }
            if (!success)
            {
                instance.State.Path.FailFlowNode(evnt);
                BusinessProcess.TriggerDelegateAsync(
                    instance.Delegates.Events.Events.Error,
                    evnt, 
                    new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
                );
            }
            else
            {
                instance.State.Path.SucceedFlowNode(evnt);
                BusinessProcess.TriggerDelegateAsync(
                    instance.Delegates.Events.Events.Completed, 
                    evnt, 
                    new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
                );
                if (evnt is EndEvent endEvent)
                {
                    var sp = endEvent.SubProcess as SubProcess;
                    if (sp!=null && 
                        (
                            !endEvent.IsProcessEnd
                            ||(endEvent.IsProcessEnd && !endEvent.IsTermination)
                        )
                    ){
                        instance.State.Path.SucceedFlowNode(sp);
                        BusinessProcess.TriggerDelegateAsync(
                            instance.Delegates.Events.SubProcesses.Completed,
                            sp,
                            new ReadOnlyProcessVariablesContainer(sp.ID, instance)
                        );
                    }
                    else if (endEvent.IsProcessEnd)
                    {
                        if (!endEvent.IsTermination)
                        {
                            if (sp==null)
                            {
                                BusinessProcess.TriggerDelegateAsync(
                                    instance.Delegates.Events.Processes.Completed,
                                    endEvent.Process,
                                    new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
                                );
                                instance.CompleteProcess();
                            }
                        }
                        else
                        {
                            ReadOnlyProcessVariablesContainer vars = new(evnt.ID, instance);
                            instance.State.AbortableSteps.ForEach(str => { AbortStep(instance, evnt.ID, GetElement(str), vars); });
                            BusinessProcess.TriggerDelegateAsync(
                                instance.Delegates.Events.Processes.Completed,
                                endEvent.Process,
                                new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
                            );
                            instance.CompleteProcess();
                        }
                    }
                }
            }
        }

        private void AbortStep(ProcessInstance instance, string sourceID, IElement element, IReadonlyVariables variables)
        {
            instance.State.Path.AbortStep(sourceID, element.ID);
            BusinessProcess.TriggerDelegateAsync(
                instance.Delegates.Events.OnStepAborted, 
                element, GetElement(sourceID), 
                variables
            );
            if (element is SubProcess process)
            {
                process.Children.ForEach(child =>
                {
                    bool abort = false;
                    switch (instance.State.Path.GetStatus(child.ID))
                    {
                        case StepStatuses.Suspended:
                            abort=true;
                            StepScheduler.Instance.AbortSuspendedElement(instance, child.ID);
                            break;
                        case StepStatuses.Waiting:
                        case StepStatuses.Started:
                            abort=true;
                            break;
                    }
                    if (abort)
                        AbortStep(instance, sourceID, child, variables);
                });
            }
        }

        private void ProcessGateway(ProcessInstance instance, string sourceID, AGateway gw)
        {
            if (instance.State.Path.ProcessGateway(gw, sourceID))
            {
                BusinessProcess.TriggerDelegateAsync(
                    instance.Delegates.Events.Gateways.Started, 
                    gw, 
                    new ReadOnlyProcessVariablesContainer(gw.ID, instance)
                );
                IEnumerable<string> outgoings = null;
                try
                {
                    outgoings = gw.EvaulateOutgoingPaths(definition, instance.Delegates.Validations.IsFlowValid, new ReadOnlyProcessVariablesContainer(gw.ID, instance));
                }
                catch (Exception e)
                {
                    instance.WriteLogException(gw, new StackFrame(1, true), DateTime.Now, e);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Error, 
                        gw, 
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance, e)
                    );
                    outgoings = null;
                }
                if (outgoings==null || !outgoings.Any())
                {
                    instance.State.Path.FailFlowNode(gw);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Error, 
                        gw, 
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance, new Exception("No valid outgoing path located"))
                    );
                }
                else
                {
                    instance.State.Path.SucceedFlowNode(gw, outgoing: outgoings);
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Completed, 
                        gw, 
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance)
                    );
                }
            }
        }

        private static void ProcessFlowElement(ProcessInstance instance, IFlowElement flowElement)
        {
            instance.State.Path.ProcessFlowElement(flowElement);
            Delegate delCall = instance.Delegates.Events.Flows.SequenceFlow;
            if (flowElement is MessageFlow)
                delCall = instance.Delegates.Events.Flows.MessageFlow;
            else if (flowElement is Association)
                delCall = instance.Delegates.Events.Flows.AssociationFlow;
            BusinessProcess.TriggerDelegateAsync(
                delCall, 
                flowElement, 
                new ReadOnlyProcessVariablesContainer(flowElement.ID, instance)
            );
        }
    }
}
