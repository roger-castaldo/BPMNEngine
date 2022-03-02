using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class DrawingImage : IDrawingSurface
    {
        public static readonly string[] ASSEMBLY_NAME = new string[]{ "System.Drawing.Common", "System.Drawing.Primitives" };

        private static readonly Type _GraphicsType = Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Graphics");
        private static readonly Type _BitmapType = Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Bitmap");

        private static readonly object _FONT = (Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Font")==null ?
            null :
            Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Font").GetConstructor(new Type[] { Utility.GetType(ASSEMBLY_NAME,"System.Drawing.FontFamily"), typeof(float), Utility.GetType(ASSEMBLY_NAME,"System.Drawing.FontStyle"), Utility.GetType(ASSEMBLY_NAME,"System.Drawing.GraphicsUnit") }).Invoke(
                new object[]
                {
                    Utility.GetType(ASSEMBLY_NAME,"System.Drawing.FontFamily").GetProperty("GenericSerif",BindingFlags.Static|BindingFlags.Public).GetValue(null),
                    Constants.FONT_SIZE,
                    Enum.Parse(Utility.GetType(ASSEMBLY_NAME,"System.Drawing.FontStyle"),Constants.FONT_STYLE),
                    Enum.Parse(Utility.GetType(ASSEMBLY_NAME,"System.Drawing.GraphicsUnit"),Constants.FONT_GRAPHICS_UNIT)
                }
            )
        );

        private static object _CenterStringFormat=null;
        private static object _LeftStringFormat = null;

        private static readonly ConstructorInfo _bmpConstructor = (_BitmapType==null ? null : _BitmapType.GetConstructor(new Type[] { typeof(int),typeof(int) }));

        private static Dictionary<string, MethodInfo> _methods;

        static DrawingImage()
        {
            if (_GraphicsType!=null)
            {
                Type t = Utility.GetType(ASSEMBLY_NAME, "System.Drawing.StringFormat");
                if (t!=null)
                {
                    _CenterStringFormat = t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                    t.GetProperty("Alignment").SetValue(_CenterStringFormat, Enum.Parse(Utility.GetType(ASSEMBLY_NAME, "System.Drawing.StringAlignment"), "Center"));
                    t.GetProperty("LineAlignment").SetValue(_CenterStringFormat, Enum.Parse(Utility.GetType(ASSEMBLY_NAME, "System.Drawing.StringAlignment"), "Center"));
                    _LeftStringFormat = t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                    t.GetProperty("Alignment").SetValue(_LeftStringFormat, Enum.Parse(Utility.GetType(ASSEMBLY_NAME, "System.Drawing.StringAlignment"), "Center"));
                    t.GetProperty("LineAlignment").SetValue(_LeftStringFormat, Enum.Parse(Utility.GetType(ASSEMBLY_NAME, "System.Drawing.StringAlignment"), "Near"));
                }
                _methods = new Dictionary<string, MethodInfo>()
                {
                    {"FromImage",_GraphicsType.GetMethod("FromImage", BindingFlags.Public|BindingFlags.Static) },
                    {"LoadFromStream", Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Image").GetMethod("FromStream", new Type[] { typeof(Stream) })},
                    {"TranslateTransform", _GraphicsType.GetMethod("TranslateTransform", new Type[] { typeof(float), typeof(float) })},
                    {"RotateTransform", _GraphicsType.GetMethod("RotateTransform", new Type[] { typeof(float) })},
                    {"DrawImage", _GraphicsType.GetMethod("DrawImage", new Type[] { Utility.GetType(ASSEMBLY_NAME, "System.Drawing.Image"), Rectangle.DrawingType })},
                    {"DrawLines", _GraphicsType.GetMethod("DrawLines", new Type[] { Pen.DrawingType, Utility.GetType(ASSEMBLY_NAME, Point.DrawingType.FullName+"[]")})},
                    {"DrawLine",_GraphicsType.GetMethod("DrawLine", new Type[] { Pen.DrawingType, Point.DrawingType, Point.DrawingType }) },
                    {"DrawEllipse",_GraphicsType.GetMethod("DrawEllipse", new Type[] { Pen.DrawingType, Rectangle.DrawingType }) },
                    {"FillEllipse",_GraphicsType.GetMethod("FillEllipse",new Type[] { Utility.GetType(ASSEMBLY_NAME, "System.Drawing.Brush"),Rectangle.DrawingType}) },
                    {"MeasureString",_GraphicsType.GetMethod("MeasureString", new Type[] { typeof(string), _FONT.GetType(), Utility.GetType(ASSEMBLY_NAME,"System.Drawing.SizeF"), _CenterStringFormat.GetType() }) },
                    {"DrawString",_GraphicsType.GetMethod("DrawString", new Type[] { typeof(string), _FONT.GetType(), Utility.GetType(ASSEMBLY_NAME, "System.Drawing.Brush"), Rectangle.DrawingType, _CenterStringFormat.GetType() }) },
                    {"DrawRoundRectangle",_GraphicsType.GetMethod("DrawPath", new Type[] { Pen.DrawingType, RoundRectangle.DrawingType }) },
                    {"Flush",_GraphicsType.GetMethod("Flush", Type.EmptyTypes) },
                    {"GetPixel",_BitmapType.GetMethod("GetPixel", new Type[] { typeof(int), typeof(int) }) },
                    {"SetPixel",_BitmapType.GetMethod("SetPixel", new Type[] { typeof(int), typeof(int), Color.DrawingType }) },
                    {"FillPolygon",_GraphicsType.GetMethod("FillPolygon", new Type[] { Utility.GetType(ASSEMBLY_NAME,"System.Drawing.Brush"), Utility.GetType(ASSEMBLY_NAME, Point.DrawingType.FullName+"[]") }) },
                    {"Save",_BitmapType.GetMethod("Save",new Type[] {typeof(Stream), Utility.GetType(ASSEMBLY_NAME, "System.Drawing.Imaging.ImageFormat")}) },
                    {"Clear",_GraphicsType.GetMethod("Clear",new Type[]{Color.DrawingType}) },
                    {"BMPDispose",_BitmapType.GetMethod("Dispose",Type.EmptyTypes)},
                    {"GPDispose",_GraphicsType.GetMethod("Dispose",Type.EmptyTypes) }
                };
            }
        }

        public static bool CanUse
        {
            get { return _GraphicsType!=null; }
        }

        private object _bmp;
        private object _gp;

        private Size _size;
        public Size Size { get { return _size; } }

        public object DrawingObject
        {
            get { return _bmp; }
        }

        public object SkiaObject { get { return null; } }

        public DrawingImage(Size size)
        {
            _size = size;
            _bmp = _bmpConstructor.Invoke(new object[] { size.Width, size.Height });
            _gp = _methods["FromImage"].Invoke(null,new object[] { _bmp });
        }

        public DrawingImage(Stream str)
        {
            _bmp = _methods["LoadFromStream"].Invoke(null, new object[] { str });
            _size = new Size((int)_bmp.GetType().GetProperty("Width").GetValue(_bmp), (int)_bmp.GetType().GetProperty("Height").GetValue(_bmp));
            _gp = _methods["FromImage"].Invoke(null, new object[] { _bmp });
        }

        public void Clear(Color color)
        {
            _methods["Clear"].Invoke(_gp, new object[] { color.DrawingObject });
        }
        public void TranslateTransform(float x, float y)
        {
            _methods["TranslateTransform"].Invoke(_gp, new object[] { x, y });
        }

        public void RotateTransform(float angle)
        {
            _methods["RotateTransform"].Invoke(_gp, new object[] { angle });
        }

        public void DrawImage(IDrawingSurface image, Rectangle rect)
        {
            _methods["DrawImage"].Invoke(_gp, new object[] { image.DrawingObject, rect.DrawingObject });
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            Array arr = Array.CreateInstance(Point.DrawingType, points.Length);
            for (int x = 0; x<points.Length; x++)
                arr.SetValue(points[x].DrawingObject, x);
            _methods["DrawLines"].Invoke(_gp, new object[] { pen.DrawingObject, arr });
        }

        public void DrawLine(Pen pen, Point start, Point end)
        {
            _methods["DrawLine"].Invoke(_gp, new object[] { pen.DrawingObject, start.DrawingObject, end.DrawingObject });
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            _methods["DrawEllipse"].Invoke (_gp, new object[] { pen.DrawingObject, rect.DrawingObject });
        }

        public void FillEllipse(SolidBrush brush,Rectangle rect)
        {
            _methods["FillEllipse"].Invoke(_gp,new object[] {brush.DrawingObject, rect.DrawingObject });
        }

        public Size MeasureString(string content, Size container)
        {
            object tmp = _methods["MeasureString"].Invoke(_gp, new object[] { content, _FONT, (container==null ? null : container.FloatDrawingObject), _CenterStringFormat });
            return new Size((int)Math.Ceiling((float)tmp.GetType().GetProperty("Width").GetValue(tmp)), (int)Math.Ceiling((float)tmp.GetType().GetProperty("Height").GetValue(tmp)));
        }

        public void DrawString(string content, SolidBrush brush, Rectangle rect,bool center)
        {
            _methods["DrawString"].Invoke(_gp, new object[] { content, _FONT, brush.DrawingObject,rect.DrawingObject,(center ? _CenterStringFormat : _LeftStringFormat) });
        }

        public void DrawRoundRectangle(Pen pen, RoundRectangle rect)
        {
            _methods["DrawRoundRectangle"].Invoke(_gp, new object[] { pen.DrawingObject, rect.DrawingObject });
        }

        public void Flush()
        {
            _methods["Flush"].Invoke(_gp, new object[] { });
        }

        public Color GetPixel(int x, int y)
        {
            return Color.FromDrawingObject(_methods["GetPixel"].Invoke(_bmp, new object[] { x, y }));
        }

        public void SetPixel(int x, int y, Color color)
        {
            _methods["SetPixel"].Invoke(_bmp, new object[] { x, y, color.DrawingObject });
        }

        public void FillPolygon(SolidBrush brush, Point[] points)
        {
            Array arr = Array.CreateInstance(Point.DrawingType, points.Length);
            for (int x = 0; x<points.Length; x++)
                arr.SetValue(points[x].DrawingObject,x);
            _methods["FillPolygon"].Invoke(_gp, new object[] { brush.DrawingObject, arr });
        }

        private static readonly Type _imageFormat = Utility.GetType(ASSEMBLY_NAME, "System.Drawing.Imaging.ImageFormat");
        public byte[] ToFile(ImageOuputTypes type)
        {
            try
            {
                Flush();
            }
            catch (Exception e) { }
            MemoryStream ms = new MemoryStream();
            _methods["Save"].Invoke(_bmp,new object[] {ms,_imageFormat.GetProperty(type.ToString(),BindingFlags.Public|BindingFlags.Static).GetValue(null)});
            return ms.ToArray();
        }

        public void Dispose()
        {
            try
            {
                if (_gp!=null)
                    _methods["GPDispose"].Invoke(_gp, new object[] { });
            }
            catch (Exception e) { }

            try
            {
                if (_bmp!=null)
                    _methods["BMPDispose"].Invoke(_gp, new object[] { });
            }
            catch (Exception e) { }
        }
    }
}
