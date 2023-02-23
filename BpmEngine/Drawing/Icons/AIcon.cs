using Microsoft.Maui.Graphics;
using Org.Reddragonit.BpmEngine.Drawing.Icons.IconParts;

using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Icons
{
    internal abstract class AIcon
    {
        public const int IMAGE_SIZE = 46;

        private Dictionary<Color, Image> _cache;

        protected virtual int _ImageSize { get { return IMAGE_SIZE; } }

        protected virtual IIconPart[] _parts { get { return new IIconPart[0]; } }

        protected virtual void _Draw(Image gp, Color color)
        {
            foreach (IIconPart part in _parts)
                part.Add(gp,_ImageSize, color);
        }

        public AIcon()
        {
            _cache= new Dictionary<Color, Image>();
        }

        public void Draw(Rect container, Image gp, Color color)
        {
            lock (_cache)
            {
                if (!_cache.ContainsKey(color))
                {
                    Image g = new Image(_ImageSize, _ImageSize);
                    _Draw(g, color);
                    _cache.Add(color, g);
                }
                gp.DrawImage(_cache[color], container);
            }
        }
    }
}
