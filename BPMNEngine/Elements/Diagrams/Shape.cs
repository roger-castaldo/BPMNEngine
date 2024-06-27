using BPMNEngine.Attributes;
using BPMNEngine.Drawing;
using BPMNEngine.Elements.Collaborations;
using BPMNEngine.Elements.Processes;
using BPMNEngine.Elements.Processes.Events;
using BPMNEngine.Elements.Processes.Tasks;
using BPMNEngine.Interfaces.Elements;
using BPMNEngine.State;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Elements.Diagrams
{
    [XMLTagAttribute("bpmndi", "BPMNShape")]
    [RequiredAttributeAttribute("id")]
    [ValidParent(typeof(Plane))]
    internal record Shape : ADiagramElement, IRenderingElement
    {
        public Shape(XmlElement elem, XmlPrefixMap map, AElement parent)
            : base(elem, map, parent) { }
        public override RectF Rectangle => Children
            .OfType<Bounds>()
            .Select(elem => elem.Rectangle)
            .FirstOrDefault(new Rect(0, 0, 0, 0));

        public Label Label => (Label)Children
            .FirstOrDefault(elem => elem is Label);

        public override bool IsValid(out IEnumerable<string> err)
        {
            var res = base.IsValid(out err);
            if (!Children.Any(elem => elem is Bounds))
            {
                err =(err ?? []).Append("No bounds specified for the shape.");
                return false;
            }
            return res;
        }

        public void Render(ICanvas surface, ProcessPath path, Definition definition)
        {
            var relem = definition.LocateElement(BPMNElement);
            var color = Diagram.GetColor(path.GetStatus(relem.ID));
            if (relem is Lane || relem is Participant || relem is LaneSet)
                RenderLane(relem, surface, color);
            else
            {
                if (relem is AEvent aEvent)
                    RenderEvent(aEvent, surface, color);
                else if (relem is Processes.Gateways.AGateway aGateway)
                    RenderGateway(aGateway, surface, color);
                else if (relem is ATask aTask)
                    RenderTask(aTask, surface, color);
                else if (relem is TextAnnotation)
                    RenderTextAnnotation(surface, color);
                else if (relem is SubProcess)
                    RenderSubProcess(surface, color);

                if (relem.ToString()!="")
                {
                    surface.FontColor=color;
                    surface.DrawString(relem.ToString(), (Label?.Bounds.Rectangle??Rectangle), HorizontalAlignment.Center, VerticalAlignment.Center);
                }
            }
        }

        #region constants
        private const float _PEN_SIZE = 2.0f;
        private const float _ACTIVITY_PEN_SIZE = 4.5f;
        private const float _LANE_CORNER_RADIUS = 3.0f;
        private const float _TASK_RADIUS = 10f;
        private const float _SUB_PROCESS_CORNER_RADIUS = 10f;
        private const float _CALL_ACTIVITY_SQUARE_SIZE = 10.0f;
        private const float _CALL_ACTIVITY_SQUARE_PADDING = 5.0f;
        #endregion

        #region Event
        private void RenderEvent(AEvent aEvent, ICanvas surface, Color color)
        {
            var icon = (aEvent) switch
            {
                (StartEvent startEvent) when startEvent.SubType.HasValue
                    => startEvent.SubType.Value switch
                    {
                        EventSubTypes.Message => BPMIcons.MessageStartEvent,
                        EventSubTypes.Conditional => BPMIcons.ConditionalStartEvent,
                        EventSubTypes.Signal => BPMIcons.SignalStartEvent,
                        EventSubTypes.Timer => BPMIcons.TimerStartEvent,
                        _ => BPMIcons.StartEvent
                    },
                (IntermediateThrowEvent intermediateThrowEvent)
                    => intermediateThrowEvent.SubType switch
                    {
                        EventSubTypes.Message => BPMIcons.MessageIntermediateThrowEvent,
                        EventSubTypes.Compensation => BPMIcons.CompensationIntermediateThrowEvent,
                        EventSubTypes.Escalation => BPMIcons.EscalationIntermediateThrowEvent,
                        EventSubTypes.Link => BPMIcons.LinkIntermediateThrowEvent,
                        EventSubTypes.Signal => BPMIcons.SignalIntermediateThrowEvent,
                        EventSubTypes.Timer => BPMIcons.TimerStartEvent,
                        _ => BPMIcons.IntermediateThrowEvent
                    },
                (IntermediateCatchEvent intermediateCatchEvent)
                    => intermediateCatchEvent.SubType switch
                    {
                        EventSubTypes.Conditional => BPMIcons.ConditionalIntermediateCatchEvent,
                        EventSubTypes.Link => BPMIcons.LinkIntermediateCatchEvent,
                        EventSubTypes.Message => BPMIcons.MessageIntermediateCatchEvent,
                        EventSubTypes.Signal => BPMIcons.SignalIntermediateCatchEvent,
                        EventSubTypes.Timer => BPMIcons.TimerIntermediateCatchEvent,
                        _ => BPMIcons.StartEvent
                    },
                (EndEvent endEvent)
                    => endEvent.SubType switch
                    {
                        EventSubTypes.Compensation => BPMIcons.CompensationEndEvent,
                        EventSubTypes.Escalation => BPMIcons.EscalationEndEvent,
                        EventSubTypes.Message => BPMIcons.MessageEndEvent,
                        EventSubTypes.Signal => BPMIcons.SignalEndEvent,
                        EventSubTypes.Error => BPMIcons.ErrorEndEvent,
                        EventSubTypes.Terminate => BPMIcons.TerminateEndEvent,
                        _ => BPMIcons.EndEvent
                    },
                (BoundaryEvent boundaryEvent)
                    => (boundaryEvent.SubType, boundaryEvent.CancelActivity) switch
                    {
                        (EventSubTypes.Message, true) => BPMIcons.InteruptingMessageBoundaryEvent,
                        (EventSubTypes.Message, false) => BPMIcons.NonInteruptingMessageBoundaryEvent,
                        (EventSubTypes.Conditional, true) => BPMIcons.InteruptingConditionalBoundaryEvent,
                        (EventSubTypes.Conditional, false) => BPMIcons.NonInteruptingConditionalBoundaryEvent,
                        (EventSubTypes.Escalation, true) => BPMIcons.InteruptingEscalationBoundaryEvent,
                        (EventSubTypes.Escalation, false) => BPMIcons.NonInteruptingEscalationBoundaryEvent,
                        (EventSubTypes.Signal, true) => BPMIcons.InteruptingSignalBoundaryEvent,
                        (EventSubTypes.Signal, false) => BPMIcons.NonInteruptingSignalBoundaryEvent,
                        (EventSubTypes.Timer, true) => BPMIcons.InteruptingTimerBoundaryEvent,
                        (EventSubTypes.Timer, false) => BPMIcons.NonInteruptingTimerBoundaryEvent,
                        (EventSubTypes.Error, _) => BPMIcons.InteruptingErrorBoundaryEvent,
                        (EventSubTypes.Compensation, _) => BPMIcons.InteruptingCompensationBoundaryEvent,
                        _ => BPMIcons.StartEvent
                    },
                _ => BPMIcons.StartEvent
            };
            IconGraphic.AppendIcon(this.Rectangle, icon, surface, color);
        }

        #endregion

        #region Gateway
        private void RenderGateway(Processes.Gateways.AGateway aGateway, ICanvas surface, Color color)
            => IconGraphic.AppendIcon(this.Rectangle, (BPMIcons)Enum.Parse(typeof(BPMIcons), aGateway.GetType().Name), surface, color);
        #endregion

        #region Lane
        private void RenderLane(IElement elem, ICanvas surface, Color color)
        {
            if (elem is not LaneSet)
            {
                surface.StrokeColor=color;
                surface.StrokeDashPattern=null;
                surface.StrokeSize = _PEN_SIZE;

                surface.DrawRoundedRectangle(this.Rectangle, _LANE_CORNER_RADIUS);
            }

            if (elem.ToString()!="")
            {
                surface.Rotate(-90, this.Rectangle.X, this.Rectangle.Y);
                surface.FontColor=color;
                surface.DrawString(elem.ToString(), new RectF(this.Rectangle.X-this.Rectangle.Height, this.Rectangle.Y+1, this.Rectangle.Height, this.Rectangle.Width), HorizontalAlignment.Center, VerticalAlignment.Top);
                surface.Rotate(90, this.Rectangle.X, this.Rectangle.Y);
            }
        }
        #endregion

        #region Task
        private void RenderTask(ATask aTask, ICanvas surface, Color color)
        {
            surface.StrokeColor = color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = aTask is CallActivity ? _ACTIVITY_PEN_SIZE : _PEN_SIZE;

            surface.DrawRoundedRectangle(this.Rectangle, _TASK_RADIUS);

            if (Enum.TryParse(typeof(BPMIcons), aTask.GetType().Name, out object obj))
                IconGraphic.AppendIcon(new Rect(this.Rectangle.X + 7, this.Rectangle.Y + 11, 15, 15), (BPMIcons)obj, surface, color);

            if (aTask is CallActivity)
            {
                surface.StrokeSize = _PEN_SIZE;
                var centerX = this.Rectangle.X+(this.Rectangle.Width/2);
                surface.DrawRectangle(centerX-(_CALL_ACTIVITY_SQUARE_SIZE/2), Rectangle.Y+Rectangle.Height-_CALL_ACTIVITY_SQUARE_PADDING-_CALL_ACTIVITY_SQUARE_SIZE, _CALL_ACTIVITY_SQUARE_SIZE, _CALL_ACTIVITY_SQUARE_SIZE);
                surface.DrawLine(centerX, Rectangle.Y+Rectangle.Height-_CALL_ACTIVITY_SQUARE_PADDING-_CALL_ACTIVITY_SQUARE_SIZE, centerX, Rectangle.Y+Rectangle.Height-_CALL_ACTIVITY_SQUARE_PADDING);
                surface.DrawLine(centerX-(_CALL_ACTIVITY_SQUARE_SIZE/2), Rectangle.Y+Rectangle.Height-_CALL_ACTIVITY_SQUARE_PADDING-(_CALL_ACTIVITY_SQUARE_SIZE/2), centerX+(_CALL_ACTIVITY_SQUARE_SIZE/2), Rectangle.Y+Rectangle.Height-_CALL_ACTIVITY_SQUARE_PADDING-(_CALL_ACTIVITY_SQUARE_SIZE/2));
            }
        }
        #endregion

        #region TextAnnotation
        private void RenderTextAnnotation(ICanvas surface, Color color)
        {
            surface.StrokeColor=color;
            surface.StrokeSize=_PEN_SIZE;
            surface.StrokeDashPattern=null;

            surface.DrawLine(new Point(this.Rectangle.X+20, this.Rectangle.Y),
                            new Point(this.Rectangle.X, this.Rectangle.Y));
            surface.DrawLine(new Point(this.Rectangle.X, this.Rectangle.Y),
                            new Point(this.Rectangle.X, this.Rectangle.Y+this.Rectangle.Height));
            surface.DrawLine(new Point(this.Rectangle.X, this.Rectangle.Y+this.Rectangle.Height),
                            new Point(this.Rectangle.X+20, this.Rectangle.Y+this.Rectangle.Height));
        }
        #endregion

        #region SubProcess
        private void RenderSubProcess(ICanvas surface, Color color)
        {
            surface.StrokeColor=color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = _PEN_SIZE;
            surface.DrawRoundedRectangle(this.Rectangle, _SUB_PROCESS_CORNER_RADIUS);
        }
        #endregion
    }
}
