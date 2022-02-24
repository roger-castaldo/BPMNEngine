using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Pen : IDrawingObject
    {
        public static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Pen");
        private static readonly ConstructorInfo _drawingConstructor = (DrawingType==null ? null : DrawingType.GetConstructor(new Type[] { Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Brush"), typeof(float) }));

        private SolidBrush _brush;
        public SolidBrush Brush { get { return _brush; } }
        private float _size;
        public float Size { get { return _size;} }

        private float[] _dashPattern;
        public float[] DashPattern { get { return _dashPattern; } set { _dashPattern = value; } }

        public Pen(SolidBrush brush,float size)
        {
            _brush= brush;
            _size= size;
        }

        public Pen(Color color, float size)
            : this(new SolidBrush(color), size) { }

        public object DrawingObject { 
            get {
                object ret = (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _brush.DrawingObject, _size }) : null);
                if (_dashPattern!=null) {
                    if (ret.GetType().GetProperty("DashPattern", BindingFlags.Public|BindingFlags.Instance)!=null)
                        ret.GetType().GetProperty("DashPattern", BindingFlags.Public|BindingFlags.Instance).SetValue(ret, _dashPattern);
                }
                return ret;
            } 
        }
    }
}
