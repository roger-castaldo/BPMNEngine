using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Rewind(bool filled) : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly PointF[] _POINTS = [
            new(9f,23f),
            new(23f,15f),
            new(23f,23f),
            new(37f,15f),
            new(37f,31f),
            new(23f,23f),
            new(23f,31f),
            new(9f,23f)
        ];

        private static readonly PathF _PATH;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static Rewind()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p => _PATH.LineTo(p));
            _PATH.Close();
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            if (filled)
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
