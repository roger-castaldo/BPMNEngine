using Org.Reddragonit.BpmEngine.Drawing.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal abstract class ABase64Icon : IIconPart
    {
        protected abstract string _imageData { get; }

        public void Add(Image gp,int iconSize, Color color)
        {
            Image img = Image.FromStream(new MemoryStream(Convert.FromBase64String(_imageData)));
            if (color!=Color.Black)
            {
                Image g = new Image(img.Size);
                g.DrawImage(img,new Rectangle(0,0,img.Size.Width,img.Size.Height));
                g.Flush();
                for (int x = 0; x<g.Size.Width; x++)
                {
                    for (int y = 0; y<g.Size.Height; y++)
                    {
                        Color c = g.GetPixel(x, y);
                        g.SetPixel(x, y, Color.FromArgb(
                            c.A,
                            Math.Min(255, c.R+color.R),
                            Math.Min(255, c.G+color.G),
                            Math.Min(255, c.B+color.B)
                        ));
                    }
                }
                img=g;
            }
            gp.DrawImage(img, new Rectangle(0, 0, iconSize,iconSize));
        }
    }
}
