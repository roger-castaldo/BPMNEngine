using Microsoft.Maui.Graphics;
using BpmEngine.Elements;
using BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpmEngine.Drawing
{
    internal interface IRenderingElement
    {
        void Render(ICanvas surface, ProcessPath path, Definition definition);
    }
}
