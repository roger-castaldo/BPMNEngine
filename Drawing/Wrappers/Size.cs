using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Size : IDrawingObject
    {
        public static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Size");
        private static readonly ConstructorInfo _drawingConstructor = (DrawingType==null ? null : DrawingType.GetConstructor(new Type[] { typeof(int), typeof(int) }));

        private static readonly ConstructorInfo _floatConstructor = (DrawingType==null ? null : Utility.GetType(DrawingImage.ASSEMBLY_NAME,"System.Drawing.SizeF").GetConstructor(new Type[] { typeof(float), typeof(float) }));

        private int _width;
        public int Width { get { return _width; } }
        private int _height;
        public int Height { get { return _height; } }

        public Size(int width,int height)
        {
            _width=width;
            _height=height;
        }

        public Size(float width,float height)
        {
            _width=(int)Math.Ceiling(width);
            _height=(int)Math.Ceiling(height);
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] {_width,_height}) : null);  
            }
        }

        public object FloatDrawingObject
        {
            get
            {
                return (_floatConstructor!=null ? _floatConstructor.Invoke(new object[] { (float)_width, (float)_height }) : null);
            }
        }
    }
}
