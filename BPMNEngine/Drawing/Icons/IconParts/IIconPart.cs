
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal interface IIconPart
    {
        void Add(ICanvas surface, int iconSize, Color color);
    }
}
