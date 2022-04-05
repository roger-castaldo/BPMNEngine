using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Rectangle :IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.RectangleF";
        public const string SKIA_TYPE = "SkiaSharp.SKRect";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);

        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));

        private float _x;
        public float X { get { return _x; } }
        private float _y;
        public float Y { get { return _y; } }
        private float _width;   
        public float Width { get { return _width; } }
        private float _height;  
        public float Height { get { return _height; } }

        public Rectangle(float x,float y,float width,float height)
        {
            _x = x;
            _y = y; 
            _width = width;
            _height = height;
        }

        public Rectangle(Point p1,Point p2)
        {
            _x = Math.Min(p1.X, p2.X);
            _y = Math.Min(p1.Y, p2.Y);
            _width = Math.Abs(p1.X-p2.X);
            _height = Math.Abs(p1.Y-p2.Y);
        }

        public Rectangle Merge(Rectangle additional)
        {
            if (additional==null)
                return this;
            float minX = Math.Min(_x, additional.X);
            float minY = Math.Min(_y, additional.Y);
            float maxX = Math.Max(_x+_width, additional.X+additional.Width);
            float maxY = Math.Max(_y+_height, additional.Y+additional.Height);
            return new Rectangle(minX, minY, Math.Abs(maxX-minX), Math.Abs(maxY-minY));
        }

        public Rectangle(object drawingObject)
        {
            _height = (float)drawingObject.GetType().GetProperty("Height").GetValue(drawingObject, new object[] { });
            _width = (float)drawingObject.GetType().GetProperty("Width").GetValue(drawingObject, new object[] { });
            if (drawingObject.GetType().FullName==_DrawingType.FullName)
            {
                _x = (float)drawingObject.GetType().GetProperty("X").GetValue(drawingObject, new object[] { });
                _y = (float)drawingObject.GetType().GetProperty("Y").GetValue(drawingObject, new object[] { });
            }else if (drawingObject.GetType().FullName==_SkiaType.FullName)
            {
                _x = (float)drawingObject.GetType().GetProperty("Left").GetValue(drawingObject, new object[] { });
                _y = (float)drawingObject.GetType().GetProperty("Top").GetValue(drawingObject, new object[] { });
            }
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _x, _y, _width, _height }) : null);
            }
        }

        public object SkiaObject
        {
            get { return (_skiaConstructor!=null ? _skiaConstructor.Invoke(new object[] { _x, _y, _x+_width, _y+_height }) : null); }
        }
    }
}
