using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal interface IIconPart
    {
        void Add(Image gp,int iconSize,Color color);
    }
}
