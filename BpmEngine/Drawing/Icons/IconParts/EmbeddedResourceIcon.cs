
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal abstract class EmbeddedResourceIcon
        : IIconPart
    {
        protected abstract string _resourceName { get; }

        public void Add(Image gp,int iconSize, Color color)
        {
            Image img = Image.FromStream(GetType().GetTypeInfo().Assembly.GetManifestResourceStream(string.Format("Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts.resources.{0}",_resourceName)));
            if (color!=Image.Black)
            {
                Image g = new Image(img.Size);
                g.DrawImage(img,new Rect(0,0,img.Size.Width,img.Size.Height));
                for (int x = 0; x<g.Size.Width; x++)
                {
                    for (int y = 0; y<g.Size.Height; y++)
                    {
                        Color c = g.GetPixel(x, y);
                        g.SetPixel(x, y, new Color(
                            Math.Min(255, c.Red+color.Red),
                            Math.Min(255, c.Green+color.Green),
                            Math.Min(255, c.Blue+color.Blue),
                            c.Alpha
                        ));
                    }
                }
                img=g;
            }
            gp.DrawImage(img, new Rect(0, 0, iconSize,iconSize));
        }
    }
}
