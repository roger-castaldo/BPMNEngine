using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Pen : IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.Pen";
        public const string SKIA_TYPE = "SkiaSharp.SKPaint";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);

        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(new Type[] { DrawingImage.LocateType("System.Drawing.Brush"), typeof(float) }));
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(Type.EmptyTypes));

        private static readonly object _skiaStyle = (SkiaImage.LocateType("SkiaSharp.SKPaintStyle")==null ? null : Enum.Parse(SkiaImage.LocateType("SkiaSharp.SKPaintStyle"), "Stroke"));

        private static readonly MethodInfo _skiaCreateDash = (SkiaImage.LocateType("SkiaSharp.SKPathEffect")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPathEffect").GetMethod("CreateDash", BindingFlags.Public|BindingFlags.Static));

        private SolidBrush _brush;
        public SolidBrush Brush { get { return _brush; } }
        private float _size;
        public float Size { get { return _size;} }

        private float[] _dashPattern;
        public float[] DashPattern { get { return _dashPattern; } set { _dashPattern = value; } }

        public Pen(SolidBrush brush,float size,float[] dashPattern)
        {
            _brush= brush;
            _size= size;
            _dashPattern= dashPattern;
        }

        public Pen(SolidBrush brush, float size)
            : this(brush, size, null) { }

        public Pen(Color color, float size,float[] dashPattern)
            : this(new SolidBrush(color), size,dashPattern) { }

        public Pen(Color color, float size)
            : this(color,size,null) { }

        public object DrawingObject { 
            get {
                object ret = (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _brush.DrawingObject, _size }) : null);
                if (ret!=null) { 
                    if (_dashPattern!=null)
                    {
                        if (ret.GetType().GetProperty("DashPattern", BindingFlags.Public|BindingFlags.Instance)!=null)
                            ret.GetType().GetProperty("DashPattern", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _dashPattern);
                    }
                }
                return ret;
            } 
        }

        public object SkiaObject
        {
            get
            {
                object ret =(_skiaConstructor==null ? null : _skiaConstructor.Invoke(new object[] { }));
                if (ret!=null) { 
                    ret.GetType().GetProperty("Color", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _brush.Color.SkiaObject);
                    ret.GetType().GetProperty("Style", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _skiaStyle);
                    ret.GetType().GetProperty("StrokeWidth", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _size);
                    if (_dashPattern!=null)
                        ret.GetType().GetProperty("PathEffect", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _skiaCreateDash.Invoke(null, new object[] { _dashPattern, 20 }));
                }
                return ret;
            }
        }
    }
}
