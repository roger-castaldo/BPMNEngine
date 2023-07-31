using Microsoft.Maui.Graphics;
using BPMNEngine.Elements;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    internal abstract class AGateway : AIcon
    {
        private const float _PEN_SIZE = 2.0f;
        public new const int IMAGE_SIZE = 63;

        private static readonly Point[] _POINTS = new Point[] {
            new Point((float)AGateway.IMAGE_SIZE/2f,0),
            new Point((float)AGateway.IMAGE_SIZE-2,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,(float)AGateway.IMAGE_SIZE-2),
            new Point(0,(float)AGateway.IMAGE_SIZE/2f),
            new Point((float)AGateway.IMAGE_SIZE/2f,0)
        };

        protected override sealed int ImageSize
        {
            get { return IMAGE_SIZE; }
        }

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
