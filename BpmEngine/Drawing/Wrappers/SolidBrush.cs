using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class SolidBrush : IDrawingObject
    {

        private const string _SKIA_PAINT_TYPE = "SkiaSharp.SKPaint";
        private const string _SKIA_PAINT_STYLE_TYPE = "SkiaSharp.SKPaintStyle";
        private const string _SKIA_TEXT_ALIGN_TYPE = "SkiaSharp.SKTextAlign";
        private const string _SKIA_TYPE_FACE_TYPE = "SkiaSharp.SKTypeface";

        public static readonly string[] SKIA_TYPES = new string[] { _SKIA_PAINT_TYPE, _SKIA_PAINT_STYLE_TYPE, _SKIA_TEXT_ALIGN_TYPE, _SKIA_TYPE_FACE_TYPE };

        private const string _DRAWING_BRUSH_TYPE = "System.Drawing.SolidBrush";
        private const string _DRAWING_COLOR_TYPE = "System.Drawing.Color";

        public static readonly string[] DRAWING_TYPES = new string[] { _DRAWING_BRUSH_TYPE, _DRAWING_COLOR_TYPE };

        private static readonly ConstructorInfo _drawingConstructor = (DrawingImage.LocateType(_DRAWING_BRUSH_TYPE)==null ? null : DrawingImage.LocateType(_DRAWING_BRUSH_TYPE).GetConstructor(new Type[] { DrawingImage.LocateType(_DRAWING_COLOR_TYPE) }));
        private static readonly ConstructorInfo _skiaConstructor = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetConstructor(Type.EmptyTypes));

        private static readonly object _skiaStyle = (SkiaImage.LocateType(_SKIA_PAINT_STYLE_TYPE)==null ? null : Enum.Parse(SkiaImage.LocateType(_SKIA_PAINT_STYLE_TYPE), "StrokeAndFill"));
        private static readonly object _skiaTextAlign = (SkiaImage.LocateType(_SKIA_TEXT_ALIGN_TYPE)==null ? null : Enum.Parse(SkiaImage.LocateType(_SKIA_TEXT_ALIGN_TYPE), "Left"));
        private static readonly object _skiaTypeFace = (SkiaImage.LocateType(_SKIA_TYPE_FACE_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_TYPE_FACE_TYPE).GetMethod("FromFamilyName", new Type[] { typeof(string) }).Invoke(null, new object[] { "Arial" }));

        private static readonly PropertyInfo _skiaColorProperty = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetProperty("Color",BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaStyleProperty = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetProperty("Style", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextAlignProperty = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetProperty("TextAlign", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextSizeProperty = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetProperty("TextSize", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTypeFaceProperty = (SkiaImage.LocateType(_SKIA_PAINT_TYPE)==null ? null : SkiaImage.LocateType(_SKIA_PAINT_TYPE).GetProperty("Typeface", BindingFlags.Public | BindingFlags.Instance));

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
