using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Rectangle :IDrawingObject
    {
        public static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.RectangleF");
        private static readonly ConstructorInfo _drawingConstructor = (DrawingType==null ? null : DrawingType.GetConstructor(new Type[] { typeof(float), typeof(float), typeof(float), typeof(float) }));

        private float _x;
        public float X { get { return _x; } }
        private float _y;
        public float Y { get { return _y; } }
        private float _width;   
        public float Width { get { return _width; } }
        private float _height;  
        public float Height { get { return _height; } }

        public Rectangle(float x,float y,float width,float height)
        {
            _x = x;
            _y = y; 
            _width = width;
            _height = height;
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] { _x, _y, _width, _height }) : null);
            }
        }
    }
}
