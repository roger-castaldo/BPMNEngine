using Org.Reddragonit.BpmEngine.Drawing.Gif;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class SkiaImage : IDrawingSurface
    {
        public static readonly string[] _ASSEMBLY_NAME = new string[] { "SkiaSharp" };

        public static readonly bool CAN_USE = Utility.LoadAssemblies(_ASSEMBLY_NAME);

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

        private static readonly Type _GraphicsType = LocateType("SkiaSharp.SKCanvas");
        private static readonly Type _BitmapType = LocateType("SkiaSharp.SKBitmap");

        private static readonly ConstructorInfo _bmpConstructor = (_BitmapType==null ? null : _BitmapType.GetConstructor(new Type[] { typeof(int), typeof(int),typeof(bool) }));
        private static readonly ConstructorInfo _graphicsConstructor = (_GraphicsType==null ? null : _GraphicsType.GetConstructor(new Type[] { _BitmapType }));

        private static Dictionary<string, MethodInfo> _methods;

        static SkiaImage()
        {
            if (CAN_USE)
            {
                try
                {
                    _methods = new Dictionary<string, MethodInfo>()
                {
                    {"FromEncodedData", LocateType("SkiaSharp.SKImage").GetMethod("FromEncodedData", new Type[] { typeof(Stream) })},
                    {"FromImage", _BitmapType.GetMethod("FromImage", new Type[] { LocateType("SkiaSharp.SKImage") })},
                    {"_Clear",_GraphicsType.GetMethod("Clear",Type.EmptyTypes) },
                    {"Clear",_GraphicsType.GetMethod("Clear",new Type[]{LocateType(Color.SKIA_TYPE)}) },
                    {"DrawEllipse",_GraphicsType.GetMethod("DrawOval",new Type[]{LocateType(Rectangle.SKIA_TYPE),LocateType(Pen.SKIA_TYPE)}) },
                    {"FillEllipse",_GraphicsType.GetMethod("DrawOval",new Type[]{LocateType(Rectangle.SKIA_TYPE),LocateType("SkiaSharp.SKPaint")}) },
                    {"DrawImage",_GraphicsType.GetMethod("DrawImage",new Type[]{LocateType("SkiaSharp.SKImage"),LocateType(Rectangle.SKIA_TYPE),LocateType("SkiaSharp.SKPaint")}) },
                    {"_ScalePixels",_BitmapType.GetMethod("ScalePixels",new Type[]{_BitmapType,LocateType("SkiaSharp.SKFilterQuality")}) },
                    {"DrawLine",_GraphicsType.GetMethod("DrawLine",new Type[]{LocateType(Point.SKIA_TYPE),LocateType(Point.SKIA_TYPE),LocateType(Pen.SKIA_TYPE)}) },
                    //{"DrawLines",_GraphicsType.GetMethod("DrawPoints",new Type[]{LocateType("SkiaSharp.SKPointMode"),LocateType(LocateType(Point.SKIA_TYPE).ToString()+"[]"),LocateType(Pen.SKIA_TYPE)}) },
                    {"DrawRoundRectangle",_GraphicsType.GetMethod("DrawRoundRect",new Type[]{LocateType(RoundRectangle.SKIA_TYPE),LocateType(Pen.SKIA_TYPE)}) },
                    {"MeasureString",LocateType("SkiaSharp.SKPaint").GetMethod("MeasureText",new Type[]{typeof(string), LocateType(Rectangle.SKIA_TYPE).MakeByRefType()}) },
                    {"_MeasureString",LocateType("SkiaSharp.SKPaint").GetMethod("MeasureText",new Type[]{typeof(string)}) },
                    {"DrawString",_GraphicsType.GetMethod("DrawText",new Type[]{typeof(string),typeof(float),typeof(float),LocateType("SkiaSharp.SKPaint")}) },
                    {"FillPolygon",_GraphicsType.GetMethod("DrawPath",new Type[]{LocateType("SkiaSharp.SKPath"),LocateType("SkiaSharp.SKPaint")}) },
                    {"Flush",_GraphicsType.GetMethod("Flush",Type.EmptyTypes) },
                    {"GetPixel",_BitmapType.GetMethod("GetPixel",new Type[]{typeof(int),typeof(int)}) },
                    {"RotateTransform",_GraphicsType.GetMethod("RotateDegrees",new Type[]{typeof(float)}) },
                    {"SetPixel",_BitmapType.GetMethod("SetPixel",new Type[]{typeof(int),typeof(int),LocateType(Color.SKIA_TYPE)}) },
                    {"Save",_BitmapType.GetMethod("Encode",new Type[]{typeof(Stream),LocateType("SkiaSharp.SKEncodedImageFormat"),typeof(int)}) },
                    {"TranslateTransform",_GraphicsType.GetMethod("Translate",new Type[]{typeof(float),typeof(float)}) },
                    {"FromBitmap",LocateType("SkiaSharp.SKImage").GetMethod("FromBitmap", new Type[] { _BitmapType }) }
                };
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private Size _size;
        public Size Size { get { return _size; } }

        private object _bmp;
        private object _gp;
        public object DrawingObject
        {
            get { return null; }
        }

        public object SkiaObject
        {
            get { return (_bmp==null ? null : (_bmp.GetType().FullName==_BitmapType.FullName ? _methods["FromBitmap"].Invoke(null,new object[] {_bmp}) : _bmp)); }
        }

        public SkiaImage(Size size)
        {
            _size = size;
            _bmp = _bmpConstructor.Invoke(new object[] { size.Width, size.Height,false });
            _gp = _graphicsConstructor.Invoke(new object[] { _bmp });
            _methods["_Clear"].Invoke(_gp, new object[] { });
        }

        public SkiaImage(Stream str)
        {
            _bmp = _methods["FromEncodedData"].Invoke(null, new object[] { str });
            if (_bmp.GetType().FullName!=_BitmapType.FullName)
                _bmp = _methods["FromImage"].Invoke(null, new object[] { _bmp });
            _size = new Size((int)_bmp.GetType().GetProperty("Width").GetValue(_bmp), (int)_bmp.GetType().GetProperty("Height").GetValue(_bmp));
            _gp = _graphicsConstructor.Invoke(new object[] { _bmp });
        }

        public void DrawEllipse(Pen pen, Rectangle rect)
        {
            _methods["DrawEllipse"].Invoke(_gp, new object[] { rect.SkiaObject, pen.SkiaObject });
        }

        public void Clear(Color color)
        {
            _methods["Clear"].Invoke(_gp, new object[] { color.SkiaObject });
        }

        private static readonly object _filterQuality = (CAN_USE ? Enum.Parse(LocateType("SkiaSharp.SKFilterQuality"), "High") : null);
        public void DrawImage(IDrawingSurface image, Rectangle rect)
        {
            object img = image.SkiaObject;
            if (image.Size.Width!=rect.Width||image.Size.Height!=rect.Height)
            {
                object tmp = _bmpConstructor.Invoke(new object[] { (int)rect.Width, (int)rect.Height,false });
                _methods["_ScalePixels"].Invoke(_methods["FromImage"].Invoke(null, new object[] { img }), new object[] { tmp, _filterQuality });
                img=_methods["FromBitmap"].Invoke(null, new object[] { tmp });
            }
            _methods["DrawImage"].Invoke(_gp, new object[] { img, rect.SkiaObject, null });
        }

        public void DrawLine(Pen pen, Point start, Point end)
        {
            _methods["DrawLine"].Invoke(_gp, new object[] { start.SkiaObject, end.SkiaObject, pen.SkiaObject });
        }

        private object _ConvertPoints(Point[] points)
        {
            Array arr = Array.CreateInstance(LocateType(Point.SKIA_TYPE), points.Length);
            for (int x = 0; x<points.Length; x++)
                arr.SetValue(points[x].SkiaObject, x);
            return arr;
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            //_methods["DrawLines"].Invoke(_gp, new object[] { _pointModeLines,_ConvertPoints(points), pen.SkiaObject });
            for(int x=0;x<points.Length-1;x++)
                DrawLine(pen, points[x], points[x+1]);
        }

        public void DrawRoundRectangle(Pen pen, RoundRectangle rect)
        {
            _methods["DrawRoundRectangle"].Invoke(_gp, new object[] { rect.SkiaObject, pen.SkiaObject });
        }

        public void DrawString(string content, SolidBrush brush, Rectangle rect,bool center)
        {
            content = _WordWrap(content, rect.Width);
            Size s = MeasureString(content, new Size(rect.Width, rect.Height));
            if (center)
            {
                float y = rect.Y+((rect.Height-s.Height)/2);
                foreach (string str in content.Split('\n'))
                {
                    s = MeasureString(str, new Size(rect.Width, rect.Height));
                    _methods["DrawString"].Invoke(_gp, new object[] { str, rect.X+((rect.Width-s.Width)/2), y, brush.SkiaObject });
                    y+=s.Height;
                }
            }
            else
            {
                float offset = 0;
                foreach (string str in content.Split('\n'))
                {
                    s = MeasureString(str, new Size(rect.Width, rect.Height));
                    _methods["DrawString"].Invoke(_gp, new object[] { str, rect.X+1.5f, rect.Y+offset, brush.SkiaObject });
                    offset+=s.Height;
                }
            }
        }

        public void FillEllipse(SolidBrush brush, Rectangle rect)
        {
            _methods["FillEllipse"].Invoke(_gp, new object[] { rect.SkiaObject, brush.SkiaObject });
        }

        private static readonly Type _skPath = LocateType("SkiaSharp.SKPath");
        private static readonly ConstructorInfo _skPathConstructor = (_skPath==null ? null : _skPath.GetConstructor(Type.EmptyTypes));
        private static readonly MethodInfo _skPathMoveTo = (_skPath==null ? null : _skPath.GetMethod("MoveTo", new Type[] { typeof(float), typeof(float) }));
        private static readonly MethodInfo _skPathLineTo = (_skPath==null ? null : _skPath.GetMethod("LineTo", new Type[] { typeof(float), typeof(float) }));
        private static readonly MethodInfo _skPathClose = (_skPath==null ? null : _skPath.GetMethod("Close", Type.EmptyTypes));
        public void FillPolygon(SolidBrush brush, Point[] points)
        {
            if (points[0]!=points[points.Length-1])
            {
                Point[] tmp = new Point[points.Length+1];
                points.CopyTo(tmp, 0);
                tmp[tmp.Length-1]= points[0];
                points=tmp;
            }
            object path = _skPathConstructor.Invoke(new object[] { });
            _skPathMoveTo.Invoke(path, new object[] { points[0].X, points[0].Y });
            for(int x = 1; x<points.Length; x++)
                _skPathLineTo.Invoke(path, new object[] { points[x].X, points[x].Y });
            _skPathClose.Invoke(path,new object[] { });
            _methods["FillPolygon"].Invoke(_gp, new object[] { path, brush.SkiaObject });
        }

        public void Flush()
        {
            _methods["Flush"].Invoke(_gp, new object[] { });
        }

        public Color GetPixel(int x, int y)
        {
            return Color.FromDrawingObject(_methods["GetPixel"].Invoke(_bmp, new object[] { x, y }));
        }

        private object _measurePaint = new SolidBrush(Color.Black).SkiaObject;
        private float _MeasureString(string content)
        {
            return (float)_methods["_MeasureString"].Invoke(_measurePaint, new object[] { content });
        }

        private string _WordWrap(string content,float maxWidth)
        {
            content=content.Trim();
            if (content=="")
                return content;
            if (content.Contains("\n"))
                return content;
            string[] splt = content.Split(' ');
            List<string> ret = new List<string>();

            string tmp = "";
            int idx = 0;
            //maxWidth-=2f;
            while (true)
            {
                float width = _MeasureString(tmp);
                if (width<maxWidth)
                {
                    if (idx==splt.Length)
                        break;
                    tmp+=" "+splt[idx];
                    tmp=tmp.Trim();
                    idx++;
                }else if (width==maxWidth)
                {
                    ret.Add(tmp);
                    tmp="";
                }
                else
                {
                    if (idx==1)
                    {
                        ret.Add(tmp);
                        tmp="";
                    }
                    else
                    {
                        if (tmp==splt[idx-1])
                        {
                            ret.Add(tmp);
                            tmp="";
                        }
                        else { 
                            tmp = tmp.Substring(0,tmp.Length-splt[idx-1].Length).Trim();
                            ret.Add(tmp);
                            tmp=splt[idx-1];
                        }
                    }
                }
            }
            if (tmp!="")
                ret.Add(tmp);

            string res = "";
            foreach (string str in ret)
                res+=str+"\n";
            return res.Trim();
        }

        public Size MeasureString(string content, Size container)
        {
            object[] args = new object[] { content, new Rectangle(0, 0, (container==null ? float.MaxValue : container.Width), (container==null ? float.MaxValue : container.Height)).SkiaObject };
            if (container==null)
                _methods["MeasureString"].Invoke(_measurePaint, args);
            else
            {
                content = _WordWrap(content, container.Width);
                if (content.Contains("\n"))
                {
                    string[] splt = content.Split('\n');
                    args[0]=splt[0];
                    _methods["MeasureString"].Invoke(_measurePaint, args);
                    Rectangle r = new Rectangle(args[1]);
                    args[1] = new Rectangle(r.X, r.Y, r.Width, r.Height*splt.Length);
                }
                else
                    _methods["MeasureString"].Invoke(_measurePaint, args);
            }
            Rectangle rect = new Rectangle(args[1]);
            return new Size(rect.Width, rect.Height);
        }

        public void RotateTransform(float angle)
        {
            _methods["RotateTransform"].Invoke(_gp, new object[] { angle });
        }

        public void SetPixel(int x, int y, Color color)
        {
            _methods["SetPixel"].Invoke(_bmp, new object[] { x, y, color.SkiaObject });
        }

        private static readonly Type _imageFormatEnum = LocateType("SkiaSharp.SKEncodedImageFormat");
        public byte[] ToFile(ImageOuputTypes type)
        { 
            MemoryStream ms = new MemoryStream();
            _methods["Save"].Invoke(_bmp, new object[]
            {
                ms,
                Enum.Parse(_imageFormatEnum,type.ToString()),
                100
            });
            return ms.ToArray();
        }

        public void TranslateTransform(float x, float y)
        {
            _methods["TranslateTransform"].Invoke(_gp, new object[] { x, y });
        }

        public void Dispose()
        {
            try
            {
                if (_gp!=null)
                    ((IDisposable)_gp).Dispose();
            }catch(Exception e) { }
            try
            {
                if (_bmp!=null)
                    ((IDisposable)_bmp).Dispose();
            }
            catch (Exception e) { }
        }
    }
}
