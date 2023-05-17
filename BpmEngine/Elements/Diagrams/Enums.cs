using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Elements.Diagrams
{
    internal enum BPMIcons
    {
        StartEvent,
        MessageStartEvent,
        TimerStartEvent,
        ConditionalStartEvent,
        SignalStartEvent,
        IntermediateThrowEvent,
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
        BusinessRuleTask,
        InteruptingMessageBoundaryEvent,
        InteruptingTimerBoundaryEvent,
        InteruptingEscalationBoundaryEvent,
        InteruptingConditionalBoundaryEvent,
        InteruptingErrorBoundaryEvent,
        InteruptingSignalBoundaryEvent,
        InteruptingCompensationBoundaryEvent,
        NonInteruptingMessageBoundaryEvent,
        NonInteruptingTimerBoundaryEvent,
        NonInteruptingEscalationBoundaryEvent,
        NonInteruptingConditionalBoundaryEvent,
        NonInteruptingSignalBoundaryEvent
    }
}
