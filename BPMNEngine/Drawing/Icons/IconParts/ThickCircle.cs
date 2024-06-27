using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class ThickCircle : IIconPart
    {
        private const float _PEN_SIZE = 4.0f;

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            surface.StrokeColor= color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize= _PEN_SIZE;

            surface.DrawEllipse(new Rect(2, 2, AIcon.IMAGE_SIZE-5, AIcon.IMAGE_SIZE-5));
        }
    }
}
