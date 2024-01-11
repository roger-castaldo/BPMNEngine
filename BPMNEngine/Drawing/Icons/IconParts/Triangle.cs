using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Triangle : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = new Point[] {
            new(24f,9f),
            new(34f,30f),
            new(13f,30f),
            new(24f,9f)
        };

        private static readonly PathF _PATH;

        static Triangle()
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p => _PATH.LineTo(p));
            _PATH.Close();
        }

        private readonly bool _filled;

        public Triangle(bool filled)
        {
            _filled = filled;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            if (_filled)
            {
                surface.FillColor=color;
                surface.FillPath(_PATH);
            }
            else
            {
                surface.StrokeColor=color;
                surface.StrokeSize = _PEN_SIZE;
                surface.StrokeDashPattern=null;
                surface.DrawPath(_PATH);
            }
        }
    }
}
