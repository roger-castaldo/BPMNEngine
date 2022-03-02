using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class SolidBrush : IDrawingObject
    {
        private static readonly ConstructorInfo _drawingConstructor = (Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.SolidBrush")==null ? null : Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.SolidBrush").GetConstructor(new Type[] { Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Color") }));
        private static readonly ConstructorInfo _skiaConstructor = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetConstructor(Type.EmptyTypes));

        private static readonly object _skiaStyle = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaintStyle")==null ? null : Enum.Parse(Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaintStyle"), "StrokeAndFill"));
        private static readonly object _skiaTextAlign = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKTextAlign")==null ? null : Enum.Parse(Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKTextAlign"), "Left"));
        private static readonly object _skiaTypeFace = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKTypeface")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKTypeface").GetMethod("FromFamilyName", new Type[] { typeof(string) }).Invoke(null, new object[] { "Arial" }));

        private static readonly PropertyInfo _skiaColorProperty = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetProperty("Color",BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaStyleProperty = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetProperty("Style", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextAlignProperty = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetProperty("TextAlign", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTextSizeProperty = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetProperty("TextSize", BindingFlags.Public | BindingFlags.Instance));
        private static readonly PropertyInfo _skiaTypeFaceProperty = (Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint")==null ? null : Utility.GetType(SkiaImage.ASSEMBLY_NAME, "SkiaSharp.SKPaint").GetProperty("Typeface", BindingFlags.Public | BindingFlags.Instance));

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
