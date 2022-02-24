using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class SolidBrush : IDrawingObject
    {
        private static readonly ConstructorInfo _drawingConstructor = (Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.SolidBrush")==null ? null : Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.SolidBrush").GetConstructor(new Type[] { Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Color") }));

        private Color _color;

        public SolidBrush(Color color)
        {
            _color = color;
        }
        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor!=null ? _drawingConstructor.Invoke(new object[] {_color.DrawingObject}) : null);
            }
        }
    }
}
