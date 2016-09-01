using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Elements.Diagrams
{
    internal enum BPMIcons
    {
        StartEvent,
        MessageStartEvent,
        TimerStartEvent,
        ConditionalStartEvent,
        SignalStartEvent,
        MessageIntermediateThrowEvent,
        EscalationIntermediateThrowEvent,
        LinkIntermediateThrowEvent,
        CompensationIntermediateThrowEvent,
        SignalIntermediateThrowEvent,
        MessageIntermediateCatchEvent,
        TimerIntermediateCatchEvent,
        LinkIntermediateCatchEvent,
        ConditionalIntermediateCatchEvent,
        SignalIntermediateCatchEvent,
        EndEvent,
        MessageEndEvent,
        EscalationEndEvent,
        ErrorEndEvent,
        CompensationEndEvent,
        SignalEndEvent,
        TerminateEndEvent,
        ExclusiveGateway,
        ParallelGateway,
        InclusiveGateway,
        ComplexGateway,
        EventBasedGateway,
        Task,
        SendTask,
        ReceiveTask,
        UserTask,
        ManualTask,
        ServiceTask,
        ScriptTask,
        BusinessRuleTask
    }
}
