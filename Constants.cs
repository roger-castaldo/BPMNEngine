using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Constants
    {
        public const float PEN_WIDTH = 3.5F;
        public static readonly float[] DASH_PATTERN = new float[] { 3.0f, 3.0f };
        public const float FONT_SIZE = 10F;
        public static readonly Font FONT = new Font(FontFamily.GenericSerif, FONT_SIZE,FontStyle.Regular,GraphicsUnit.Point);
    }
}
