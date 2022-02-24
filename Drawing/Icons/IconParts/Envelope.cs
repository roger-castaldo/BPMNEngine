using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
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

        public void Add(Image gp, int iconSize, Color color)
        {
            Rectangle rect = (_fullSize ? new Rectangle(0,5,AIcon.IMAGE_SIZE-1,30) : new Rectangle(10f,14f,25f,17f));
            if (_filled)
                gp.FillRectangle(new SolidBrush(color), rect);
            else
                gp.DrawRectangle(new Pen(color, (_fullSize ? 2f : 1f)),rect);
            gp.DrawLines(new Pen((_filled ? Color.White : color), 2f), new Point[]
            {
                new Point(rect.X, rect.Y),
                new Point(rect.X+(rect.Width/2f),rect.Y+(rect.Height/3f)),
                new Point(rect.X+rect.Width,rect.Y)
            });
        }
    }
}
