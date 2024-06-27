using BPMNEngine.Elements;
using BPMNEngine.State;
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing
{
    internal interface IRenderingElement
    {
        void Render(ICanvas surface, ProcessPath path, Definition definition);
    }
}
