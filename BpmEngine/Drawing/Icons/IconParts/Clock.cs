
using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal class Clock : IIconPart
    {
        private const float _PEN_SIZE = 2.0f;
        private const float _TICK_SIZE = 1.0f;

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

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            Rect rect = new Rect(8f,8f,30f,30f);
            Point c = new Point(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

            surface.StrokeColor = color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = _TICK_SIZE;

            float rad = (float)rect.Width / 2f;
            foreach (sAngleFactorPair angle in _ANGLES)
            {
                surface.DrawLine(
                    new Point(c.X + (rad * angle.X), c.Y + (rad * angle.Y)),
                    new Point(c.X + ((rad-3) * angle.X), c.Y + ((rad - 3) * angle.Y))
                );
            }

            surface.StrokeSize = _PEN_SIZE;
            surface.DrawEllipse(rect);

            surface.DrawLine(
                new Point(c.X, c.Y),
                new Point(c.X + ((rad - 2) * _MINUTE_HAND.X), c.Y + ((rad - 2) * _MINUTE_HAND.Y))
            );
            surface.DrawLine(
                new Point(c.X, c.Y), 
                new Point(c.X +((rad - 5) * _HOUR_HAND.X), c.Y +((rad - 5) * _HOUR_HAND.Y))
            );
        }
    }
}
