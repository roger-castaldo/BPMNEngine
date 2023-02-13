using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Point : IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.PointF";
        public const string SKIA_TYPE = "SkiaSharp.SKPoint";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);
        
        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(new Type[] { typeof(float), typeof(float) }));
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(new Type[] { typeof(float), typeof(float) }));

        private readonly float _x;
        public float X => _x;
        private readonly float _y;
        public float Y => _y;

        public Point(float x,float y)
        {
            _x=x;
            _y=y;
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _x, _y }) : null);
            }
        }

        public object SkiaObject { get { return (_skiaConstructor!=null ? _skiaConstructor.Invoke(new object[] { _x, _y }) : null); } }

        public override bool Equals(object obj)
        {
            if (obj is Point p)
            {
                return _x==p.X&&_y==p.Y;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return string.Format("{0},{1}", _x, _y).GetHashCode();
        }
    }
}
