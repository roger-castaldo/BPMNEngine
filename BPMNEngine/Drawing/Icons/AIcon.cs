using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using BPMNEngine.Drawing.Icons.IconParts;
using BPMNEngine.Elements;

namespace BPMNEngine.Drawing.Icons
{
    internal abstract class AIcon
    {
        public const int IMAGE_SIZE = 46;

        private readonly Dictionary<Microsoft.Maui.Graphics.Color, SkiaBitmapExportContext> cache = [];
        protected virtual int ImageSize => IMAGE_SIZE;
        protected virtual IIconPart[] Parts => [];
        protected virtual void InternalDraw(ICanvas surface, Color color)
            => Parts.ForEach(part => { part.Add(surface, ImageSize, color); });
        public void Draw(RectF container, ICanvas surface, Color color)
        {
            lock (cache)
            {
                if (!cache.TryGetValue(color, out SkiaBitmapExportContext value))
                {
                    var image  = Diagram.ProduceImage(ImageSize, ImageSize);
                    var canvas = image.Canvas;
                    canvas.FillColor = Colors.White;
                    canvas.FillRectangle(0,0,ImageSize, ImageSize);
                    InternalDraw(canvas, color);
                    value=image;
                    cache.Add(color, value);
                }
                surface.DrawImage(value.Image, container.X,container.Y,container.Width,container.Height);
            }
        }
    }
}
