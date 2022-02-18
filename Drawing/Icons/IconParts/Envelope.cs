using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Envelope : IIconPart
    {
        private bool _filled;
        private bool _fullSize;

        public Envelope(bool filled,bool fullSize)
        {
            _filled = filled;
            _fullSize = fullSize;
        }

        public void Add(Graphics gp, int iconSize, Color color)
        {
            RectangleF rect = (_fullSize ? new RectangleF(0,5,AIcon.IMAGE_SIZE-1,30) : new RectangleF(10f,14f,25f,17f));
            if (_filled)
                gp.FillRectangle(new SolidBrush(color), rect);
            else
                gp.DrawRectangle(new Pen(color, (_fullSize ? 2f : 1f)),rect.X,rect.Y,rect.Width,rect.Height);
            gp.DrawLines(new Pen((_filled ? Color.White : color), 2f), new PointF[]
            {
                new PointF(rect.X, rect.Y),
                new PointF(rect.X+(rect.Width/2f),rect.Y+(rect.Height/3f)),
                new PointF(rect.X+rect.Width,rect.Y)
            });
        }
    }
}
