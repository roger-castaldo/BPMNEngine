
using Microsoft.Maui.Graphics;

using System;
using System.Collections.Generic;
using System.Text;

namespace BpmEngine.Drawing.Icons.IconParts
{
    internal class Envelope : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private readonly bool _filled;
        private readonly bool _fullSize;

        public Envelope(bool filled,bool fullSize)
        {
            _filled = filled;
            _fullSize = fullSize;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            Rect rect = (_fullSize ? new Rect(0,5,AIcon.IMAGE_SIZE-1,30) : new Rect(10f,14f,25f,17f));

            surface.StrokeColor=color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize=_PEN_SIZE;

            if (_filled)
            {
                surface.FillColor=color;
                surface.FillRectangle(rect);
            }
            else
                surface.DrawRectangle(rect);

            if (_filled)
                surface.StrokeColor=Colors.White;

            surface.DrawLine(new Point(rect.X, rect.Y), new Point(rect.X+(rect.Width/2f), rect.Y+(rect.Height/3f)));
            surface.DrawLine(new Point(rect.X+(rect.Width/2f), rect.Y+(rect.Height/3f)), new Point(rect.X+rect.Width, rect.Y));
        }
    }
}
