using Microsoft.Maui.Graphics;
using BPMNEngine.Elements;
using BPMNEngine.State;

namespace BPMNEngine.Drawing
{
    internal interface IRenderingElement
    {
        void Render(ICanvas surface, ProcessPath path, Definition definition);
    }
}
