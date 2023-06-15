
using Microsoft.Maui.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class InnerCircle : IIconPart
    {
        private readonly bool _dashed;
        private const float _PEN_SIZE = 2f;

        public InnerCircle()
            : this(false) { }
        public InnerCircle(bool dashed)
        {
            _dashed=dashed;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            surface.StrokeColor = color;
            surface.StrokeSize = _PEN_SIZE;
            surface.StrokeDashPattern = _dashed ? Constants.DASH_PATTERN : null;
            surface.DrawEllipse(new Rect(4, 4, AIcon.IMAGE_SIZE - 8, AIcon.IMAGE_SIZE- 8));
        }
    }
}
