
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Clock : IIconPart
    {
        private const float _PEN_SIZE = 2.0f;
        private const float _TICK_SIZE = 1.0f;

        private readonly struct SAngleFactorPair
        {
            public float X { get; private init; }
            public float Y { get; private init; }

            public SAngleFactorPair(double degrees)
            {
                degrees= degrees * Math.PI / 180;
                X = (float)Math.Cos(degrees);
                Y = (float)Math.Sin(degrees);
            }
        }

        private static readonly SAngleFactorPair[] _ANGLES = new SAngleFactorPair[]
        {
            new(30),
            new(60),
            new(90),
            new(120),
            new(150),
            new(180),
            new(210),
            new(240),
            new(270),
            new(300),
            new(330),
            new(360)
        };

        private static readonly SAngleFactorPair _HOUR_HAND = new(15);
        private static readonly SAngleFactorPair _MINUTE_HAND = new(290);

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            Rect rect = new(8f,8f,30f,30f);
            Point c = new(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));

            surface.StrokeColor = color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize = _TICK_SIZE;

            float rad = (float)rect.Width / 2f;
            _ANGLES.ForEach(angle => {
                surface.DrawLine(
                    new Point(c.X + (rad * angle.X), c.Y + (rad * angle.Y)),
                    new Point(c.X + ((rad-3) * angle.X), c.Y + ((rad - 3) * angle.Y))
                );
            });

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
