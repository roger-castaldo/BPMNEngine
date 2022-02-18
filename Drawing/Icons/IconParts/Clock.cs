using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Clock : IIconPart
    {
        private struct sAngleFactorPair
        {
            private float _x;
            public float X { get { return _x; } }
            private float _y;
            public float Y { get { return _y; } }

            public sAngleFactorPair(double degrees)
            {
                degrees= degrees * Math.PI / 180;
                _x = (float)Math.Cos(degrees);
                _y = (float)Math.Sin(degrees);
            }
        }

        private static readonly sAngleFactorPair[] _ANGLES = new sAngleFactorPair[]
        {
            new sAngleFactorPair(30),
            new sAngleFactorPair(60),
            new sAngleFactorPair(90),
            new sAngleFactorPair(120),
            new sAngleFactorPair(150),
            new sAngleFactorPair(180),
            new sAngleFactorPair(210),
            new sAngleFactorPair(240),
            new sAngleFactorPair(270),
            new sAngleFactorPair(300),
            new sAngleFactorPair(330),
            new sAngleFactorPair(360)
        };

        private static readonly sAngleFactorPair _HOUR_HAND = new sAngleFactorPair(15);
        private static readonly sAngleFactorPair _MINUTE_HAND = new sAngleFactorPair(290);

        public void Add(Graphics gp, int iconSize, Color color)
        {
            Pen p = new Pen(color, 1f);
            RectangleF rect = new RectangleF(8f,8f,30f,30f);
            PointF c = new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
            gp.DrawEllipse(p, rect);
            float rad = rect.Width / 2f;
            foreach (sAngleFactorPair angle in _ANGLES)
            {
                gp.DrawLine(p,
                    new PointF(c.X + (rad * angle.X), c.Y + (rad * angle.Y)),
                    new PointF(c.X + ((rad-3) * angle.X), c.Y + ((rad - 3) * angle.Y))
                );
            }
            gp.DrawLine(p,
                new PointF(c.X, c.Y),
                new PointF(c.X + ((rad - 2) * _MINUTE_HAND.X), c.Y + ((rad - 2) * _MINUTE_HAND.Y))
            );
            gp.DrawLine(p,
                new PointF(c.X, c.Y),
                new PointF(c.X + ((rad - 5) * _HOUR_HAND.X), c.Y + ((rad - 5) * _HOUR_HAND.Y))
            );
        }
    }
}
