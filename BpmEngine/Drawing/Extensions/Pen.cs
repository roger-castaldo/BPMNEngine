using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Org.Reddragonit.BpmEngine.Drawing.Extensions
{
    internal class Pen
    {
        private readonly Color _color;
        public Color Color => _color;
        private readonly float _size;
        public float Size => _size;

        private readonly float[] _dashPattern;
        public float[] DashPattern => _dashPattern;

        public Pen(Color color, float size, float[] dashPattern)
        {
            _color= color;
            _size= size;
            _dashPattern= dashPattern;
        }

        public Pen(Color color, float size)
            : this(color, size, null) { }

    }
}
