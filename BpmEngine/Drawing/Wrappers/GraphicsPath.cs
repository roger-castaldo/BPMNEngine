using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class GraphicsPath : IDrawingObject
    {
        public const string DRAWING_TYPE = "System.Drawing.Drawing2D.GraphicsPath";
        public const string SKIA_TYPE = "SkiaSharp.SKPath";

        private static readonly Type _DrawingType = DrawingImage.LocateType(DRAWING_TYPE);
        private static readonly ConstructorInfo _drawingConstructor = (_DrawingType==null ? null : _DrawingType.GetConstructor(Type.EmptyTypes));

        private static readonly Type _SkiaType = SkiaImage.LocateType(SKIA_TYPE);
        private static readonly ConstructorInfo _skiaConstructor = (_SkiaType==null ? null : _SkiaType.GetConstructor(Type.EmptyTypes));

        private object _drawingPath;
        private object _skiaPath;
        private bool _first = true;

        public GraphicsPath()
        {
            _drawingPath= (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { }) : null);
            _skiaPath = (_skiaConstructor!=null ? _skiaConstructor.Invoke(new object[] { }) : null);
        }

        private static readonly MethodInfo _drawingAddLine = (_DrawingType==null ? null : _DrawingType.GetMethod("AddLine", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));
        private static readonly MethodInfo _skiaMoveTo = (_SkiaType==null ? null : _SkiaType.GetMethod("MoveTo", new Type[] { typeof(float), typeof(float) }));
        private static readonly MethodInfo _skiaLineTo = (_SkiaType==null ? null : _SkiaType.GetMethod("LineTo", new Type[] { typeof(float), typeof(float) }));
        public void AddLine(float x,float y,float x2,float y2)
        {
            if (_drawingPath!=null)
                _drawingAddLine.Invoke(_drawingPath, new object[] { x, y, x2, y2 });
            if (_skiaPath!=null)
            {
                if (_first)
                {
                    _skiaMoveTo.Invoke(_skiaPath, new object[] { x, y });
                    _first=false;
                }
                _skiaLineTo.Invoke(_skiaPath, new object[] { x2, y2 });
            }
        }

        public void AddLine(Point start,Point end)
        {
            this.AddLine(start.X, start.Y, end.X, end.Y);
        }

        private static readonly MethodInfo _drawingAddArc = (_DrawingType==null ? null : _DrawingType.GetMethod("AddArc",new Type[] {typeof(float), typeof(float), typeof(float), typeof(float),typeof(float),typeof(float)}));
        private static readonly MethodInfo _skiaAddArc = (_SkiaType==null ? null : _SkiaType.GetMethod("AddArc", new Type[] { SkiaImage.LocateType(Rectangle.SKIA_TYPE), typeof(float), typeof(float) }));
        public void AddArc(float x,float y,float width,float height,float startAngle,float sweep)
        {
            if (_drawingPath!=null)
                _drawingAddArc.Invoke(_drawingPath, new object[] { x, y, width, height, startAngle, sweep });
            if (_skiaPath!=null)
            {
                _first=false;
                _skiaAddArc.Invoke(_skiaPath, new object[] { new Rectangle(x, y, width, height).SkiaObject, startAngle, sweep });
            }
        }

        private static readonly MethodInfo _drawingClose = (_DrawingType==null ? null : _DrawingType.GetMethod("CloseFigure", Type.EmptyTypes));
        private static readonly MethodInfo _skiaClose = (_SkiaType==null ? null : _SkiaType.GetMethod("Close", Type.EmptyTypes));
        public void CloseFigure()
        {
            if (_drawingPath!=null)
                _drawingClose.Invoke(_drawingPath, new object[] { });
            if (_skiaPath!=null)
                _skiaClose.Invoke(_skiaPath, new object[] { });
        }

        public object DrawingObject
        {
            get { return _drawingPath; }
        }

        public object SkiaObject
        {
            get { return _skiaPath; }
        }
    }
}
