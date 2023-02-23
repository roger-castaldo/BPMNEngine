using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Platform;
using Microsoft.Maui.Graphics.Skia;
using Org.Reddragonit.BpmEngine.Drawing.Extensions;
using Org.Reddragonit.BpmEngine.Elements.Processes.Events.Definitions.Extensions;
using Org.Reddragonit.BpmEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Org.Reddragonit.BpmEngine.Drawing
{
    internal class Image : IDisposable {


        public const int VerticalTextShift = 5;
        public const float EdgeLabelVerticalShift = 6f;
        public const float EdgeLabelHorizontalShift = -1.5f;
        public const float FONT_SIZE = 9.2F*0.89f*1.3333333f;
        private static readonly IFont DefaultFont = new Font("Arial");

        public static readonly Color White = new Color(255, 255, 255,255);
        public static readonly Color Red = new Color(179, 0, 0,255);
        public static readonly Color Green = new Color(0, 179, 0,255);
        public static readonly Color Blue = new Color(0, 0, 179,255);
        public static readonly Color Black = new Color(0, 0, 0,255);
        public static readonly Color GoldenYellow = new Color(255, 184, 28,255);
        public static readonly Color Orange = new Color(255, 165, 0,255);

        private readonly SkiaBitmapExportContext _image;
        private readonly ICanvas _surface;

        public static Rect ProduceRectangle(Point p1,Point p2)
        {
            return new Rect(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X-p2.X),
                Math.Abs(p1.Y-p2.Y)
            );
        }

        public static Rect MergeRectangle(Rect initial,Rect? additional)
        {
            if (additional==null)
                return initial;
            double minX = Math.Min(initial.Left, additional.Value.Left);
            double minY = Math.Min(initial.Top, additional.Value.Top);
            double maxX = Math.Max(initial.Left+initial.Width, additional.Value.X+additional.Value.Width);
            double maxY = Math.Max(initial.Left + initial.Width, additional.Value.Y+additional.Value.Height);
            return new Rect(minX, minY, Math.Abs(maxX-minX), Math.Abs(maxY-minY));
        }


        public Size Size => new(_image.Width,_image.Height);

        public Image(Rect rect)
            : this((int)rect.Width, (int)rect.Height) { }

        public Image(double width, double height)
            : this((int)Math.Ceiling(width), (int)Math.Ceiling(height)) { }

        public Image(int width,int height)
            : this(new Size(width, height)) { }

        public Image(Size size)
        {
            _image = new SkiaBitmapExportContext((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height), 1.0f,transparent:true);
            _surface = _image.Canvas;
        }

        private Image(Stream str)
        {
            var image = SkiaImage.FromStream(str);
            _image = new SkiaBitmapExportContext((int)Math.Ceiling(image.Width), (int)Math.Ceiling(image.Height), 1.0f);
            _surface=_image.Canvas;
            _surface.DrawImage(image, 0, 0, image.Width, image.Height);
        }

        public void Clear(Color color)
        {
            _surface.FillColor=color;
            _surface.FillRectangle(new Rect(0, 0, _image.Width, _image.Height));
        }

        public void TranslateTransform(double x, double y)
        {
            _surface.Translate((float)x, (float)y);
        }

        public void RotateTransform(float angle)
        {
            _surface.Rotate(angle);
        }

        public void DrawImage(Image image, Point point)
        {
            DrawImage(image, new RectF((float)point.X, (float)point.Y, (float)image.Size.Width, (float)image.Size.Height));
        }

        public void DrawImage(Image image, RectF rect)
        {
            _surface.DrawImage(image._image.Image, rect.X,rect.Y,rect.Width, rect.Height);
        }

        private void _SetPen(Pen pen)
        {
            _surface.StrokeColor=pen.Color;
            _surface.StrokeDashPattern=pen.DashPattern;
            _surface.StrokeSize=pen.Size;
        }

        public void DrawLines(Pen pen, Point[] points)
        {
            for(int x = 1; x<points.Length; x++)
                DrawLine(pen, points[x-1], points[x]);
        }

        public void DrawLine(Pen pen, Point start, Point end)
        {
            _SetPen(pen);
            _surface.DrawLine(start, end);
        }

        public void DrawEllipse(Pen pen, RectF rect)
        {
            _SetPen(pen);
            _surface.DrawEllipse(rect.X,rect.Y,rect.Width,rect.Height);
        }

        public void FillEllipse(Color color,RectF rect)
        {
            _surface.FillColor=color;
            _surface.FillEllipse(rect);
        }

        public void FillPolygon(Color color, Point[] points)
        {
            _surface.FillColor=color;
            var path = new PathF(points[0]);
            for (int x = 1; x<points.Length; x++)
                path.LineTo(points[x]);
            path.Close();
            _surface.FillPath(path);
        }

        public void DrawRectangle(Pen pen, RectF rect)
        {
            _SetPen(pen);
            _surface.DrawRectangle(rect);
        }
        public void FillRectangle(Color color, RectF rect)
        {
            _surface.FillColor=color;
            _surface.FillRectangle(rect);
        }

        private string _WordWrap(string content, float maxWidth)
        {
            content=content.Trim();
            if (content=="")
                return content;
            if (content.Contains("\n"))
                return content;
            string[] splt = content.Split(' ');
            List<string> ret = new List<string>();

            var tmp = new StringBuilder();
            int idx = 0;
            //maxWidth-=2f;
            while (true)
            {
                float width = MeasureString(tmp.ToString()).Width;
                if (width<maxWidth)
                {
                    if (idx==splt.Length)
                        break;
                    tmp.AppendFormat(" {0}", splt[idx].Trim());
                    idx++;
                }
                else if (width==maxWidth)
                {
                    ret.Add(tmp.ToString());
                    tmp.Clear();
                }
                else
                {
                    if (idx==1)
                    {
                        ret.Add(tmp.ToString());
                        tmp.Clear();
                    }
                    else
                    {
                        if (tmp.ToString()==splt[idx-1])
                        {
                            ret.Add(tmp.ToString());
                            tmp.Clear();
                        }
                        else
                        {
                            tmp.Length-=splt[idx-1].Length;
                            ret.Add(tmp.ToString().Trim());
                            tmp.Clear();
                            tmp.Append(splt[idx-1].Trim());
                        }
                    }
                }
            }
            if (tmp.Length!=0)
                ret.Add(tmp.ToString());

            StringBuilder res = new StringBuilder();
            foreach (string str in ret)
                res.AppendLine(str);
            return res.ToString().Trim();
        }

        public SizeF MeasureString(string content)
        {
            return _surface.GetStringSize(content, DefaultFont, FONT_SIZE);
        }

        public SizeF MeasureString(string content, SizeF container)
        {
            content = _WordWrap(content, container.Width);
            if (content.Contains("\n"))
            {
                string[] splt = content.Split('\n');
                var size = MeasureString(splt[0]);
                return new SizeF(size.Width, size.Height*splt.Length);
            }
            return MeasureString(content);
        }

        public void DrawString(string content, Color color,Point point)
        {
            var sf = MeasureString(content);
            DrawString(content, color, new RectF((float)point.X, (float)point.Y, sf.Width, sf.Height),false);
        }

        public void DrawString(string content, Color color, RectF rect,bool center)
        {
            _surface.Font = DefaultFont;
            _surface.StrokeColor = color;
            _surface.DrawString(content, rect.X, rect.Y, (center ? HorizontalAlignment.Center : HorizontalAlignment.Left));
        }

        public void DrawRoundRectangle(Pen pen, RectF rect,float radius)
        {
            _SetPen(pen);
            _surface.DrawRoundedRectangle(rect, radius);
        }

        public static Image FromStream(Stream str)
        {
            return new Image(str);
        }

        public Color GetPixel(int x, int y)
        {
            var scolor = _image.Bitmap.GetPixel(x, y);
            return new Color(scolor.Red, scolor.Green, scolor.Blue, scolor.Alpha);
        }

        public void SetPixel(int x, int y, Color color)
        {
            _image.Bitmap.SetPixel(x, y, new SkiaSharp.SKColor((byte)color.Red, (byte)color.Green, (byte)color.Blue, (byte)color.Alpha));
        }

        public byte[] ToFile(ImageFormat format)
        {
            return _image.Image.AsBytes(format);
        }

        public void Dispose()
        {
            if (_image!=null)
                _image.Dispose();
        }
    }
}
