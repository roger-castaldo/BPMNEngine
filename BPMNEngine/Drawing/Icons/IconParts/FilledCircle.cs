
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class FilledCircle : IIconPart
    {
        public void Add(ICanvas surface, int iconSize, Color color)
        {
            surface.FillColor= color;
            surface.FillEllipse(new Rect(10, 10, AIcon.IMAGE_SIZE-20, AIcon.IMAGE_SIZE-20));
        }
    }
}
