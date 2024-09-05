using BPMNEngine.Elements;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    internal abstract class AGateway : AIcon
    {
        private const float _PEN_SIZE = 2.0f;
        public new const float IMAGE_SIZE = 63;

        private static readonly Point[] _POINTS = [
            new(AGateway.IMAGE_SIZE/2f,0),
            new(AGateway.IMAGE_SIZE-2,AGateway.IMAGE_SIZE/2f),
            new(AGateway.IMAGE_SIZE/2f,AGateway.IMAGE_SIZE-2),
            new(0,AGateway.IMAGE_SIZE/2f),
            new(AGateway.IMAGE_SIZE/2f,0)
        ];

        protected override sealed int ImageSize
            => (int)IMAGE_SIZE;

        protected override void InternalDraw(ICanvas surface, Color color)
        {
            base.InternalDraw(surface, color);

            surface.StrokeColor=color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = _PEN_SIZE;

            Diagram.DrawLines(surface, _POINTS);

            surface.DrawLine(_POINTS[^1], _POINTS[0]);
        }
    }
}
