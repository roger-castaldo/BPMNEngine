using BPMNEngine.Elements;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Gateways;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.Interfaces.Extensions;
using BPMNEngine.Interfaces.Variables;
using BPMNEngine.Scheduling;

namespace BPMNEngine
{
    public sealed partial class BusinessProcess
    {
        private static void TriggerDelegateAsync(Delegate dgate, params object[] pars)
        {
            if (dgate!=null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    dgate.DynamicInvoke(pars);
                });
            }
        }
        private async ValueTask<IEnumerable<AHandlingEvent>> GetEventHandlersAsync(EventSubTypes type, object? data, AFlowNode source, IReadonlyVariables variables, ILogger logger)
        {
            var handlerGroup = (await eventHandlers
                    .GroupByAsync(handler => handler.EventCostAsync(type, data, source, variables, logger))
                    .ConfigureAwait(true)
                )
                .OrderBy(grp => grp.Key)
                .FirstOrDefault();

            switch (type)
            {
                case EventSubTypes.Timer:
                    if (handlerGroup!=null && handlerGroup.Key>0)
                        return [];
                    break;
            }

            return (handlerGroup==null || handlerGroup.Key==int.MaxValue ? Array.Empty<AHandlingEvent>() : handlerGroup.ToList());
        }

        internal async ValueTask ProcessStepCompleteAsync(ProcessInstance instance, string sourceID, string outgoingID)
        {
            using var logger = instance.GetLogger(sourceID);
            if (sourceID!=null)
            {
                IElement elem = GetElement(sourceID);
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer vars = new(sourceID, instance);
                    (await GetEventHandlersAsync(EventSubTypes.Timer, null, node, vars, logger)).ForEach(ahe =>
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
                        .OfType<IElement>()
                        .Where(child => instance.State.Path.AbortableSteps.Contains(child.ID))
                        .ForEach(child => AbortStep(instance, sourceID, child, vars));
                }
            }
            logger.LogDebug("Process Step has been completed");
            if (outgoingID != null)
            {
                IElement elem = GetElement(outgoingID);
                if (elem != null)
                    await ProcessElementAsync(instance, sourceID, elem);
            }
        }

        internal async ValueTask ProcessStepErrorAsync(ProcessInstance instance, IElement step, Exception ex)
        {
            bool success = false;
            using var logger = instance.GetLogger(step);
            logger.LogInformation("Process Step Error occured, checking for valid Intermediate Catch Event");
            if (step is AFlowNode node)
            {
                var events = await GetEventHandlersAsync(EventSubTypes.Error, ex, node, new ReadOnlyProcessVariablesContainer(step.ID, instance, ex), logger);
                if (events.Any())
                {
                    success=true;
                    await events.ForEachAsync(ahe =>
                    {
                        logger.LogDebug("Valid Error handle located at {ElementID}", ahe.ID);
                        return ProcessElementAsync(instance, step.ID, ahe);
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

        private async ValueTask ProcessElementAsync(ProcessInstance instance, string sourceID, IElement elem)
        {
            if (instance.IsSuspended)
            {
                instance.State.Path.SuspendElement(sourceID, elem, instance.GetLogger(elem));
                instance.MreSuspend.Set();
            }
            else
            {
                bool abort = false;
                using var logger = instance.GetLogger(elem);
                logger.LogDebug("Processing Element from source {Source}", sourceID);
                if (elem is AFlowNode node)
                {
                    ReadOnlyProcessVariablesContainer ropvc = new(sourceID, instance);
                    await (await GetEventHandlersAsync(EventSubTypes.Conditional, null, node, ropvc, logger))
                        .ForEachTaskAsync(async (ahe) =>
                        {
                            await ProcessEventAsync(instance, elem.ID, ahe);
                            abort|=(ahe is BoundaryEvent @event &&@event.CancelActivity);
                        })
                        .ConfigureAwait(true);
                    if (!abort)
                    {
                        (await GetEventHandlersAsync(EventSubTypes.Timer, null, node, ropvc, logger)).ForEach(ahe =>
                        {
                            TimeSpan? ts = ahe.GetTimeout(ropvc, logger);
                            if (ts.HasValue)
                            {
                                instance.State.Path.DelayEventStart(ahe, elem.ID, ts.Value, instance.GetLogger(ahe));
                                StepScheduler.Instance.DelayStart(ts.Value, instance, (BoundaryEvent)ahe, elem.ID);
                            }
                        });
                    }
                }
                if (!abort)
                {
                    if (elem is IFlowElement flowElement)
                        BusinessProcess.ProcessFlowElement(instance, flowElement);
                    else if (elem is AGateway aGateway)
                        await ProcessGatewayAsync(instance, sourceID, aGateway);
                    else if (elem is AEvent aEvent)
                        await ProcessEventAsync(instance, sourceID, aEvent);
                    else if (elem is ATask aTask)
                        await BusinessProcess.ProcessTaskAsync(instance, sourceID, aTask);
                    else if (elem is SubProcess subProcess)
                        await BusinessProcess.ProcessSubProcessAsync(instance, sourceID, subProcess);
                }
            }
        }

        private static async ValueTask ProcessSubProcessAsync(ProcessInstance instance, string sourceID, SubProcess esp)
        {
            ReadOnlyProcessVariablesContainer variables = new(new ProcessVariablesContainer(esp.ID, instance));
            if (await esp.IsStartValidAsync(variables, instance.Delegates.Validations.IsProcessStartValid, instance.GetLogger(esp)))
            {
                var startEvent = await esp.StartEvents.FirstOrDefaultAsync(se => se.IsEventStartValidAsync(variables, instance.Delegates.Validations.IsEventStartValid, instance.GetLogger(se)));
                if (startEvent!=null)
                {
                    using var logger = instance.GetLogger(startEvent);
                    logger.LogInformation("Valid Sub Process Start for Sub Process[{SubProcessID}] located, beginning process", esp.ID);
                    instance.State.Path.StartFlowNode(esp, sourceID, instance.GetLogger(esp));
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.SubProcesses.Started,
                        esp,
                        variables
                    );
                    instance.State.Path.StartFlowNode(startEvent, null, instance.GetLogger(startEvent));
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Events.Started,
                        startEvent,
                        variables
                    );
                    instance.State.Path.SucceedFlowNode(startEvent, instance.GetLogger(startEvent));
                    BusinessProcess.TriggerDelegateAsync(
                        instance.Delegates.Events.Events.Completed,
                        startEvent,
                        variables
                    );
                }
            }
        }

        private static async ValueTask ProcessTaskAsync(ProcessInstance instance, string sourceID, ATask tsk)
        {
            using var logger = instance.GetLogger(tsk);
            instance.State.Path.StartFlowNode(tsk, sourceID, logger);
            BusinessProcess.TriggerDelegateAsync(
                instance.Delegates.Events.Tasks.Started,
                tsk,
                new ReadOnlyProcessVariablesContainer(tsk.ID, instance)
            );
            try
            {
                ProcessVariablesContainer variables = new(tsk.ID, instance);
                (Tasks.ExternalTask? task,ProcessTask? delTask) = (tsk) switch
                {
                    (BusinessRuleTask) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.ProcessBusinessRuleTask),
                    (ReceiveTask) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.ProcessReceiveTask),
                    (SendTask) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.ProcessSendTask),
                    (ServiceTask) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.ProcessServiceTask),
                    (BPMNEngine.Elements.Processes.Tasks.Task) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.ProcessTask),
                    (ScriptTask) => (new Tasks.ExternalTask(tsk, variables, instance),instance.Delegates.Tasks.ProcessScriptTask),
                    (CallActivity) => (new Tasks.ExternalTask(tsk, variables, instance), instance.Delegates.Tasks.CallActivity),
                    (ManualTask)=>(new Tasks.ManualTask(tsk, variables, instance),null),
                    (UserTask)=>(new Tasks.UserTask(tsk,variables,instance),null),
                    _ => (null,null)

                };
                var success = true;
                if (task!=null)
                {
                    foreach (var taskExtension in (tsk.ExtensionElement?.Extensions.OfType<ITaskExtensionElementElement>()?? []))
                    {
                        success = await taskExtension.ExecuteTaskExtensionAsync(task);
                        if (task.Aborted || !success)
                            break;
                    }
                    if (!task.Aborted && success)
                    {
                        if (task is Tasks.UserTask ut)
                            TriggerDelegateAsync(
                                instance.Delegates.Tasks.BeginUserTask,
                                ut
                            );
                        else if (task is Tasks.ManualTask mt)
                            TriggerDelegateAsync(
                                instance.Delegates.Tasks.BeginManualTask,
                                mt
                            );
                        else
                        {
                            delTask?.Invoke(task);
                            if (!task.Aborted)
                                instance.MergeVariables(task);
                        }
                    }
                    if (!success && !task.Aborted)
                        instance.State.Path.FailFlowNode(tsk, logger);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured processing the given task");
                BusinessProcess.TriggerDelegateAsync(
                    instance.Delegates.Events.Tasks.Error,
                    tsk,
                    new ReadOnlyProcessVariablesContainer(tsk.ID, instance, e)
                );
                instance.State.Path.FailFlowNode(tsk,logger, error: e);
            }
        }

        internal async ValueTask ProcessEventAsync(ProcessInstance instance, string sourceID, AEvent evnt)
        {
            using var logger = instance.GetLogger(evnt);
            if (evnt is IntermediateCatchEvent)
            {
                SubProcess sp = (SubProcess)evnt.SubProcess;
                if (sp != null)
                    instance.State.Path.StartFlowNode(sp, sourceID, instance.GetLogger(sp));
            }
            instance.State.Path.StartFlowNode(evnt, sourceID, logger);
            TriggerDelegateAsync(
                instance.Delegates.Events.Events.Started,
                evnt,
                new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
            );
            if (evnt is BoundaryEvent @event && @event.CancelActivity)
                AbortStep(instance, sourceID, GetElement(@event.AttachedToID), new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
            bool success = true;
            TimeSpan? ts = ((evnt is IntermediateCatchEvent || evnt is IntermediateThrowEvent) ?
                 evnt.GetTimeout(new ReadOnlyProcessVariablesContainer(evnt.ID, instance), logger)
                 : null);
            if (ts.HasValue)
            {
                instance.State.SuspendStep(sourceID, evnt.ID, ts.Value,logger);
                if (ts.Value.TotalMilliseconds > 0)
                {
                    StepScheduler.Instance.Sleep(ts.Value, instance, evnt);
                    return;
                }
                else
                    success = true;
            }
            else if (evnt is IntermediateThrowEvent intermediateThrowEvent)
            {
                if (intermediateThrowEvent.SubType.HasValue)
                    (await GetEventHandlersAsync(evnt.SubType.Value, intermediateThrowEvent.Message, evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance), logger))
                        .ForEach(tsk => { ProcessEventAsync(instance, evnt.ID, tsk); });
            }
            else if (instance.Delegates.Validations.IsEventStartValid != null && (evnt is IntermediateCatchEvent || evnt is StartEvent))
            {
                try
                {
                    success = instance.Delegates.Validations.IsEventStartValid(evnt, new ReadOnlyProcessVariablesContainer(evnt.ID, instance));
                }
                catch (Exception e)
                {
                    logger.LogError(e, "An error occured attempting to check the validity of an event start");
                    success = false;
                }
            }
            if (!success)
            {
                instance.State.Path.FailFlowNode(evnt, logger);
                TriggerDelegateAsync(
                    instance.Delegates.Events.Events.Error,
                    evnt,
                    new ReadOnlyProcessVariablesContainer(evnt.ID, instance)
                );
            }
            else
            {
                instance.State.Path.SucceedFlowNode(evnt, logger);
                TriggerDelegateAsync(
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
                    )
                    {
                        instance.State.Path.SucceedFlowNode(sp, instance.GetLogger(sp));
                        TriggerDelegateAsync(
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
                                TriggerDelegateAsync(
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
                            TriggerDelegateAsync(
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
            instance.State.Path.AbortStep(sourceID, element.ID, instance.GetLogger(element));
            BusinessProcess.TriggerDelegateAsync(
                instance.Delegates.Events.OnStepAborted,
                element, GetElement(sourceID),
                variables
            );
            if (element is SubProcess process)
            {
                process.Children.OfType<IElement>().ForEach(child =>
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

        private async ValueTask ProcessGatewayAsync(ProcessInstance instance, string sourceID, AGateway gw)
        {
            using var logger = instance.GetLogger(gw);
            if (instance.State.Path.ProcessGateway(gw, sourceID, logger))
            {
                TriggerDelegateAsync(
                    instance.Delegates.Events.Gateways.Started,
                    gw,
                    new ReadOnlyProcessVariablesContainer(gw.ID, instance)
                );
                IEnumerable<string> outgoings = null;
                try
                {
                    outgoings = await gw.EvaulateOutgoingPathsAsync(definition, instance.Delegates.Validations.IsFlowValid, new ReadOnlyProcessVariablesContainer(gw.ID, instance), logger);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "An error occured attempting to evaulate the outgoing gateway paths");
                    TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Error,
                        gw,
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance, e)
                    );
                    outgoings = null;
                }
                if (outgoings==null || !outgoings.Any())
                {
                    instance.State.Path.FailFlowNode(gw,logger);
                    TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Error,
                        gw,
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance, new Exception("No valid outgoing path located"))
                    );
                }
                else
                {
                    instance.State.Path.SucceedFlowNode(gw, logger, outgoing: outgoings);
                    TriggerDelegateAsync(
                        instance.Delegates.Events.Gateways.Completed,
                        gw,
                        new ReadOnlyProcessVariablesContainer(gw.ID, instance)
                    );
                }
            }
        }

        private static void ProcessFlowElement(ProcessInstance instance, IFlowElement flowElement)
        {
            instance.State.Path.ProcessFlowElement(flowElement, instance.GetLogger(flowElement));
            Delegate delCall = instance.Delegates.Events.Flows.SequenceFlow;
            if (flowElement is MessageFlow)
                delCall = instance.Delegates.Events.Flows.MessageFlow;
            else if (flowElement is Association)
                delCall = instance.Delegates.Events.Flows.AssociationFlow;
            TriggerDelegateAsync(
                delCall,
                flowElement,
                new ReadOnlyProcessVariablesContainer(flowElement.ID, instance)
            );
        }
    }
}
