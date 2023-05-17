using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using BpmEngine.Drawing.Icons.IconParts;
using BpmEngine.Elements;
using System.Collections.Generic;

namespace BpmEngine.Drawing.Icons
{
    internal abstract class AIcon
    {
        public const int IMAGE_SIZE = 46;

        private Dictionary<Microsoft.Maui.Graphics.Color, SkiaBitmapExportContext> _cache;

        protected virtual int _ImageSize { get { return IMAGE_SIZE; } }

        protected virtual IIconPart[] _parts { get { return new IIconPart[0]; } }

        protected virtual void _Draw(ICanvas surface, Color color)
        {
            _parts.ForEach(part => { part.Add(surface, _ImageSize, color); });
        }

        public AIcon()
        {
            _cache= new Dictionary<Color, SkiaBitmapExportContext>();
        }

        public void Draw(RectF container, ICanvas surface, Color color)
        {
            lock (_cache)
            {
                if (!_cache.ContainsKey(color))
                {
                    var image  = Diagram.ProduceImage(_ImageSize, _ImageSize);
                    var canvas = image.Canvas;
                    canvas.FillColor = Colors.White;
                    canvas.FillRectangle(0,0,_ImageSize, _ImageSize);
                    _Draw(canvas, color);
                    _cache.Add(color, image);
                }
                surface.DrawImage(_cache[color].Image, container.X,container.Y,container.Width,container.Height);
            }
        }
    }
}
