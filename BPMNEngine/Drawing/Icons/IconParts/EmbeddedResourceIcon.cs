
using BPMNEngine.Elements;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Skia;
using SkiaSharp;
using System.Reflection;

namespace BPMNEngine.Drawing.Icons.IconParts
{
    internal abstract class EmbeddedResourceIcon
        : IIconPart
    {
        protected abstract string ResourceName { get; }

        public void Add(ICanvas surface, int iconSize, Color color)
        {
            var icon = SKImage.FromEncodedData(GetType().GetTypeInfo().Assembly.GetManifestResourceStream(string.Format("BPMNEngine.Drawing.Icons.IconParts.resources.{0}", ResourceName)));
            var bmp = SKBitmap.FromImage(icon);
            var image = Diagram.ProduceImage(icon.Width, icon.Height);
            for (int x = 0; x<bmp.Width; x++)
            {
                for (int y = 0; y<bmp.Height; y++)
                {
                    var c = bmp.GetPixel(x, y);
                    if (c.AsColor()!=Colors.White)
                    {
                        image.Bitmap.SetPixel(x, y, new Color(
                            Math.Min(c.Red+color.Red, byte.MaxValue),
                            Math.Min(c.Green+color.Green, byte.MaxValue),
                            Math.Min(c.Blue+color.Blue, byte.MaxValue),
                            c.Alpha
                        ).AsSKColor());
                    }
                }
            }
            surface.DrawImage(image.Image, 0, 0, iconSize, iconSize);
        }
    }
}
