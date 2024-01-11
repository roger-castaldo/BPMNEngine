using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.Gateways
{
    [IconTypeAttribute(Elements.Diagrams.BPMIcons.InclusiveGateway)]
    internal class InclusiveGateway : AGateway
    {
        protected override void InternalDraw(ICanvas surface, Color color)
        {
            base.InternalDraw(surface, color);

            surface.DrawEllipse(new Rect(16, 16, 30, 30));
        }
    }
}
