using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class RightArrow : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;

        private static readonly Point[] _POINTS = new Point[] {
            new Point(11F,18f),
            new Point(26F,18f),
            new Point(26F,11f),
            new Point(39F,23f),
            new Point(26F,35f),
            new Point(26F,29f),
            new Point(11F,29f),
            new Point(11F,18f)
        };

        private static readonly PathF _PATH;

        static RightArrow()
        {
            _PATH = new PathF(_POINTS[0]);
            _POINTS.Skip(1).ForEach(p => _PATH.LineTo(p));
            _PATH.Close();
        }

        private readonly bool _filled;
        public RightArrow(bool filled)
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
