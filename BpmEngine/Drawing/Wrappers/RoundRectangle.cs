using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class RoundRectangle : IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.Drawing2D.GraphicsPath";
        public const string SKIA_TYPE = "SkiaSharp.SKRoundRect";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(Type.EmptyTypes));
        private static readonly MethodInfo _drawingAddArc = (_DrawingType==null ? null : _DrawingType.GetMethod("AddArc", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) }));
        private static readonly MethodInfo _drawingAddLine = (_DrawingType==null ? null : _DrawingType.GetMethod("AddLine", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));
        private static readonly MethodInfo _drawingClose = (_DrawingType==null ? null : _DrawingType.GetMethod("CloseFigure", Type.EmptyTypes));

        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(new Type[] {SkiaImage.LocateType(Rectangle.SKIA_TYPE),typeof(float) }));

        private float _x;
        public float X { get { return _x; } }
        private float _y;
        public float Y { get { return _y; } }
        private float _width;
        public float Width { get { return _width; } }
        private float _height;
        public float Height { get { return _height; } }
        private float _radius;
        public float Radius { get { return _radius; } }

        public RoundRectangle(float x,float y,float width,float height,float radius)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _radius = radius;
        }

        public RoundRectangle(Rectangle rect,float radius) 
            : this(rect.X,rect.Y,rect.Width,rect.Height,radius)
        { }

        public object DrawingObject
        {
            get
            {
                object ret = null;
                if (_drawingConstructor != null)
                {
                    ret = _drawingConstructor.Invoke(new object[] { });
                    _drawingAddLine.Invoke(ret,new object[] { _x+_radius, _y, _x+ _width- (_radius * 2f), _y });
                    _drawingAddArc.Invoke(ret,new object[] { _x+ _width- (_radius * 2f), _y, _radius * 2f, _radius * 2f, 270f, 90f });
                    _drawingAddLine.Invoke(ret, new object[] { _x+ _width, _y+ _radius, _x+ _width, _y+ _height- (_radius * 2f) });
                    _drawingAddArc.Invoke(ret, new object[] { _x+ _width- (_radius * 2f), _y+ _height- (_radius * 2f), _radius * 2, _radius * 2f, 0f, 90f });
                    _drawingAddLine.Invoke(ret, new object[] { _x+ _width- (_radius * 2f), _y+ _height, _x+ _radius, _y+ _height });
                    _drawingAddArc.Invoke(ret, new object[] { _x, _y+ _height- (_radius * 2f), _radius * 2f, _radius * 2f, 90f, 90f });
                    _drawingAddLine.Invoke(ret, new object[] { _x, _y+ _height- (_radius * 2f), _x, _y+ _radius });
                    _drawingAddArc.Invoke(ret, new object[] { _x, _y, _radius * 2f, _radius * 2f, 180f, 90f });
                    _drawingClose.Invoke(ret,new object[] { });
                }
                return ret;
            }
        }

        public object SkiaObject
        {
            get
            {
                return (_skiaConstructor==null ? null : _skiaConstructor.Invoke(new object[] { new Rectangle(_x, _y, _width, _height).SkiaObject, _radius }));
            }
        }
    }
}
