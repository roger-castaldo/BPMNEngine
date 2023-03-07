using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Elements;
using Org.Reddragonit.BpmEngine.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal interface IRenderingElement
    {
        void Render(ICanvas surface, ProcessPath path, Definition definition);
    }
}
