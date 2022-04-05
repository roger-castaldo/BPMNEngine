using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Size : IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.Size";
        public const string SKIA_TYPE = "SkiaSharp.SKSize";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);
        
        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(new Type[] { typeof(int), typeof(int) }));
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(new Type[] { typeof(float), typeof(float) }));

        private static readonly ConstructorInfo _floatConstructor = (_DrawingType==null ? null : DrawingImage.LocateType("System.Drawing.SizeF").GetConstructor(new Type[] { typeof(float), typeof(float) }));

        private int _width;
        public int Width { get { return _width; } }
        private int _height;
        public int Height { get { return _height; } }

        public Size(int width,int height)
        {
            _width=width;
            _height=height;
        }

        public Size(float width,float height)
        {
            _width=(int)Math.Ceiling(width);
            _height=(int)Math.Ceiling(height);
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] {_width,_height}) : null);  
            }
        }

        public object FloatDrawingObject
        {
            get
            {
                return (_floatConstructor!=null ? _floatConstructor.Invoke(new object[] { (float)_width, (float)_height }) : null);
            }
        }

        public object SkiaObject { get { return (_skiaConstructor!=null ? _skiaConstructor.Invoke(new object[] { (float)_width, (float)_height }) : null); } }
    }
}
