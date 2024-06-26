using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Triangle(bool filled) : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = [
            new(24f,9f),
            new(34f,30f),
            new(13f,30f),
            new(24f,9f)
        ];

        private static readonly PathF _PATH;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static Triangle()
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
