using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class SolidBrush : IDrawingObject
    {
        private static readonly ConstructorInfo _drawingConstructor = (DrawingImage.LocateType("System.Drawing.SolidBrush")==null ? null : DrawingImage.LocateType("System.Drawing.SolidBrush").GetConstructor(new Type[] { DrawingImage.LocateType("System.Drawing.Color") }));
        private static readonly ConstructorInfo _skiaConstructor = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetConstructor(Type.EmptyTypes));

        private static readonly object _skiaStyle = (SkiaImage.LocateType("SkiaSharp.SKPaintStyle")==null ? null : Enum.Parse(SkiaImage.LocateType("SkiaSharp.SKPaintStyle"), "StrokeAndFill"));
        private static readonly object _skiaTextAlign = (SkiaImage.LocateType("SkiaSharp.SKTextAlign")==null ? null : Enum.Parse(SkiaImage.LocateType("SkiaSharp.SKTextAlign"), "Left"));
        private static readonly object _skiaTypeFace = (SkiaImage.LocateType("SkiaSharp.SKTypeface")==null ? null : SkiaImage.LocateType("SkiaSharp.SKTypeface").GetMethod("FromFamilyName", new Type[] { typeof(string) }).Invoke(null, new object[] { "Arial" }));

        private static readonly PropertyInfo _skiaColorProperty = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetProperty("Color",BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaStyleProperty = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetProperty("Style", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextAlignProperty = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetProperty("TextAlign", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextSizeProperty = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetProperty("TextSize", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTypeFaceProperty = (SkiaImage.LocateType("SkiaSharp.SKPaint")==null ? null : SkiaImage.LocateType("SkiaSharp.SKPaint").GetProperty("Typeface", BindingFlags.Public | BindingFlags.Instance));

        private Color _color;
        public Color Color { get { return _color; } }

        public SolidBrush(Color color)
        {
            _color = color;
        }
        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _color.DrawingObject }) : null);
            }
        }

        public object SkiaObject
        {
            get
            {
                if (_skiaConstructor!=null)
                {
                    object skb = _skiaConstructor.Invoke(new object[] { });
                    _skiaColorProperty.SetValue(skb, _color.SkiaObject);
                    _skiaStyleProperty.SetValue(skb, _skiaStyle);
                    _skiaTextAlignProperty.SetValue(skb, _skiaTextAlign);
                    _skiaTextSizeProperty.SetValue(skb, Constants.FONT_SIZE*0.89f*1.3333333f);
                    _skiaTypeFaceProperty.SetValue(skb, _skiaTypeFace);
                    return skb;
                }
                return null;
            }
        }
    }
}
