using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class OuterCircle : IIconPart
    {
        private readonly bool _dashed;
        private const float _PEN_SIZE = 2f;

        public OuterCircle()
            : this(false) { }
        public OuterCircle(bool dashed)
        {
            _dashed=dashed;
        }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            surface.StrokeColor = color;
            surface.StrokeSize = _PEN_SIZE;
            surface.StrokeDashPattern = _dashed ? Constants.DASH_PATTERN : null;
            surface.DrawEllipse(new Rect(0,0,AIcon.IMAGE_SIZE-1,AIcon.IMAGE_SIZE-1));
        }
    }
}
