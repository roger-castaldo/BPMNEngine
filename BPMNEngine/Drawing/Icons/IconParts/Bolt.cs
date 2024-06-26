
using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Bolt(bool filled) : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly PointF[] _POINTS =
        [
            new(11f,33f),
            new(18f,12f),
            new(27f,23f),
            new(34f,11f),
            new(28f,32f),
            new(19f,22f),
            new(11f,33f)
        ];

        private static readonly PathF _PATH;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static Bolt()
#pragma warning restore S3963 // "static" fields should be initialized inline
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p=>_PATH.LineTo(p));
            _PATH.Close();
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            if (filled)
            {
                surface.FillColor= color;
                surface.FillPath(_PATH);
            }
            else
            {
                surface.StrokeColor=color;
                surface.StrokeDashPattern=null;
                surface.StrokeSize = _PEN_SIZE;
                surface.DrawPath(_PATH);
            }
        }
    }
}
