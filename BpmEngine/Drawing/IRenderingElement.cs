using Microsoft.Maui.Graphics;
using BPMNEngine.Elements;
using BPMNEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMNEngine.Drawing
{
    internal interface IRenderingElement
    {
        void Render(ICanvas surface, ProcessPath path, Definition definition);
    }
}
