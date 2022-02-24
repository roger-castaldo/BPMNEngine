using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class Color : IDrawingObject
    {
        public static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Color");

        private static readonly MethodInfo _drawingMethod = (DrawingType == null ? null : DrawingType.GetMethod("FromArgb", new Type[] {typeof(int), typeof(int) , typeof(int) , typeof(int) }));

        public static readonly Color White = new Color(255, 255,255,255);
        public static readonly Color Red = new Color(255, 255, 0, 0);
        public static readonly Color Green = new Color(255, 0, 255, 0);
        public static readonly Color Blue = new Color(255, 0, 0, 255);
        public static readonly Color Black = new Color(255, 0, 0, 0);

        private int _a;
        public int A { get { return _a; } }
        private int _r;
        public int R { get { return _r; } }
        private int _g;
        public int G { get { return _g; } }
        private int _b;
        public int B { get { return _b; } }

        private Color(int a,int r,int g,int b)
        {
            _a=a;
            _r=r;
            _g=g;   
            _b=b;
        }

        internal static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color(a, r, g, b);
        }

        private Color(byte a, byte r, byte g, byte b)
        : this((int)a, (int)r, (int)g, (int)b) { }

        internal static Color FromDrawingObject(object obj)
        {
            if (obj.GetType().FullName==DrawingType.FullName)
            {
                return new Color(
                    (byte)DrawingType.GetProperty("A").GetValue(obj),
                    (byte)DrawingType.GetProperty("R").GetValue(obj),
                    (byte)DrawingType.GetProperty("G").GetValue(obj),
                    (byte)DrawingType.GetProperty("B").GetValue(obj)
                );
            }
            return null;
        }

        public object DrawingObject
        {
            get
            {
                return (_drawingMethod!=null ? _drawingMethod.Invoke(null,new object[] { _a, _r, _g, _b }) : null);
            }
        }
    }
}
