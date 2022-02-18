using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts
{
    internal abstract class ABase64Icon : IIconPart
    {
        protected abstract string _imageData { get; }

        public void Add(Graphics gp,int iconSize, Color color)
        {
            Image img = Image.FromStream(new MemoryStream(Convert.FromBase64String(_imageData)));
            if (color!=Color.Black)
            {
                Bitmap bmp = new Bitmap(img.Width, img.Height);
                Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(img,new Rectangle(0,0,img.Width,img.Height));
                g.Flush();
                for (int x = 0; x<bmp.Width; x++)
                {
                    for (int y = 0; y<bmp.Height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        bmp.SetPixel(x, y, Color.FromArgb(
                            c.A,
                            Math.Min(255, c.R+color.R),
                            Math.Min(255, c.G+color.G),
                            Math.Min(255, c.B+color.B)
                        ));
                    }
                }
                img=bmp;
            }
            gp.DrawImage(img, new Rectangle(0, 0, iconSize,iconSize));
        }
    }
}
