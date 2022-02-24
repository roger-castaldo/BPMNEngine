using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class GraphicsPath : IDrawingObject
    {
        public static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Drawing2D.GraphicsPath");
        private static readonly ConstructorInfo _drawingConstructor = (DrawingType==null ? null : DrawingType.GetConstructor(Type.EmptyTypes)); 

        private object _path;

        public GraphicsPath()
        {
            _path = (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { }) : null);
        }

        private static readonly MethodInfo _drawingAddLine = (DrawingType==null ? null : DrawingType.GetMethod("AddLine", new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));
        public void AddLine(float x,float y,float x2,float y2)
        {
            if (_drawingAddLine!=null)
                _drawingAddLine.Invoke(_path, new object[] { x, y, x2, y2 });
        }

        public void AddLine(Point start,Point end)
        {
            this.AddLine(start.X, start.Y, end.X, end.Y);
        }

        private static readonly MethodInfo _drawingAddArc = (DrawingType==null ? null : DrawingType.GetMethod("AddArc",new Type[] {typeof(float), typeof(float), typeof(float), typeof(float),typeof(float),typeof(float)}));
        public void AddArc(float x,float y,float width,float height,float startAngle,float sweep)
        {
            if (_drawingAddArc!=null)
                _drawingAddArc.Invoke(_path,new object[] {x,y,width,height,startAngle,sweep});
        }

        private static readonly MethodInfo _drawingClose = (DrawingType==null ? null : DrawingType.GetMethod("CloseFigure", Type.EmptyTypes));
        public void CloseFigure()
        {
            if (_drawingClose!=null)
                _drawingClose.Invoke(_path, new object[] { });
        }

        public object DrawingObject
        {
            get { return _path; }
        }
    }
}
