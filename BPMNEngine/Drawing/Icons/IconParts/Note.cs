using Microsoft.Maui.Graphics;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal class Note : IIconPart
    {
        private const float _PEN_SIZE = 1.0f;
        private const float _START_X = 16f;
        private const float _END_X = 29f;
        private const float _Y_SHIFT = 3.8f;

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            surface.StrokeColor=color;
            surface.StrokeDashPattern=null;
            surface.StrokeSize=_PEN_SIZE;

            surface.DrawRectangle(new Rect(13f, 11f, 19f, 23f));
            float y = 11f + _Y_SHIFT;
            for(int x = 0; x < 5; x++)
            {
                surface.DrawLine(new Point(_START_X,y),new Point(_END_X,y));
                y += _Y_SHIFT;
            }
        }
    }
}
