﻿
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal interface IIconPart
    {
        void Add(ICanvas surface, int iconSize,Color color);
    }
}