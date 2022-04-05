using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class DrawingImage : IDrawingSurface
    {
        private static readonly string[] _ASSEMBLY_NAME = new string[]{ "System.Drawing.Common", "System.Drawing.Primitives" };

        public static bool CAN_USE = Utility.LoadAssemblies(_ASSEMBLY_NAME);

        private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        public static Type LocateType(string name)
        {
            Type ret = null;
            if (CAN_USE)
            {
                lock (_typeCache)
                {
                    if (_typeCache.ContainsKey(name))
                        ret=_typeCache[name];
                    else
                    {
                        ret=Utility.GetType(_ASSEMBLY_NAME, name);
                        _typeCache.Add(name, ret);
                    }
                }
            }
            return ret;
        }

        private static readonly Type _GraphicsType = LocateType("System.Drawing.Graphics");
        private static readonly Type _BitmapType = LocateType("System.Drawing.Bitmap");

        private static object _FONT = null;

        private static object _CenterStringFormat=null;
        private static object _LeftStringFormat = null;

        private static readonly ConstructorInfo _bmpConstructor = (_BitmapType==null ? null : _BitmapType.GetConstructor(new Type[] { typeof(int),typeof(int) }));

        private static Dictionary<string, MethodInfo> _methods;

        static DrawingImage()
        {
            if (CAN_USE)
            {
                try
                {
                    _FONT= (LocateType("System.Drawing.Font")==null ?
                            null :
                            LocateType("System.Drawing.Font").GetConstructor(new Type[] { LocateType("System.Drawing.FontFamily"), typeof(float), LocateType("System.Drawing.FontStyle"), LocateType("System.Drawing.GraphicsUnit") }).Invoke(
                                new object[]
                                {
                                    Utility.GetType(_ASSEMBLY_NAME,"System.Drawing.FontFamily").GetProperty("GenericSerif",BindingFlags.Static|BindingFlags.Public).GetValue(null),
                                    Constants.FONT_SIZE,
                                    Enum.Parse(Utility.GetType(_ASSEMBLY_NAME,"System.Drawing.FontStyle"),Constants.FONT_STYLE),
                                    Enum.Parse(Utility.GetType(_ASSEMBLY_NAME,"System.Drawing.GraphicsUnit"),Constants.FONT_GRAPHICS_UNIT)
                                }
                            )
                        );
                    Type t = LocateType("System.Drawing.StringFormat");
                    if (t!=null)
                    {
                        _CenterStringFormat = t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        t.GetProperty("Alignment").SetValue(_CenterStringFormat, Enum.Parse(LocateType("System.Drawing.StringAlignment"), "Center"));
                        t.GetProperty("LineAlignment").SetValue(_CenterStringFormat, Enum.Parse(LocateType("System.Drawing.StringAlignment"), "Center"));
                        _LeftStringFormat = t.GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
                        t.GetProperty("Alignment").SetValue(_LeftStringFormat, Enum.Parse(LocateType("System.Drawing.StringAlignment"), "Center"));
                        t.GetProperty("LineAlignment").SetValue(_LeftStringFormat, Enum.Parse(LocateType("System.Drawing.StringAlignment"), "Near"));
                    }
                    _methods = new Dictionary<string, MethodInfo>()
                    {
                        {"FromImage",_GraphicsType.GetMethod("FromImage", BindingFlags.Public|BindingFlags.Static) },
                        {"LoadFromStream", Utility.GetType(_ASSEMBLY_NAME,"System.Drawing.Image").GetMethod("FromStream", new Type[] { typeof(Stream) })},
                        {"TranslateTransform", _GraphicsType.GetMethod("TranslateTransform", new Type[] { typeof(float), typeof(float) })},
                        {"RotateTransform", _GraphicsType.GetMethod("RotateTransform", new Type[] { typeof(float) })},
                        {"DrawImage", _GraphicsType.GetMethod("DrawImage", new Type[] { LocateType("System.Drawing.Image"), LocateType(Rectangle.DRAWING_TYPE) })},
                        {"DrawLines", _GraphicsType.GetMethod("DrawLines", new Type[] { LocateType(Pen.DRAWING_TYPE), LocateType(Point.DRAWING_TYPE+"[]")})},
                        {"DrawLine",_GraphicsType.GetMethod("DrawLine", new Type[] { LocateType(Pen.DRAWING_TYPE), LocateType(Point.DRAWING_TYPE), LocateType(Point.DRAWING_TYPE) }) },
                        {"DrawEllipse",_GraphicsType.GetMethod("DrawEllipse", new Type[] { LocateType(Pen.DRAWING_TYPE), LocateType(Rectangle.DRAWING_TYPE) }) },
                        {"FillEllipse",_GraphicsType.GetMethod("FillEllipse",new Type[] { LocateType("System.Drawing.Brush"),LocateType(Rectangle.DRAWING_TYPE)}) },
                        {"MeasureString",_GraphicsType.GetMethod("MeasureString", new Type[] { typeof(string), _FONT.GetType(), LocateType("System.Drawing.SizeF"), _CenterStringFormat.GetType() }) },
                        {"DrawString",_GraphicsType.GetMethod("DrawString", new Type[] { typeof(string), _FONT.GetType(), LocateType("System.Drawing.Brush"), LocateType(Rectangle.DRAWING_TYPE), _CenterStringFormat.GetType() }) },
                        {"DrawRoundRectangle",_GraphicsType.GetMethod("DrawPath", new Type[] { LocateType(Pen.DRAWING_TYPE), LocateType(RoundRectangle.DRAWING_TYPE) }) },
                        {"Flush",_GraphicsType.GetMethod("Flush", Type.EmptyTypes) },
                        {"GetPixel",_BitmapType.GetMethod("GetPixel", new Type[] { typeof(int), typeof(int) }) },
                        {"SetPixel",_BitmapType.GetMethod("SetPixel", new Type[] { typeof(int), typeof(int), LocateType(Color.DRAWING_TYPE) }) },
                        {"FillPolygon",_GraphicsType.GetMethod("FillPolygon", new Type[] { LocateType("System.Drawing.Brush"), LocateType(LocateType(Point.DRAWING_TYPE).FullName+"[]") }) },
                        {"Save",_BitmapType.GetMethod("Save",new Type[] {typeof(Stream), LocateType("System.Drawing.Imaging.ImageFormat")}) },
                        {"Clear",_GraphicsType.GetMethod("Clear",new Type[]{LocateType(Color.DRAWING_TYPE)}) },
                        {"BMPDispose",_BitmapType.GetMethod("Dispose",Type.EmptyTypes)},
                        {"GPDispose",_GraphicsType.GetMethod("Dispose",Type.EmptyTypes) }
                    };
                }
                catch (Exception e) {
                    CAN_USE=false;
                }
            }
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
            Array arr = Array.CreateInstance(LocateType(Point.DRAWING_TYPE), points.Length);
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
            Array arr = Array.CreateInstance(LocateType(Point.DRAWING_TYPE), points.Length);
            for (int x = 0; x<points.Length; x++)
                arr.SetValue(points[x].DrawingObject,x);
            _methods["FillPolygon"].Invoke(_gp, new object[] { brush.DrawingObject, arr });
        }

        private static readonly Type _imageFormat = (CAN_USE ? LocateType("System.Drawing.Imaging.ImageFormat") : null);
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
