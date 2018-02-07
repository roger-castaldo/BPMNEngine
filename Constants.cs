using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Org.Reddragonit.BpmEngine
{
    internal static class Constants
    {
        public const float PEN_WIDTH = 3F;
        public static readonly float[] DASH_PATTERN = new float[] { 3.0f, 3.0f };
        public const float FONT_SIZE = 9.2F;
        public static readonly Font FONT = new Font(FontFamily.GenericSerif, FONT_SIZE,FontStyle.Regular,GraphicsUnit.Point);
        public const string DATETIME_FORMAT = "yyyyMMddHHmmssfff";

        public static StringFormat STRING_FORMAT
        {
            get
            {
                StringFormat ret = new StringFormat();
                ret.Alignment = StringAlignment.Center;
                ret.LineAlignment = StringAlignment.Center;
                return ret;
            }
        }
    }
}
