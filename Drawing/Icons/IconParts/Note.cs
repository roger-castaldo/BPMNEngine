using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Note : IIconPart
    {
        private const float _START_X = 16f;
        private const float _END_X = 29f;
        private const float _Y_SHIFT = 3.8f;

        public void Add(Graphics gp, int iconSize, Color color)
        {
            Pen p = new Pen(color, 1f);
            gp.DrawRectangle(p, 13f, 11f, 19f, 23f);
            float y = 11f + _Y_SHIFT;
            for(int x = 0; x < 5; x++)
            {
                gp.DrawLine(p,new PointF(_START_X,y),new PointF(_END_X,y));
                y += _Y_SHIFT;
            }
        }
    }
}
