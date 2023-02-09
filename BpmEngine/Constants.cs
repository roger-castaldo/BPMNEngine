using System;
using System.Diagnostics.CodeAnalysis;

namespace Org.Reddragonit.BpmEngine
{
    [ExcludeFromCodeCoverage]
    internal static class Constants
    {
        public const float PEN_WIDTH = 3F;
        public const float WIDE_PEN_WIDTH = 9F;
        public static readonly float[] DASH_PATTERN = new float[] { 3.0f, 3.0f };
        public const float FONT_SIZE = 9.2F;
        public const string FONT_STYLE = "Regular";
        public const string FONT_GRAPHICS_UNIT = "Point";
        public const string DATETIME_FORMAT = "yyyyMMddHHmmssfff";
    }
}
