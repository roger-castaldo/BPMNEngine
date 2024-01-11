using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using BPMNEngine.Drawing.Icons.IconParts;
using BPMNEngine.Elements;

namespace BPMNEngine.Drawing.Icons
{
    internal abstract class AIcon
    {
        public const int IMAGE_SIZE = 46;

        private readonly Dictionary<Microsoft.Maui.Graphics.Color, SkiaBitmapExportContext> cache = new();
        protected virtual int ImageSize => IMAGE_SIZE;
        protected virtual IIconPart[] Parts => Array.Empty<IIconPart>();
        protected virtual void InternalDraw(ICanvas surface, Color color)
        {
            Parts.ForEach(part => { part.Add(surface, ImageSize, color); });
        }
        public void Draw(RectF container, ICanvas surface, Color color)
        {
            lock (cache)
            {
                if (!cache.ContainsKey(color))
                {
                    var image  = Diagram.ProduceImage(ImageSize, ImageSize);
                    var canvas = image.Canvas;
                    canvas.FillColor = Colors.White;
                    canvas.FillRectangle(0,0,ImageSize, ImageSize);
                    InternalDraw(canvas, color);
                    cache.Add(color, image);
                }
                surface.DrawImage(cache[color].Image, container.X,container.Y,container.Width,container.Height);
            }
        }
    }
}
