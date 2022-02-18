using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal interface IIconPart
    {
        void Add(Graphics gp,int iconSize,Color color);
    }
}
