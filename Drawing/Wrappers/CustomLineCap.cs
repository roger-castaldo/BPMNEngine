using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing.Wrappers
{
    internal class CustomLineCap : IDrawingObject
    {
        private static readonly Type DrawingType = Utility.GetType(DrawingImage.ASSEMBLY_NAME, "System.Drawing.Drawing2D.CustomLineCap");

        private static readonly ConstructorInfo _drawingConstructor = (DrawingType==null ? null : DrawingType.GetConstructor(new Type[] { GraphicsPath.DrawingType, GraphicsPath.DrawingType }));

        private GraphicsPath _strokePath;
        private GraphicsPath _fillPath;

        public CustomLineCap(GraphicsPath fillPath,GraphicsPath strokePath)
        {
            _fillPath = fillPath;
            _strokePath = strokePath;
        }

        
        public object DrawingObject
        {
            get
            {
                return (_drawingConstructor==null ? null : _drawingConstructor.Invoke(new object[] { (_fillPath==null ? null : _fillPath.DrawingObject), (_strokePath==null ? null : _strokePath.DrawingObject) }));
            }
        }
    }
}
